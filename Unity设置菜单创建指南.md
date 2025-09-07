# Unity 设置菜单 UI 创建指南

## 概述

本指南将帮助你在Unity中创建一个完整的设置菜单UI，支持音频、显示和键位绑定设置。

## 前置要求

1. **Audio Mixer**: 需要创建一个Audio Mixer来控制音频
2. **Input System**: 确保已安装Unity Input System包
3. **TextMeshPro**: 确保已导入TextMeshPro

## 步骤 1: 创建Audio Mixer

1. 在Project窗口中右击 → Create → Audio → Audio Mixer
2. 命名为 "MainAudioMixer"
3. 在Audio Mixer窗口中：
   - 创建两个子组：Music 和 SFX
   - 右击Groups → Add child group
   - 分别命名为 "Music" 和 "SFX"
4. 为每个组添加参数：
   - 选择Music组 → 在右侧添加exposed parameter → 命名为 "MusicVolume"
   - 选择SFX组 → 在右侧添加exposed parameter → 命名为 "SFXVolume"

## 步骤 2: 创建Canvas和UI结构

### 2.1 创建主Canvas
1. Hierarchy窗口 → 右击 → UI → Canvas
2. 命名为 "SettingsCanvas"
3. 设置Canvas Scaler：
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080
   - Match: 0.5

### 2.2 创建设置面板
1. 右击SettingsCanvas → UI → Panel
2. 命名为 "SettingsPanel"
3. 设置RectTransform：
   - Anchor: stretch (按Alt+Shift点击右下角锚点选项)
   - Left, Top, Right, Bottom都设为0

### 2.3 创建标题
1. 右击SettingsPanel → UI → Text - TextMeshPro
2. 命名为 "TitleText"
3. 设置属性：
   - Text: "Settings"
   - Font Size: 48
   - Alignment: Center
   - 位置：顶部居中

## 步骤 3: 创建音频设置区域

### 3.1 创建音频容器
1. 右击SettingsPanel → Create Empty
2. 命名为 "AudioSection"
3. 添加Vertical Layout Group组件
4. 设置Layout Group：
   - Control Child Size: Width ✓, Height ✓
   - Use Child Scale: Width ✓, Height ✓
   - Child Force Expand: Width ✓

### 3.2 创建音频标题
1. 右击AudioSection → UI → Text - TextMeshPro
2. 命名为 "AudioTitle"
3. 设置Text: "Audio Settings"

### 3.3 创建音乐音量滑块
1. 右击AudioSection → Create Empty
2. 命名为 "MusicVolumeContainer"
3. 添加Horizontal Layout Group组件
4. 创建子对象：
   - Text (命名为MusicLabel，内容："Music Volume")
   - Slider (命名为MusicVolumeSlider)
   - Text (命名为MusicVolumeText，内容："80%")

### 3.4 创建音效音量滑块
1. 复制MusicVolumeContainer
2. 重命名为 "SFXVolumeContainer"
3. 修改标签文本为 "SFX Volume"
4. 重命名滑块为 "SFXVolumeSlider"

### 3.5 创建静音开关
1. 右击AudioSection → UI → Toggle
2. 命名为 "MuteToggle"
3. 修改Label文本为 "Mute"

## 步骤 4: 创建显示设置区域

### 4.1 创建显示容器
1. 右击SettingsPanel → Create Empty
2. 命名为 "DisplaySection"
3. 添加Vertical Layout Group组件

### 4.2 创建全屏开关
1. 右击DisplaySection → UI → Toggle
2. 命名为 "FullscreenToggle"
3. 修改Label文本为 "Fullscreen"

### 4.3 创建分辨率下拉菜单
1. 右击DisplaySection → Create Empty
2. 命名为 "ResolutionContainer"
3. 添加Horizontal Layout Group组件
4. 创建子对象：
   - Text (内容："Resolution")
   - Dropdown - TextMeshPro (命名为ResolutionDropdown)

### 4.4 创建画质下拉菜单
1. 复制ResolutionContainer
2. 重命名为 "QualityContainer"
3. 修改文本为 "Quality"
4. 重命名下拉菜单为 "QualityDropdown"

## 步骤 5: 创建键位绑定区域

### 5.1 创建键位绑定容器
1. 右击SettingsPanel → Create Empty
2. 命名为 "KeyBindingSection"
3. 添加Vertical Layout Group组件

