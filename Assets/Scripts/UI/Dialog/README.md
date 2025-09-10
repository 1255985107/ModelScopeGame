# Unity 对话系统使用指南

## 概述

这个对话系统为Unity提供了完整的对话功能，包括：
- 文字打字机效果显示
- 语音播放同步
- 角色头像显示
- 自定义文字颜色和显示速度
- 交互式对话推进
- 自动播放模式
- 完整的编辑器支持

## 快速开始

### 1. 自动设置（推荐）

使用菜单 `Tools > 对话系统 > 完整设置` 来自动创建所有必要组件。

### 2. 手动设置

#### 2.1 创建对话管理器
1. 使用菜单 `Tools > 对话系统 > 创建对话管理器`
2. 或手动创建GameObject并添加 `DialogManager` 和 `DialogUIManager` 组件

#### 2.2 创建对话UI
1. 使用菜单 `Tools > 对话系统 > 创建对话UI`
2. 或手动创建UI面板并设置以下组件：
   - 对话面板 (GameObject with CanvasGroup)
   - 角色名字文本 (TextMeshProUGUI)
   - 对话内容文本 (TextMeshProUGUI)
   - 角色头像 (Image)
   - 继续按钮 (Button) - 可选

#### 2.3 设置输入系统
确保项目中有InputActionAsset，并包含"Interact"动作。

## 创建对话内容

### 创建对话序列资产

1. 在Project窗口中右击
2. 选择 `Create > 对话系统 > 对话序列`
3. 配置对话内容：

```csharp
// 对话数据结构
public class DialogData
{
    public string characterName;        // 角色名
    public string dialogText;           // 对话文本
    public AudioClip voiceClip;         // 语音剪辑
    public float textDisplaySpeed;      // 文字显示速度
    public bool waitForInput;           // 是否等待玩家输入
    public float autoAdvanceDelay;      // 自动继续延迟
    public Sprite characterPortrait;    // 角色头像
    public Color textColor;             // 文字颜色
}
```

### 对话序列设置

- **Loop Sequence**: 是否循环播放对话
- **Pause Game During Dialog**: 对话期间是否暂停游戏（设置Time.timeScale = 0）

## 触发对话

### 方式一：使用DialogTrigger组件

1. 使用菜单 `Tools > 对话系统 > 创建对话触发器`
2. 设置触发方式：
   - **OnTriggerEnter**: 玩家进入触发区域时自动播放
   - **OnInteract**: 玩家在触发区域内按交互键播放
   - **Manual**: 手动调用代码触发

3. 配置触发器：
   - 分配对话序列资产
   - 设置触发区域大小
   - 配置交互按键
   - 设置是否只触发一次

### 方式二：通过代码触发

```csharp
public class ExampleScript : MonoBehaviour
{
    public DialogSequence myDialog;
    
    void Start()
    {
        DialogManager dialogManager = DialogManager.Instance;
        if (dialogManager != null)
        {
            dialogManager.StartDialog(myDialog);
        }
    }
}
```

## 代码API

### DialogManager 主要方法

```csharp
// 开始对话
public void StartDialog(DialogSequence dialogSequence);

// 前进到下一个对话
public void AdvanceDialog();

// 结束对话
public void EndDialog();

// 跳过当前对话序列
public void SkipDialog();

// 检查是否正在进行对话
public bool IsDialogActive();

// 获取当前对话进度
public (int current, int total) GetDialogProgress();
```

### 事件监听

```csharp
void OnEnable()
{
    DialogManager.OnDialogStart += OnDialogStarted;
    DialogManager.OnDialogEnd += OnDialogEnded;
    DialogManager.OnDialogAdvance += OnDialogAdvanced;
}

void OnDisable()
{
    DialogManager.OnDialogStart -= OnDialogStarted;
    DialogManager.OnDialogEnd -= OnDialogEnded;
    DialogManager.OnDialogAdvance -= OnDialogAdvanced;
}

private void OnDialogStarted()
{
    // 对话开始时的逻辑
}

private void OnDialogEnded()
{
    // 对话结束时的逻辑
}

private void OnDialogAdvanced(DialogData dialogData)
{
    // 每个对话推进时的逻辑
}
```

## 运行时创建对话

```csharp
private void CreateDynamicDialog()
{
    DialogSequence dynamicDialog = ScriptableObject.CreateInstance<DialogSequence>();
    
    dynamicDialog.dialogs = new DialogData[]
    {
        new DialogData
        {
            characterName = "NPC",
            dialogText = "这是动态创建的对话！",
            textDisplaySpeed = 0.05f,
            waitForInput = true,
            textColor = Color.white
        }
    };
    
    DialogManager.Instance.StartDialog(dynamicDialog);
}
```

## 高级功能

### 自定义UI样式

在DialogUIManager中可以自定义：
- 面板淡入淡出效果
- 打字机音效
- 文本颜色和样式
- 头像显示逻辑

### 与其他系统集成

```csharp
public class GameController : MonoBehaviour
{
    void OnEnable()
    {
        DialogManager.OnDialogStart += DisablePlayerInput;
        DialogManager.OnDialogEnd += EnablePlayerInput;
    }
    
    private void DisablePlayerInput()
    {
        // 禁用玩家输入
        PlayerController.Instance.enabled = false;
    }
    
    private void EnablePlayerInput()
    {
        // 恢复玩家输入
        PlayerController.Instance.enabled = true;
    }
}
```

## 性能优化建议

1. **预加载音频**: 将常用的语音剪辑预加载到内存
2. **对象池**: 对于频繁创建的UI元素使用对象池
3. **异步加载**: 大型对话序列可以考虑异步加载
4. **内存管理**: 及时清理不再使用的对话序列

## 故障排除

### 常见问题

1. **对话不显示**
   - 检查DialogUIManager是否正确设置UI组件引用
   - 确认Canvas和UI元素正常工作

2. **输入无响应**
   - 检查InputActionAsset是否包含"Interact"动作
   - 确认输入系统正确配置

3. **语音不播放**
   - 检查AudioManager是否存在
   - 确认音频剪辑格式正确

4. **文字显示异常**
   - 检查TextMeshPro是否正确导入
   - 确认字体资源可用

### 调试工具

- 启用DialogTrigger的Debug Mode查看详细日志
- 使用编辑器工具实时监控对话状态
- 在Scene视图中查看触发区域Gizmos

## 扩展开发

系统采用模块化设计，可以轻松扩展：

1. **自定义对话数据**: 继承DialogData添加新字段
2. **新的触发方式**: 扩展DialogTrigger.TriggerType
3. **UI效果**: 在DialogUIManager中添加新的视觉效果
4. **音频系统**: 集成更复杂的音频管理逻辑

## 示例场景

参考DialogExample.cs了解完整的使用示例，包括：
- 基本对话播放
- 运行时对话创建
- 事件监听处理
- 与游戏逻辑集成
