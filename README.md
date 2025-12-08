# ModelScope 课程综合实践项目 (ModelScope Game)

这是一个基于 Unity 引擎开发的 2D 游戏项目，作为 ModelScope 课程的综合实践成果。本项目包含多个章节的关卡、剧情对话系统以及基于 Unity Input System 的输入控制。

## 📁 项目目录结构

项目遵循标准的 Unity 工程结构，核心资源位于 `Assets` 目录下：

### 核心资源 (`Assets/`)

*   **Resources/**: 存放动态加载的资源，按章节和功能模块分类。
    *   `chapter0` - `chapter6`: 各章节专属资源（背景图、道具 Sprite、对话数据等）。
    *   `Character`: 角色相关资源。
    *   `Cutscene`: 过场动画资源。
    *   `Dialogs`: 对话系统数据（`.asset` 文件）及语音文件（`.mp3`）。
    *   `InputSystem`: 输入配置文件。
    *   `UI`: 用户界面相关资源。
    *   `Videos`: 视频资源。
*   **Scenes/**: 存放游戏场景文件 (`.unity`)。
*   **Scripts/**: 存放所有 C# 脚本代码。
*   **Sound/**: 全局音效和背景音乐。
*   **Texture/**: 通用贴图和纹理素材。
*   **StreamingAssets/**: 流式资源文件夹。
*   **Settings/**: 项目配置文件（如 URP 设置）。

### 脚本模块 (`Assets/Scripts/`)

代码逻辑按功能模块划分：

*   **Core/System**:
    *   `InputSystem`: 玩家输入控制逻辑。
    *   `Interactive`: 交互系统逻辑。
    *   `UI`: 界面管理与交互逻辑。
    *   `Animator`: 动画控制逻辑。
*   **Gameplay**:
    *   `Character`: 角色控制器与行为逻辑。
    *   `Level`: 关卡管理器。
    *   `chapter[N]`: 特定章节的定制化脚本逻辑。
*   **Root Scripts** (位于 `Assets/` 根目录的通用工具):
    *   `NEXTLEVEL.cs` (Class `SceneTrigger`): 处理场景切换触发逻辑。
    *   `NEXT_LEVEL_TWO_PEOPLE.cs`: 双人/双角色触发的场景切换逻辑。
    *   `DialogStarter.cs`: 用于延时启动对话剧情。
    *   `showDataPath.cs`: 调试工具，显示数据路径。
    *   `showHintOnce.cs`: 提示信息显示逻辑。

## 🛠️ 开发与协作指南

### 协作注意事项
1.  **场景文件冲突**: 
    *   **原则**: 尽量避免两人同时修改同一个 `.unity` 场景文件。
    *   **解决方案**: 如果发生冲突，建议先接受双方的更改（Git Merge），然后进入 Unity Editor 手动剔除重复或错误的 GameObject。
2.  **资源命名**: 保持清晰的命名规范，尽量使用英文，避免特殊字符。
3.  **提交规范**: 
    *   定期发布，保持小步快跑。
    *   Commit Message 格式参考: `feat: 新增功能`, `fix: 修复bug`, `docs: 文档更新`。

### 关键系统说明

*   **渲染管线**: 项目使用 Universal Render Pipeline (URP)。
*   **对话系统**: 基于 ScriptableObject (`.asset`) 存储对话内容，支持语音播放。
*   **场景跳转**: 使用 `SceneTrigger` 组件挂载在 Trigger Collider 物体上，指定 `Next Scene Name` 或 `Index` 即可实现跳转。

## 🚀 快速开始

1.  克隆仓库到本地。
2.  使用 Unity Hub 打开项目（确保 Unity 版本与项目兼容）。
3.  打开 `Assets/Scenes` 目录下的初始场景（通常为 Start 或 Chapter0 相关场景）开始运行。

---
*文档更新时间: 2024*