### 5.2 创建键位绑定标题
1. 右击KeyBindingSection → UI → Text - TextMeshPro
2. 命名为 "KeyBindingTitle"
3. 设置Text: "Key Bindings"

### 5.3 创建键位绑定滚动区域
1. 右击KeyBindingSection → UI → Scroll View
2. 命名为 "KeyBindingScrollView"
3. 设置Content的Layout Group：
   - 添加Vertical Layout Group
   - Control Child Size: Width ✓, Height ✓
   - Use Child Scale: Width ✓, Height ✓

### 5.4 创建键位绑定项目预制体
1. 创建空GameObject，命名为 "KeyBindingItemPrefab"
2. 添加Horizontal Layout Group组件
3. 创建子对象：
   - Text (命名为FunctionName，用于显示功能名称)
   - Button (命名为KeyButton，用于显示和修改按键)
     - 在Button内创建Text (命名为KeyText)
   - 可选：创建等待指示器对象 (命名为WaitingIndicator)
4. 将此对象制作成Prefab：
   - 拖拽到Project窗口中
   - 然后从Hierarchy中删除

### 5.5 创建重置按键按钮
1. 右击KeyBindingSection → UI → Button
2. 命名为 "ResetKeysButton"
3. 修改Button文本为 "Reset Keys"

## 步骤 6: 创建控制按钮

### 6.1 创建按钮容器
1. 右击SettingsPanel → Create Empty
2. 命名为 "ButtonSection"
3. 添加Horizontal Layout Group组件
4. 设置spacing适当间距

### 6.2 创建控制按钮
1. 右击ButtonSection → UI → Button (重复3次)
2. 分别命名为：
   - "BackButton" (文本："Back")
   - "ApplyButton" (文本："Apply")
   - "ResetAllButton" (文本："Reset All")

## 步骤 7: 设置脚本组件

### 7.1 设置SettingsMenuManager
1. 为SettingsPanel添加SettingsMenuManager脚本
2. 在Inspector中分配所有UI引用：
   - Settings Panel: SettingsPanel
   - 各种按钮、滑块、开关等
   - Audio Mixer: 前面创建的MainAudioMixer
   - Key Binding Item Prefab: 前面创建的预制体

### 7.2 设置KeyBindingItem预制体
1. 选择KeyBindingItemPrefab
2. 添加KeyBindingItem脚本
3. 分配UI引用：
   - Function Name Text: FunctionName
   - Key Button: KeyButton  
   - Key Text: KeyText
   - Waiting For Input Indicator: WaitingIndicator (如果有)

### 7.3 创建AudioManager
1. 创建空GameObject，命名为 "AudioManager"
2. 添加AudioManager脚本
3. 分配Audio Mixer引用

### 7.4 设置Input Actions
确保项目中有InputActionAsset，并且包含以下Action：
- MoveUp
- MoveDown  
- MoveLeft
- MoveRight
- Interact
- ExitMenu

## 步骤 8: 测试和调试

### 8.1 初始测试
1. 进入Play模式
2. 测试各种设置是否正常工作：
   - 音量滑块是否影响音频
   - 全屏开关是否工作
   - 分辨率切换是否正常
   - 键位绑定是否可以修改

### 8.2 常见问题解决
1. **音频不工作**：检查Audio Mixer参数名称是否正确
2. **键位绑定不工作**：确保KeymapManager正确初始化
3. **UI布局混乱**：检查Layout Group设置
4. **分辨率不应用**：确保点击Apply按钮

## 步骤 9: 样式优化（可选）

### 9.1 美化UI
1. 为面板添加背景图片或颜色
2. 为按钮添加悬停效果
3. 添加动画过渡
4. 统一字体和颜色主题

### 9.2 添加音效
1. 为按钮添加点击音效
2. 为滑块添加滑动音效
3. 在AudioManager中设置音效剪辑

## 使用说明

### 打开设置菜单
```csharp
// 从其他脚本调用
SettingsMenuManager settingsMenu = FindObjectOfType<SettingsMenuManager>();
settingsMenu.OpenSettingsMenu();
```

### 保存设置
设置会自动保存到PlayerPrefs，下次启动游戏时会自动加载。

### 自定义按键绑定
玩家可以点击按键按钮，然后按下新的按键来重新绑定功能。

这样就完成了一个功能完整的设置菜单系统！
