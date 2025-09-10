# Unity视频播放设置指南

## 概述
本指南帮助你在Unity中设置一个在主菜单之前播放的介绍视频，支持玩家按键跳过。

## 文件结构
- `VideoPlayerController.cs` - 视频播放控制脚本
- `IntroVideo.unity` - 视频播放场景

## 设置步骤

### 1. 场景设置
1. **IntroVideo场景** - 视频播放场景（作为游戏的第一个场景）
2. **MainMenu场景** - 你现有的主菜单场景

### 2. Build Settings配置
1. 打开 `File > Build Settings`
2. 确保场景顺序如下：
   - `Assets/Scenes/Functional/IntroVideo.unity` (索引 0)
   - `Assets/Scenes/Functional/MainMenu.unity` (索引 1)
   - 其他游戏场景...

### 3. 视频文件准备
1. 将你的视频文件放在 `Assets/Resources/Videos/` 文件夹中
2. 支持的视频格式：`.mp4`, `.mov`, `.avi`, `.webm`
3. 推荐设置：
   - 分辨率：1920x1080 或更低
   - 编码：H.264
   - 音频：AAC

### 4. IntroVideo场景配置
1. 打开 `IntroVideo.unity` 场景
2. 选择 `VideoPlayerController` 游戏对象
3. 在Inspector中配置：
   - **Intro Video**: 拖拽你的视频文件到此处
   - **Next Scene Name**: 设置为 "MainMenu"
   - **Allow Skip**: 勾选以允许跳过
   - **Skip Prompt Delay**: 设置多少秒后显示跳过提示（默认2秒）
   - **Skip Video In Editor**: 在编辑器中测试时是否跳过视频

### 5. 添加UI元素（可选）
为了更好的用户体验，你可以添加以下UI元素：

#### 跳过提示UI
1. 在IntroVideo场景中创建一个Canvas
2. 添加一个Text或TextMeshPro组件显示"按任意键跳过"
3. 将此GameObject拖拽到VideoPlayerController的 `Skip Prompt` 字段
4. 将Text组件拖拽到 `Skip Text` 字段

#### 淡入淡出效果
1. 在Canvas下创建一个Panel
2. 设置Panel的颜色为黑色，Alpha为1
3. 添加CanvasGroup组件
4. 将Panel拖拽到VideoPlayerController的 `Fade Canvas` 字段



## 功能特性

### 输入检测
- 键盘任意按键
- 鼠标左键、右键、中键
- 手柄按键（通过Input System）

### 视觉效果
- 淡入淡出过渡效果
- 跳过提示闪烁动画
- 平滑的场景切换

### 调试功能
- 编辑器中可选择跳过视频
- Console日志输出
- 错误处理和回退机制

## 常见问题

### Q: 视频不播放怎么办？
A: 检查：
1. 视频文件格式是否支持
2. VideoPlayer组件是否正确配置
3. 视频文件路径是否正确

### Q: 如何更改跳过按键？
A: 修改 `VideoPlayerController.cs` 中的 `Update()` 方法，自定义输入检测逻辑。

### Q: 如何添加多个视频？
A: 可以扩展 `VideoPlayerController` 脚本，添加视频列表功能。

### Q: 如何在手机平台测试？
A: 确保视频格式兼容目标平台，并在真机上测试触摸输入。

## 扩展建议

1. **添加设置选项** - 让玩家可以在设置中关闭介绍视频
2. **添加字幕支持** - 为视频添加多语言字幕
3. **添加进度条** - 显示视频播放进度
4. **添加音量控制** - 独立控制视频音量
5. **添加首次启动检测** - 只在首次启动时播放视频

## 快速设置总结

### 🚀 简化设置（推荐）
1. **设置Build Settings**：
   - 将 `IntroVideo.unity` 设为第一个场景（索引0）
   - 将 `MainMenu.unity` 设为第二个场景（索引1）

2. **配置视频播放**：
   - 将视频文件放入 `Assets/Resources/Videos/` 文件夹
   - 在IntroVideo场景中配置VideoPlayerController组件
   - 设置视频文件引用和目标场景名称

3. **测试运行**：
   - 运行游戏，视频会自动播放
   - 按任意键可以跳过视频
   - 视频结束后自动跳转到MainMenu

### ✅ 这样设置的优点：
- 更简洁的场景结构
- 更少的文件需要管理
- 更直接的启动流程
- 易于调试和维护

## 注意事项

- 视频文件会增加游戏包体大小
- 不同平台的视频性能可能不同
- 建议提供跳过选项以改善用户体验
- 确保视频内容适合目标年龄段
