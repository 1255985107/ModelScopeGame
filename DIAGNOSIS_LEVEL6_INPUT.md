# Level6 直接启动按键无反馈 - 诊断指南

## 🔍 问题症状

- ✅ 从 Intro 启动 → 按键有反馈，能移动
- ❌ 直接启动 Level6 → 按键无任何反馈，什么都不动

## 🧪 诊断步骤

### 第 1 步：查看 Console 日志

直接启动 Level6 后，打开 Console（Ctrl + Shift + C），观察以下日志：

**关键日志关键词：**
- `[KeymapManager.InitializeInputActions]` - 初始化过程
- `[PlayerController.HandleMovement]` - 按键检查过程

### 第 2 步：检查初始化过程

**预期的成功日志序列：**
```
[KeymapManager.InitializeInputActions] 开始初始化
[KeymapManager.InitializeInputActions] inputActions 是否为 null: True
[KeymapManager.InitializeInputActions] inputActions 为空，尝试加载...
[KeymapManager.InitializeInputActions] 从 Resources 加载结果: 成功  ← 关键！
[KeymapManager.InitializeInputActions] 开始映射功能到动作...
[KeymapManager] ✓ 映射成功: MoveLeft → MoveLeft
[KeymapManager] ✓ 映射成功: MoveRight → MoveRight
...
[KeymapManager] ✓ 初始化完成，IsReady = true
```

**可能的失败情况 1：Resources 加载失败**
```
[KeymapManager.InitializeInputActions] 从 Resources 加载结果: 失败
[KeymapManager.InitializeInputActions] Resources 加载失败，尝试从 PlayerInput 组件查找...
[KeymapManager.InitializeInputActions] 查找 PlayerInput: 未找到
[KeymapManager] 无法加载 InputActionAsset！
```

**可能的失败情况 2：动作映射失败**
```
[KeymapManager] ✗ 映射失败: InputActionAsset 中找不到 'MoveLeft' 动作
```

### 第 3 步：检查按键输入

**预期的按键日志：**
```
[PlayerController.HandleMovement] KeymapManager 就绪，检查按键...
[PlayerController.HandleMovement] MoveLeft: False, MoveRight: True  ← 按下向右键
```

**异常日志 1：Singleton 为 null**
```
[PlayerController.HandleMovement] KeymapManager.Singleton 为 null！
```

**异常日志 2：IsReady 为 false**
```
[PlayerController.HandleMovement] KeymapManager.IsReady = false
```

---

## 🔧 根据日志诊断

### 情况 A：日志显示 "Resources 加载失败"

**原因：** `Assets/Resources/InputActions.asset` 不存在或路径错误

**解决方案：**

**方案 A1：从 Intro 复制 InputActions**

1. 打开 Intro 场景
2. 在 Hierarchy 中选中 KeymapManager
3. 在 Inspector 中看到 InputActions 字段中的资源
4. 记下这个资源的名称（通常是 InputActions）
5. 在 Project 中找到这个文件
6. 将其移到或复制到 `Assets/Resources/` 文件夹中
7. 确保文件名是 `InputActions.asset`

**检查步骤：**
```
Assets/Resources/
  └─ InputActions.asset  ← 这个文件必须存在
```

**方案 A2：设置 Level6 中的 InputActions**

1. 打开 Level6 场景
2. 在 Hierarchy 中找到 **DontDestroyOnLoad** → **KeymapManager (Auto-Created)**
3. 在 Inspector 中找到 **Keymap Manager (Script)** 组件
4. 在 **Input Actions** 字段中拖拽 InputActions asset
5. 重新运行

---

### 情况 B：日志显示 "映射失败"

**症状：**
```
[KeymapManager] ✗ 映射失败: InputActionAsset 中找不到 'MoveLeft' 动作
```

**原因：** InputActionAsset 中的动作名称与代码不匹配

**解决方案：**

1. 打开 InputActions asset 文件（双击）
2. 检查是否有以下动作：
   - MoveLeft
   - MoveRight
   - MoveUp
   - MoveDown
   - Interact
   - ExitMenu

