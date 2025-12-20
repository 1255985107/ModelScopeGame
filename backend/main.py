import uvicorn
from fastapi import FastAPI, HTTPException
from fastapi.responses import RedirectResponse

from services.chat_service import ChatService
from models import ChatRequest, APIResponse

app = FastAPI(
    title="AI BYOK Chat API",
    description="一个“自带Key”的AI叙事聊天API，用户可以动态提供API Key来使用不同的AI服务商。",
    version="2.0.0",
)

chat_service = ChatService()

@app.get("/", include_in_schema=False)
async def root():
    return RedirectResponse(url="/docs")

@app.post("/chat/send", response_model=APIResponse)
async def handle_chat(request: ChatRequest):
    """
    处理用户聊天消息的主端点。

    客户端在此端点提供会话ID、文本、AI服务商(`provider`)以及他们自己的API Key(`api_key`)。
    """
    try:
        response = chat_service.process_user_input(
            session_id=request.session_id,
            user_text=request.text,
            provider=request.provider,
            api_key=request.api_key
        )
        return response
    
    except ValueError as e:
        # 捕获由 _get_llm_client 抛出的 provider 无效的错误
        print(f"数据处理或验证错误: {e}")
        raise HTTPException(
            status_code=400, # Bad Request
            detail=str(e)
        )
    except Exception as e:
        # 捕获所有其他异常，特别是来自AI SDK的认证失败
        import traceback
        traceback.print_exc() # 打印完整堆栈信息以供调试
        error_message = str(e).lower()
        
        # 检查常见的认证失败关键词
        if "authentication" in error_message or "api key" in error_message or "invalid_api_key" in error_message:
            raise HTTPException(
                status_code=401, # Unauthorized
                detail=f"提供的 {request.provider} API Key 无效或不正确。"
            )
        
        # 对于其他未知错误，返回500
        print(f"An unexpected error occurred: {e}")
        raise HTTPException(
            status_code=500, 
            detail="服务器内部发生未知错误。"
        )

if __name__ == "__main__":
    uvicorn.run("main:app", host="0.0.0.0", port=8000, reload=True)