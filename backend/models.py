from pydantic import BaseModel, Field, SecretStr
from typing import List, Literal, Union

# --- Request Models ---

class ChatRequest(BaseModel):
    """
    Defines the structure for an incoming chat request from the client.
    """
    session_id: str = Field(
        ..., 
        description="A unique identifier for the user's conversation session.",
        example="user123_sessionABC"
    )
    text: str = Field(
        ..., 
        description="The text content of the user's message.",
        example="告诉我关于SWK的计划。"
    )
    provider: Literal["openai", "deepseek", "google", "qwen"] = Field(
        ..., 
        description="用户选择用于生成回复的AI服务商。",
        example="qwen"
    )
    api_key: SecretStr = Field(
        ..., 
        description="用户提供的对应AI服务商的API Key。这是一个敏感字段。"
    )


# --- Response Models ---
# We define a separate model for each possible response type.

# For "dialogue" type responses
class DialogueItem(BaseModel):
    character: str = Field(..., example="SWK")
    text: str = Field(..., example="我的计划？就是打破这一切虚伪的枷锁！")

class DialogueData(BaseModel):
    responses: List[DialogueItem]

class DialogueResponse(BaseModel):
    type: Literal["dialogue"]
    data: DialogueData

# For "clarification" type responses
class ClarificationResponse(BaseModel):
    type: Literal["clarification"]
    data: List[str] = Field(..., example=["你是想问关于打破旧世界秩序的计划吗？", "还是指如何应对ADA的方舟计划？"])

# For "guidance" type responses
class GuidanceData(BaseModel):
    text: str = Field(..., example="我们好像聊远了，还是继续刚才的话题吧。")

class GuidanceResponse(BaseModel):
    type: Literal["guidance"]
    data: GuidanceData

# A Union of all possible response models.
# FastAPI will use this to correctly serialize the response and generate documentation.
APIResponse = Union[DialogueResponse, ClarificationResponse, GuidanceResponse]