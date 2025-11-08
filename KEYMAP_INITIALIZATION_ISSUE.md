# 从 Level6 直接启动按键输入失效问题分析

## 🔴 问题症状

| 场景 | 左右移动 | 状态 |
|-----|--------|------|
| 从 Intro → Level6 | ✅ 工作 | 正常 |
| 直接启动 Level6 | ❌ 失效 | **按键无反应** |

---

## 🎯 根本原因分析

### 问题链：直接启动 Level6 → KeymapManager 未初始化 → 按键无反应

```
┌─────────────────────────────────────────────────────┐
│ 从 Intro 启动                                       │
├─────────────────────────────────────────────────────┤
│ 1. Intro 场景加载                                   │
│    ↓                                                 │
│ 2. KeymapManager.Awake() 执行                       │
│    ↓ 创建单例                                        │
│ 3. KeymapManager.InitializeInputActions()           │
│    ↓ 初始化 InputActionAsset                        │
│ 4. Level6 加载                                       │
│    ↓                                                 │
│ 5. PlayerController 读取 KeymapManager.Singleton ✅ │
│    ↓ 单例已经存在                                    │
│ 6. 按键正常工作                                      │
└─────────────────────────────────────────────────────┘

vs

┌─────────────────────────────────────────────────────┐
│ 直接启动 Level6                                     │
├─────────────────────────────────────────────────────┤
│ 1. Level6 场景加载                                  │
│    ↓                                                 │
│ 2. PlayerController.Start() 尝试访问 KeymapManager │
│    ↓ KeymapManager 还没有初始化！                   │
│ 3. KeymapManager.Singleton == null ❌               │
│    ↓                                                 │
│ 4. if (KeymapManager.Singleton != null) 条件失败   │
│    ↓                                                 │
│ 5. moveInput 无法设置 → 按键无反应                  │
└─────────────────────────────────────────────────────┘
```

---

## 📋 关键代码位置

### PlayerController.cs 中的问题代码：

```csharp
private void HandleMovement()
{
    if (!canMove) return;

    float moveInput = 0f;
    if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady)  // ⚠️ 问题在这里
    {
        if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveLeft))
            moveInput -= 1f;
        if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveRight))
            moveInput += 1f;
    }
    // 如果 KeymapManager.Singleton == null，moveInput 始终为 0
    
    rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);  // ❌ 无法移动
}
```

### KeymapManager.cs 中的初始化：

```csharp
void Awake()
{
    if (Singleton == null)
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
        InitializeInputActions();  // ← 初始化输入系统
        IsReady = true;
    }
    else
    {
        Destroy(gameObject);
    }
}
```

**问题：** 如果 Level6 中没有 KeymapManager，Singleton 就无法初始化！

---

## ✅ 解决方案（选一个）

### 方案 A：懒加载 KeymapManager（推荐 ⭐）

在 `KeymapManager.cs` 中添加懒加载逻辑：

```csharp
public class KeymapManager : MonoBehaviour
{
    public static KeymapManager Singleton 
    { 
        get 
        { 
            // 如果不存在，自动创建一个
            if (_singleton == null)
            {
                _singleton = FindObjectOfType<KeymapManager>();
                if (_singleton == null)
                {
                    // 如果场景中也没有，从预制体创建
                    GameObject keymapObject = new GameObject("KeymapManager");
                    _singleton = keymapObject.AddComponent<KeymapManager>();
                    Debug.Log("KeymapManager 不存在，已自动创建");
                }
            }
            return _singleton;
        }
        private set { _singleton = value; }
    }
    private static KeymapManager _singleton;

    public bool IsReady { get; private set; } = false;

    void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Singleton = this;
        DontDestroyOnLoad(gameObject);
        InitializeInputActions();
        IsReady = true;
    }
}
```

**优点：**
- ✅ 自动初始化
- ✅ 不需要修改 Build Settings
- ✅ 从任何场景启动都能工作
- ✅ 代码改动最小

---

### 方案 B：将 KeymapManager 添加到每个场景

**步骤：**

1. 打开 Intro 场景，找到 KeymapManager GameObject
2. 复制这个 GameObject（Ctrl + D）
3. 打开 Level6 场景，粘贴（Ctrl + V）
4. 确保两个 KeymapManager 的配置完全相同

**优点：**
- ✅ 简单直接
- ❌ 重复代码
- ❌ 需要每个场景都手动配置

---

### 方案 C：确保 Intro 必须加载

在 Build Settings 中：

```
File → Build Settings

Scenes In Build:
  0. Scenes/Intro          ✅ 必须是第 0 个
  1. Scenes/Level1
  2. Scenes/Level2
  ...
  6. Scenes/Level6
```

