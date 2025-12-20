import os
import json
from pydantic import SecretStr
from typing import List, Dict, TypedDict, Literal, Any

# LangChain & LangGraph
from langchain_core.messages import HumanMessage, AIMessage, BaseMessage
from langchain_core.language_models.chat_models import BaseChatModel
from langgraph.graph import StateGraph, END

# LLM Provider Imports
from langchain_openai import ChatOpenAI
from langchain_deepseek import ChatDeepSeek
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_community.chat_models import ChatTongyi
from pydantic import BaseModel, Field

class AmbiguityOutput(BaseModel):
    options: List[str] = Field(description="澄清选项列表，如果没有歧义请返回空列表")
# 定义对话结果的结构
class DialogueItem(BaseModel):
    character: str = Field(description="说话的角色名，如 SWK 或 ADA")
    text: str = Field(description="角色的对话内容")
class DialogueOutput(BaseModel):
    responses: List[DialogueItem] = Field(description="角色对话回复列表")

class GraphState(TypedDict):
    dialogue_history: List[BaseMessage]
    user_input: str
    relevance_decision: Literal["相关", "不相关"]
    clarification_options: List[str]
    final_response: Dict
    llm: BaseChatModel # 依然保留，用于在图的不同节点间传递动态创建的客户端


class ChatService:
    def __init__(self):
        print("Initializing ChatService...")
        self._load_configs()
        self._load_prompts()
        
        # 不再在启动时初始化任何LLM客户端
        # self.llms 字典被移除

        self.graph = self._build_graph()
        self.sessions: Dict[str, List[BaseMessage]] = {}
        print("ChatService initialized successfully.")

    def _get_llm_client(self, provider: str, api_key: str) -> BaseChatModel:
        """
        一个工厂方法，根据provider和api_key动态创建并返回一个LLM客户端实例。
        """
        if provider == "openai":
            return ChatOpenAI(model="gpt-4o", temperature=0.7, api_key=api_key)
        
        if provider == "deepseek":
            return ChatDeepSeek(model="deepseek-chat", temperature=0.7, api_key=api_key)

        if provider == "google":
            return ChatGoogleGenerativeAI(model="gemini-2.0-flash", temperature=0.7, api_key=api_key)
            
        if provider == "qwen":
            # 使用 OpenAI 兼容模式连接通义千问，这对 structured_output 支持更好
            return ChatOpenAI(
                model="qwen-plus", 
                temperature=0.7, 
                api_key=api_key,
                base_url="https://dashscope.aliyuncs.com/compatible-mode/v1"
            )

        raise ValueError(f"不支持的 provider: '{provider}'。有效选项为: 'openai', 'deepseek', 'google', 'qwen'")
    def _load_configs(self):
        # ... (no changes here) ...
        config_dir = os.path.join(os.path.dirname(__file__), '..', 'config')
        with open(os.path.join(config_dir, 'story.json'), 'r', encoding='utf-8') as f:
            self.story_config = json.load(f)
        with open(os.path.join(config_dir, 'characters.json'), 'r', encoding='utf-8') as f:
            self.character_config = json.load(f)
    def _load_prompts(self):
        # ... (no changes here) ...
        self.prompts = {}
        prompt_dir = os.path.join(os.path.dirname(__file__), '..', 'prompts')
        for filename in os.listdir(prompt_dir):
            if filename.endswith('.txt'):
                key = filename.replace('.txt', '')
                with open(os.path.join(prompt_dir, filename), 'r', encoding='utf-8') as f:
                    self.prompts[key] = f.read()

    def _build_graph(self) -> StateGraph:
        # ... (no changes to the graph structure) ...
        workflow = StateGraph(GraphState)
        workflow.add_node("check_relevance", self._node_check_relevance)
        workflow.add_node("check_ambiguity", self._node_check_ambiguity)
        workflow.add_node("generate_dialogue", self._node_generate_dialogue)
        workflow.add_node("generate_guidance", self._node_generate_guidance)
        workflow.add_node("prepare_clarification", self._node_prepare_clarification)
        workflow.set_entry_point("check_relevance")
        workflow.add_conditional_edges("check_relevance", self._decide_relevance, {"relevant": "check_ambiguity", "irrelevant": "generate_guidance"})
        workflow.add_conditional_edges("check_ambiguity", self._decide_ambiguity, {"clear": "generate_dialogue", "ambiguous": "prepare_clarification"})
        workflow.add_edge("generate_dialogue", END)
        workflow.add_edge("generate_guidance", END)
        workflow.add_edge("prepare_clarification", END)
        return workflow.compile()
    # --- Node Implementations ---
    def _node_check_relevance(self, state: GraphState) -> Dict:
        """Node to check if the user's input is relevant to the conversation."""
        history_str = "\n".join([f"{msg.type}: {msg.content}" for msg in state['dialogue_history'][-6:]])
        prompt = self.prompts['relevance_check'].format(
            key_themes=str(self.story_config['key_themes']),
            dialogue_history=history_str,
            user_input=state['user_input']
        )
        response = state['llm'].invoke(prompt)
        decision = response.content.strip()
        print(f"Relevance Check Decision: {decision}")
        return {"relevance_decision": decision}

    def _node_check_ambiguity(self, state: GraphState) -> Dict:
        """Node to check for ambiguity in the user's input."""
        history_str = "\n".join([f"{msg.type}: {msg.content}" for msg in state['dialogue_history']])
        prompt = self.prompts['ambiguity_check'].format(
            dialogue_history=history_str,
            user_input=state['user_input']
        )
        # Use a model that can reliably output JSON
        json_llm = state['llm'].with_structured_output(AmbiguityOutput)
        result = json_llm.invoke(prompt)
        print(f"Ambiguity Check Options: {result.options}")
        return {"clarification_options": result.options}

    def _node_generate_dialogue(self, state: GraphState) -> Dict:
        """Node to generate the main dialogue response from NPCs."""
        history_str = "\n".join([f"{msg.type}: {msg.content}" for msg in state['dialogue_history']])
        prompt = self.prompts['dialogue_gen'].format(
            story_background=self.story_config['background'],
            narrative_goal=self.story_config['narrative_goal'],
            swk_profile=json.dumps(self.character_config['SWK'], ensure_ascii=False, indent=2),
            ada_profile=json.dumps(self.character_config['ADA'], ensure_ascii=False, indent=2),
            dialogue_history=history_str,
            user_input=state['user_input']
        )
        
        response_llm = state['llm'].with_structured_output(DialogueOutput)
        ai_response = response_llm.invoke(prompt)
        
        print(f"Generated Dialogue: {ai_response}")
        
        # Prepare the final response structure
        final_response = {
            "type": "dialogue",
            "data": {
                "responses": ai_response.responses
            }
        }
        return {"final_response": final_response}

    def _node_generate_guidance(self, state: GraphState) -> Dict:
        """Node to generate a guidance message for irrelevant input."""
        final_response = {
            "type": "guidance",
            "data": {
                "text": "我们好像聊远了，还是继续刚才的话题吧。" # Or generate this with another LLM call for variety
            }
        }
        return {"final_response": final_response}

    def _node_prepare_clarification(self, state: GraphState) -> Dict:
        """Node to format the clarification options into a final response."""
        final_response = {
            "type": "clarification",
            "data": state['clarification_options']
        }
        return {"final_response": final_response}

    # --- Conditional Edge Logic ---
    def _decide_relevance(self, state: GraphState) -> str:
        """Determines the next step based on the relevance check."""
        return "relevant" if state['relevance_decision'] == "相关" else "irrelevant"

    def _decide_ambiguity(self, state: GraphState) -> str:
        """Determines the next step based on the ambiguity check."""
        return "ambiguous" if state['clarification_options'] else "clear"
    
    # --- Public Method for FastAPI ---
    def process_user_input(self, session_id: str, user_text: str, provider: str, api_key: SecretStr) -> Dict:
        """
        处理用户输入的主入口点。
        它现在接收provider和api_key来动态创建LLM客户端。
        """
        # 1. 动态创建LLM客户端
        # .get_secret_value() 用于安全地获取SecretStr中的真实字符串值
        try:
            llm_client = self._get_llm_client(provider, api_key.get_secret_value())
        except Exception as e:
            # 将异常向上抛出，由API层（main.py）来处理并返回HTTP错误
            raise e

        # 2. 检索或创建会话历史
        if session_id not in self.sessions:
            initial_message = AIMessage(content=f"系统情景: {self.story_config['initial_situation']}")
            self.sessions[session_id] = [initial_message]
        session_history = self.sessions[session_id]

        # 3. 将用户消息添加到历史记录
        session_history.append(HumanMessage(content=user_text))

        # 4. 准备图的输入，包含动态创建的客户端
        graph_input = {
            "dialogue_history": session_history,
            "user_input": user_text,
            "llm": llm_client, # 将本次请求的专属客户端传入状态机
        }

        # 5. 调用图进行处理
        final_state = self.graph.invoke(graph_input)
        final_response = final_state['final_response']

        # 6. 更新会话历史
        if final_response['type'] == 'dialogue':
            ai_dialogues = [f"{item.character}: {item.text}" for item in final_response['data']['responses']]
            session_history.append(AIMessage(content="\n".join(ai_dialogues)))
        elif final_response['type'] == 'clarification':
            # 将澄清选项也存入历史，让模型知道“我已经问过用户了”
            clarification_text = "系统提示用户澄清以下选项：\n" + "\n".join(final_response['data'])
            session_history.append(AIMessage(content=clarification_text))
            
        self.sessions[session_id] = session_history
        
        return final_response