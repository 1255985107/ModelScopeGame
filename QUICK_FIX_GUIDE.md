# PlayerController 快速排查流程图

## 症状：角色卡在下坠动画

```
┌─────────────────────────────────────┐
│  游戏运行中，角色无法着陆          │
│  Console 显示：isGrounded: False    │
└────────────────┬────────────────────┘
                 │
                 ▼
        ┌─────────────────┐
        │ 打开 Console   │
        │ 观察日志输出   │
        └────────┬────────┘
                 │
                 ▼
    ┌──────────────────────────┐
    │ 寻找这一行日志：         │
    │ [CheckGrounded]          │
    │ groundLayer: 8           │
    │ AllColliders:            │
    │ [Player Tilemap...]      │
    └──────────┬───────────────┘
               │
               ▼
    ┌───────────────────────────────────┐
    │ 对比两个值：                       │
    │                                   │
    │ groundLayer: 8 (二进制:1000)      │
    │             ▼▼▼                    │
    │        第 4 位的 Layer             │
    │                                   │
    │ AllColliders: Tilemap(Layer:???) │
    │                            ▼▼▼    │
    │                  Tilemap 在哪个 Layer
    └──────────┬──────────────────────────┘
               │
       ┌───────┴────────┐
       │                │
    相同 ✅          不相同 ❌
       │                │
       ▼                ▼
    ┌──────────┐    ┌───────────────────────────┐
    │ 检测正常 │    │ Layer 不匹配！            │
    │ 问题在  │    │ 需要修改其中之一          │
    │ 其他地方 │    │                           │
    └──────────┘    └─────┬───────────────────────┘
                          │
                ┌─────────┴──────────┐
                │                    │
        选择 A               选择 B
        (改 groundLayer)    (改 Tilemap Layer)
            │                   │
            ▼                   ▼
        ┌──────────────┐   ┌──────────────┐
        │ Player       │   │ Tilemap      │
        │ Inspector    │   │ Inspector    │
        │ ↓            │   │ ↓            │
        │ PlayerCon... │   │ Layer ▼      │
        │ ↓            │   │              │
        │ Ground Check │   │ 改成 Ground  │
        │ ↓            │   │ 或 Default   │
        │ groundLayer  │   │ (与 Player   │
        │  改为 Tilemap│   │  的设置一致) │
        │  的 Layer    │   └──────────────┘
        └──────────────┘
```

---

## 快速修复步骤

### 方法 1：查看 Console 日志（推荐）

**第 1 步：运行游戏**
- 按下 Play 按钮

**第 2 步：打开 Console**
```
菜单栏：Window → General → Console
或快捷键：Ctrl + Shift + C
```

**第 3 步：看日志的关键部分**
```
[CheckGrounded] isGrounded: False | groundLayer: 8 | AllColliders: [Player(Layer:Default) Tilemap(Layer:Default)]
                                                      ▲ 这个值                           ▲ 这个值
                                                      │                                  │
                                                      └──────────────────────────────────┘
                                                              必须相等！
```

**第 4 步：决定修改哪个**

| 如果 groundLayer 是 | Tilemap Layer 是 | 修改方案 |
|-----------------|---|---------|
| 8 | Default | 改 Player 的 groundLayer 为 Default |
| 8 | Ground | 改 Player 的 groundLayer 为 Ground |
| Default | Ground | 改 Tilemap Layer 为 Default |
| Ground | Default | 改 Player 的 groundLayer 为 Ground |

---

### 方法 2：手动逐一检查（如果没看懂 Console）

**检查清单：**

#### ✅ 第 1 步：Hierarchy 中找到 Tilemap

```
Hierarchy 面板
  ├─ Canvas
  ├─ Tilemap  ◄─── 找到它
  ├─ Player
  └─ ...
```

#### ✅ 第 2 步：点击 Tilemap，看它的 Layer

```
Inspector 面板（点击 Tilemap 时显示）
┌────────────────────────────────────┐
│ Tilemap                            │
├────────────────────────────────────┤
│ Tag   [Untagged] ▼                 │
│ Layer [Default] ◄─── 看这里         │
│ ...                                │
└────────────────────────────────────┘
```