然后在游戏启动时总是加载 Intro：

```csharp
void Start()
{
    if (SceneManager.GetActiveScene().name != "Intro")
    {
        SceneManager.LoadScene("Intro");
    }
}
```

**优点：**
- ✅ 保证单例初始化顺序
- ❌ 每次都要加载 Intro 场景
- ❌ 如果编辑器中直接播放 Level6 仍会出问题

---

## 🔧 推荐的完整解决方案

### 第 1 步：修改 KeymapManager.cs（懒加载）

```csharp
public class KeymapManager : MonoBehaviour
{
    private static KeymapManager _singleton;
    
    public static KeymapManager Singleton 
    { 
        get 
        { 
            if (_singleton == null)
            {
                _singleton = FindObjectOfType<KeymapManager>();
                if (_singleton == null)
                {
                    Debug.LogWarning("KeymapManager not found in scene, creating one...");
                    GameObject keymapObject = new GameObject("KeymapManager");
                    _singleton = keymapObject.AddComponent<KeymapManager>();
                }
            }
            return _singleton;
        }
    }

    public bool IsReady { get; private set; } = false;

    [Header("Input Actions")]
    public InputActionAsset inputActions;

    // ... 其他代码保持不变 ...

    void Awake()
    {
        if (_singleton != null && _singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _singleton = this;
        DontDestroyOnLoad(gameObject);
        InitializeInputActions();
        IsReady = true;
        Debug.Log("KeymapManager 初始化完成");
    }
}
```

### 第 2 步：在 KeymapPrefManager 中添加容错

```csharp
private IEnumerator LoadKeymap()
{
    // 尝试获取 KeymapManager，如果不存在会自动创建
    int maxWait = 30; // 最多等待 3 秒
    while ((KeymapManager.Singleton == null || !KeymapManager.Singleton.IsReady) && maxWait > 0)
    {
        yield return new WaitForSeconds(0.1f);
        maxWait--;
    }

    if (KeymapManager.Singleton == null)
    {
        Debug.LogError("KeymapManager 初始化超时！");
        yield break;
    }

    yield return null;
    LoadKeymapFromPlayerPrefs();
}
```

### 第 3 步：在 PlayerController 中添加容错

```csharp
private void HandleMovement()
{
    if (!canMove) return;

    float moveInput = 0f;
    
    // 如果 KeymapManager 不存在，尝试获取（会自动创建）
    if (KeymapManager.Singleton == null)
    {
        Debug.LogWarning("KeymapManager.Singleton 为 null，等待初始化...");
        return;
    }
    
    if (KeymapManager.Singleton.IsReady)
    {
        if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveLeft))
            moveInput -= 1f;
        if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveRight))
            moveInput += 1f;
    }

    rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
    
    // ... 其他代码 ...
}
```

---

## 🧪 测试方法

### 测试 1：从 Intro 启动（应该工作）
```
Editor 中选择 Intro 场景
按下 Play
从 Intro 过关到 Level6
检查：左右移动 ✅ 工作
```

### 测试 2：直接从 Level6 启动（修复前会失效，修复后应该工作）
```
Editor 中选择 Level6 场景
按下 Play
检查：左右移动 ❌ 失效（修复前）
            ✅ 工作（修复后）
```

---

## 📊 对比三种方案

| 方案 | 改动代码 | 编辑器测试 | 打包后 | 推荐度 |
|-----|--------|---------|-------|------|
| A: 懒加载 | 最小 | ✅ 直接启动任何场景工作 | ✅ 最佳 | ⭐⭐⭐⭐⭐ |
| B: 每个场景 | 无 | ❌ 需要手动放置 | ✅ 工作 | ⭐⭐ |
| C: 强制 Intro | 中等 | ⚠️ 每次都加载 Intro | ✅ 工作 | ⭐⭐⭐ |

---

## 🎯 立即执行的步骤

1. **打开 KeymapManager.cs**
2. **修改 Singleton 属性为懒加载版本**
3. **保存并重新运行**
4. **直接从 Level6 启动测试**

完成后，左右移动应该能工作！

---

## 🐛 调试技巧

如果修复后仍不工作，在 PlayerController.HandleMovement() 顶部添加：

```csharp
private void HandleMovement()
{
    if (!canMove) return;

    Debug.Log($"[HandleMovement] KeymapManager.Singleton: {KeymapManager.Singleton} | IsReady: {KeymapManager.Singleton?.IsReady}");
    
    // ... 其他代码 ...
}
```

观察 Console 输出：
- ✅ `KeymapManager.Singleton: KeymapManager | IsReady: True` → 正常
- ❌ `KeymapManager.Singleton: null | IsReady: ?` → 初始化失败