3. 如果缺少，添加这些动作

---

### 情况 C：日志显示 "IsReady = false"

**原因：** InputActionAsset 加载失败

**解决方案：** 执行情况 A 的解决方案

---

### 情况 D：没有 "MoveLeft/MoveRight" 的按键日志

**原因：** IsKeyHeld() 返回 false，可能是：
1. InputActions 未启用
2. 按键绑定错误
3. 输入系统有问题

**诊断：**

在 Console 中查看这行日志：
```
[KeymapManager] InputActions 已启用
```

如果没看到这行，说明 InputActions 对象可能是 null。

---

## ⚡ 快速修复步骤（从 Intro 复制 InputActions）

### 步骤 1：找到 Intro 中的 InputActions

```
File → Open Scene → Intro
  ↓
Hierarchy → KeymapManager
  ↓
Inspector → Keymap Manager (Script)
  ↓
Input Actions 字段 → 右键 → Properties
  ↓
记下文件路径或名称
```

### 步骤 2：确保 InputActions 在 Resources 中

```
Assets/
  ├─ Resources/
  │   └─ InputActions.asset  ← 文件应该在这里
  ├─ Scenes/
  ├─ Scripts/
  └─ ...
```

如果文件在其他地方，需要移到 `Assets/Resources/` 中。

### 步骤 3：重新打开 Level6

```
File → Open Scene → Level6
  ↓
按 Play
  ↓
检查 Console 日志
  ↓
尝试移动（A/D 键或左右箭头）
```

---

## 📊 完整诊断流程图

```
直接启动 Level6
    ↓
┌─ KeymapManager 自动创建
│   ↓
│   InitializeInputActions()
│   ↓
│   ┌─ inputActions == null?
│   │   ↓ 是
│   │   尝试从 Resources 加载
│   │   ↓
│   │   ┌─ 成功加载? ─────── 是 ──→ [✓ 映射成功] ──→ IsReady = true
│   │   │                              ↓
│   │   │                        按键应该工作
│   │   │
│   │   └─ 否 ──→ 尝试从 PlayerInput 查找
│   │             ↓
│   │             ┌─ 找到? ─────── 是 ──→ [✓ 映射成功]
│   │             │                    ↓
│   │             │              IsReady = true
│   │             │
│   │             └─ 否 ──→ ❌ 初始化失败
│   │                    ↓
│   │                 IsReady = false
│   │                 ↓
│   │              按键无反应
│   │
│   └─ 否 ──→ 已在 Inspector 配置 ──→ [✓ 映射成功] ──→ IsReady = true
│                                      ↓
│                                按键应该工作
```

---

## 🎯 最可能的原因排序

1. **🔴 最可能（99%）：** `Assets/Resources/InputActions.asset` 不存在
   - 解决：从 Intro 中的 InputActions 复制到 Resources 中

2. **🟡 次可能（0.5%）：** Level6 有 PlayerInput 组件但 inputActions 为 null
   - 解决：在 PlayerInput 中配置 InputActions

3. **🟡 可能（0.4%）：** InputActionAsset 中缺少必要的动作
   - 解决：添加 MoveLeft, MoveRight 等动作

4. **🟡 极小概率（0.1%）：** InputSystem 版本问题
   - 解决：更新 Input System 包

---

## 📝 运行时诊断检查清单

```
[] 1. 启动 Level6，打开 Console
[] 2. 看是否有 "[KeymapManager.InitializeInputActions] 开始初始化"
[] 3. 按 A 或 D 键，看是否有 "[PlayerController.HandleMovement]" 日志
[] 4. 如果没有按键日志，说明 IsReady = false
[] 5. 查看是否有 "Resources 加载失败" 或 "PlayerInput 未找到" 的错误
[] 6. 根据错误信息执行上方对应的解决方案
[] 7. 重新保存、编译、运行
[] 8. 验证按键现在是否有反应
```

---

**诊断完成后，请将 Console 的完整日志告诉我！**