**记住 Tilemap 的 Layer 值**（如 `Default`）

#### ✅ 第 3 步：点击 Player，看 groundLayer

```
Hierarchy 面板
  └─ Player ◄─── 点击这个

Inspector 面板
┌─────────────────────────────────┐
│ Player                          │
├─────────────────────────────────┤
│ PlayerController (Script)       │
│ ▼ Movement Settings             │
│   ...                           │
│ ▼ Ground Check                  │
│   groundLayer    [?????] ◄─── 看这里
│   groundCheckRadius 0.2         │
│   deathYThreshold -10           │
│   ...                           │
└─────────────────────────────────┘
```

**比较两个值：**
```
Tilemap Layer:          groundLayer:
Default                 Default     ✅ 相同，不需要改
或
Ground                  Default     ❌ 不相同，需要改!
```

#### ✅ 第 4 步：如果不相同，修改其中一个

**选择 A：改 Player 的 groundLayer**

```
Player Inspector
  └─ PlayerController
      └─ Ground Check
          └─ groundLayer ▼ 点击这个下拉菜单
```

选择 Tilemap 的 Layer 值

**或选择 B：改 Tilemap 的 Layer**

```
Tilemap Inspector
  └─ Layer [Default] ▼
      选择与 Player groundLayer 相同的值
```

#### ✅ 第 5 步：重新运行游戏，测试

```
Press Play
  │
  └─► 角色能着陆吗？
      ├─ 能 ✅ 问题解决！
      └─ 不能 ❌ 检查是否有其他配置错误
```

---

## 其他常见问题

### Q: groundLayer 显示的是数字（如 8），不是名字（如 Default）

**A:** 这正常。Unity 内部使用数字表示 Layer。

- `8` = 第 3 位（从 0 开始数）
- `1` = 第 0 位
- `2` = 第 1 位
- `4` = 第 2 位
- 等等...

但 Tilemap 显示的是名字（如 `Default`）。

**只要两个指向同一个 Layer 就行。** 如果不确定，都改成 `Default`。

### Q: 修改后仍然不行？

**检查以下项目：**

1. ✅ **Rigidbody2D 是否已启用？**
   ```
   Player Inspector
     └─ Rigidbody2D
         └─ ☑ Enabled (前面有勾)
   ```

2. ✅ **Player 有 Collider2D 吗？**
   ```
   Player Inspector
     └─ BoxCollider2D（或其他 Collider）
       └─ ☑ Enabled
   ```

3. ✅ **groundCheckRadius 是否太小？**
   ```
   改成 0.3 或更大试试
   ```

4. ✅ **是否曾经改过 Layer 后没有保存场景？**
   ```
   Ctrl + S 保存
   ```

---

## 一键诊断（复制粘贴）

如果上面的步骤都不明白，试试这个：

**第 1 步：打开 Console**
- Window → General → Console

**第 2 步：看输出的日志，复制这一行：**
```
[CheckGrounded] isGrounded: False | groundLayer: 8 | AllColliders: [Player(Layer:Default) Tilemap(Layer:Default)]
```

**第 3 步：查看：**
- `groundLayer: 8` → Player 期望的值
- `Tilemap(Layer:Default)` → Tilemap 实际的值

**第 4 步：不相同就改，改成一样的**

---

## 视觉化排查（在 Scene 视图看）

如果需要在 Scene 视图中看到检测范围：

**编辑 PlayerController.cs：**

在 `CheckGrounded()` 方法最后添加：

```csharp
// 可视化调试
Debug.DrawWireSphere(checkPosition, groundCheckRadius, 
    isGrounded ? Color.green : Color.red, 0.05f);
```

然后：
- 打开 Scene 视图（不是 Game 视图）
- 运行游戏
- 能看到角色下方有个圆形框
  - 🟢 **绿色** = 检测到地面
  - 🔴 **红色** = 未检测到地面

---

**记住：Layer 必须匹配！这是 99% 问题的原因。**
