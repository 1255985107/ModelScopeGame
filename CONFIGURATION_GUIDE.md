# ModelScopeGame 配置指南

## 目录
1. [PlayerController 配置](#playercontroller-配置)
2. [Layer 层级配置](#layer-层级配置)
3. [常见问题排查](#常见问题排查)
4. [调试技巧](#调试技巧)

---

## PlayerController 配置

### 什么是 PlayerController？
`PlayerController.cs` 是主要的角色控制脚本，负责：
- 地面检测（Ground Check）
- 移动和跳跃
- 音效播放
- 事件触发

### 必需的配置项

#### 1. **Movement Settings（移动设置）**

| 参数 | 默认值 | 说明 |
|------|------|------|
| `walkSpeed` | 2 | 角色行走速度 |
| `jumpForce` | 5 | 跳跃力度 |
| `fallMultiplier` | 2.5 | 下落加速倍数（使下落更快） |

**如何设置：**
- 选中场景中的 **Player** GameObject
- 在 Inspector 面板找到 `PlayerController` 组件
- 在 "Movement Settings" 部分修改这些值

---

#### 2. **Ground Check（地面检测）⚠️ 关键配置**

| 参数 | 默认值 | 说明 | 重要性 |
|------|------|------|------|
| `groundLayer` | ⚠️ | **指定哪个 Layer 是地面** | 🔴 **最重要** |
| `groundCheckRadius` | 0.2 | 地面检测的圆形范围半径 | 中等 |
| `deathYThreshold` | -10 | Y 坐标低于此值时角色死亡 | 低 |

**⚠️ groundLayer 配置**

这是**最容易出错**的地方！

**步骤 1：打开 Ground Check 部分**
```
Player GameObject
  └─ Inspector
      └─ PlayerController 组件
          └─ Ground Check 部分 👈 点击这里展开
```

**步骤 2：看到 groundLayer 字段**
- 如果是 **灰色/空白** → ❌ **未配置**
- 如果显示数字如 "8" → ✅ 已配置（但可能不正确）

**步骤 3：点击 groundLayer 下拉菜单**
```
┌─────────────────────────────────┐
│ groundLayer ▼                    │
├─────────────────────────────────┤
│ Nothing                          │
│ Everything                       │
│ Default           ✓ 选这个      │
│ TransparentFX                   │
│ Ignore Raycast                  │
│ Water                           │
│ UI                              │
│ Ground            ✓ 或这个      │
│ ...                             │
└─────────────────────────────────┘
```

**选择哪个 Layer？**
- 🟢 **推荐：`Default`** → 如果你的 Tilemap 在 Default Layer
- 🟢 **或选择：`Ground`** → 如果你的 Tilemap 在 Ground Layer

**怎么知道 Tilemap 在哪个 Layer？**
1. 在 Hierarchy 中找到 **Tilemap** GameObject
2. 看 Inspector 右上角的 **Layer** 下拉菜单
3. 它显示的就是 Tilemap 的 Layer
4. PlayerController 的 `groundLayer` 要和它一致！

---

#### 3. **Audio（音效配置）**

| 参数 | 说明 |
|------|------|
| `jumpAudioClip` | 跳跃时的音效 |
| `windAudioClip` | 风声音效 |
| `fallAudioClip` | 下落音效 |
| `runAudioClip` | 奔跑/行走音效 |

**如何配置：**
- 从 `Assets/Sound/` 文件夹拖拽音频文件到对应字段
- 或点击字段旁的圆形按钮，从 Asset Browser 选择

---

#### 4. **Events（事件配置）**

| 事件 | 触发条件 |
|------|---------|
| `onDeath` | 角色死亡时触发 |
| `onSuccessfulJump` | 跳跃超过 2 单位高度时触发 |
| `onInteract` | 角色与可交互物体相交时触发 |

**这些事件可以用来：**
- 触发动画
- 播放音效
- 更新 UI
- 调用其他脚本的方法

---

## Layer 层级配置

### 什么是 Layer？
Layer 是 Unity 中用来分类和过滤 GameObject 的系统。用途：
- 物理碰撞检测（Physics2D）
- 相机渲染掩码
- 射线投射过滤

### 确保 Tilemap 和 PlayerController 的 groundLayer 匹配

**步骤 1：检查 Tilemap 的 Layer**
```
Hierarchy
  └─ Tilemap
      └─ Inspector 右上角
          └─ Layer: [Default] ◄─── 记住这个值
```

**步骤 2：修改 Player 的 groundLayer**
```
Hierarchy
  └─ Player
      └─ Inspector
          └─ PlayerController
              └─ Ground Check
                  └─ groundLayer: [Default] ◄─── 改成和 Tilemap 一样
```

### 创建专用的 Ground Layer（高级配置）

如果你想更清晰地管理，可以创建专用 Layer：

**步骤 1：打开 Layer 管理器**
```
任意 GameObject 的 Inspector
  └─ Layer 下拉菜单
      └─ "Add Layer..." 按钮
```

**步骤 2：添加新 Layer**
```
Layers
  └─ User Layer 8: Ground ◄─── 输入名称
```

**步骤 3：将 Tilemap 分配到这个 Layer**
```
Hierarchy
  └─ Tilemap
      └─ Inspector 右上角
          └─ Layer: Ground
```

**步骤 4：将 PlayerController 的 groundLayer 设置为 Ground**
```
Player
  └─ Inspector
      └─ PlayerController
          └─ Ground Check
              └─ groundLayer: Ground
```

---

## 常见问题排查

### 问题 1：角色卡在下坠动画，无法着陆

**症状：**
- 角色在空中无法落地
- 动画显示一直在下坠
- Console 显示 `isGrounded: False`

**排查步骤：**

1. **检查 groundLayer 配置**
   ```
   Player Inspector
     └─ PlayerController
         └─ Ground Check
             └─ groundLayer: ??? (不能是空白！)
   ```
   
2. **检查 Tilemap 的 Layer**
   ```
   Tilemap Inspector
     └─ Layer: ???
   ```
   
3. **两个 Layer 必须相同！**
   ```
   ✅ 正确：Player groundLayer = "Default" AND Tilemap Layer = "Default"
   ❌ 错误：Player groundLayer = "Ground" BUT Tilemap Layer = "Default"
   ```

4. **启用调试日志**
   - 打开游戏
   - 观察 Console 的输出
   - 看 `AllColliders` 和 `groundLayer` 的值是否匹配

---

### 问题 2：调整 groundCheckRadius 仍无法检测地面

**症状：**
- 虽然 Layer 配置正确，但检测仍然失败
- 角色离 Tilemap 还有距离时无法检测

**解决方案：**

1. **增加 groundCheckRadius**
   ```
   Player Inspector
     └─ PlayerController
         └─ Ground Check
             └─ groundCheckRadius: 0.2 → 改为 0.3 或 0.4
   ```

2. **调整检测点位置**
   - 当前检测点在碰撞箱底部下方 0.1 单位
   - 如果 Tilemap 碰撞箱有厚度，可能需要调整

---

### 问题 3：角色卡在地面上无法跳起

**症状：**
- `isGrounded` 一直为 True
- 无法触发跳跃

**排查步骤：**

1. **检查 Rigidbody2D 的碰撞体**
   ```
   Player GameObject
     └─ Inspector
         └─ Rigidbody2D
             └─ 必须有 Collider2D 组件（如 BoxCollider2D）
   ```

2. **检查物理设置**
   ```
   Edit → Project Settings → Physics2D
     └─ Default Material（应该有一些摩擦力和弹性）
   ```

---

## 调试技巧

### 启用详细日志

当前代码已经包含详细的调试信息。运行游戏时，Console 会输出：

```
[CheckGrounded] isGrounded: False | groundLayer: 8 (京劬绱✧:1000) | CheckPos: (-5.39, -0.58) | CheckRadius: 0.2 | AllColliders: [Player(Layer:Default) Tilemap(Layer:Default)] | Velocity: (0.00, 0.00)

[AnimatorTrigger] grounded=False | h=0 | isWalking=False | isIdle=False | isJumping=True | velocity=(0.00, 0.00)
```

**关键信息：**
- `isGrounded: False` → 地面检测失败
- `groundLayer: 8` → 期望的 Layer 掩码
- `AllColliders: [...]` → 实际检测到的碰撞体及其 Layer
- `isJumping: True` → 动画状态为下坠

**诊断方法：**
- 如果 `AllColliders` 中的 Layer 和 `groundLayer` 不匹配 → Layer 配置错误
- 如果 `AllColliders` 为空 → 检测半径太小或检测点位置错误
- 如果都对但 `isGrounded` 仍为 False → 可能是物理世界碰撞未启用

---

### 在 Scene 视图中可视化检测范围

**步骤：**

1. 编辑 `PlayerController.cs`，在 `CheckGrounded()` 方法添加：

```csharp
private void CheckGrounded()
{
    // ... 现有代码 ...
    
    // 在 Scene 视图显示检测范围
    Debug.DrawWireSphere(checkPosition, groundCheckRadius, isGrounded ? Color.green : Color.red, 0.1f);
}
```

2. 在 Scene 视图（不是 Game 视图）可以看到：
   - 🟢 **绿色圆** → 检测到地面
   - 🔴 **红色圆** → 未检测到地面

---

## 快速参考清单

### 配置 PlayerController 前的检查表

- [ ] Player GameObject 有 `PlayerController` 脚本
- [ ] Player GameObject 有 `Rigidbody2D` 组件
- [ ] Player GameObject 有 `Collider2D` 组件（BoxCollider2D 或 CircleCollider2D）
- [ ] Player GameObject 有 `Animator` 组件（用于动画）
- [ ] Tilemap 对象存在于场景中
- [ ] PlayerController 的 `groundLayer` 和 Tilemap 的 `Layer` 一致
- [ ] `groundCheckRadius` > 0（通常 0.2 就够）
- [ ] 所有音效文件都已分配（可选）
- [ ] 事件监听器已设置（可选）

### 角色卡在下坠时的快速排查

1. ✅ 检查 Console 日志
2. ✅ 确认 `groundLayer` 值
3. ✅ 检查 Tilemap 的 `Layer` 值
4. ✅ 两个值必须相同
5. ✅ 如果不同，改变其中之一使其相同
6. ✅ 重新运行游戏测试

---

## 相关文件

- `Assets/Scripts/Interactive/PlayerController.cs` - 主控制脚本
- `Assets/Scripts/Animator/AnimatorTrigger.cs` - 动画触发脚本
- `Assets/Scenes/` - 游戏场景文件

---

## 联系与反馈

如果遇到其他配置问题，检查以下文件的注释：
- PlayerController.cs 顶部的头部注释
- AnimatorTrigger.cs 中的参数说明

---

**最后更新：** 2025年11月6日
