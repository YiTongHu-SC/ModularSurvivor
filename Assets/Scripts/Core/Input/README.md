# Input - 输入系统

统一管理所有玩家输入，支持键盘、手柄、移动端等多平台输入。

## 🚀 快速开始

### 基本使用

```csharp
// 1. 查询移动输入（每帧）
Vector2 move = InputManager.Instance.GetMoveDirection();

// 2. 订阅攻击事件（响应式）
EventManager.Instance.Subscribe<InputEvents.PlayerAttackInputEvent>(
    e => { if (e.IsPressed) Debug.Log("攻击！"); }, 
    this
);

// 3. 控制输入状态
InputManager.Instance.SetPausedContext();      // 暂停（打开菜单时）
InputManager.Instance.EnableGameplayInput();   // 恢复游戏输入
```

## 📋 支持的输入

| 动作 | 键盘 | 手柄 | 代码 |
|-----|------|------|------|
| **移动** | WASD/方向键 | 左摇杆 | `GetMoveDirection()` |
| **视角** | 鼠标 | 右摇杆 | `GetLookDelta()` |
| **攻击** | 鼠标左键 | X/Square | 订阅 `PlayerAttackInputEvent` |
| **冲刺** | Shift | L3 | `IsSprintPressed()` |
| **跳跃** | Space | A/Cross | 订阅 `PlayerJumpInputEvent` |
| **蹲伏** | Ctrl | B/Circle | `IsCrouchPressed()` |
| **交互** | E | Y/Triangle | 订阅 `PlayerInteractInputEvent` |
| **切换** | Q/R | L1/R1 | 订阅 `PlayerPrevious/NextInputEvent` |

## 🎯 使用模式

### 连续输入 → 状态查询
```csharp
void Update()
{
    Vector2 move = InputManager.Instance.GetMoveDirection();
    bool sprinting = InputManager.Instance.IsSprintPressed();
    
    // 应用移动逻辑
    if (move.magnitude > 0.1f)
    {
        float speed = sprinting ? 10f : 5f;
        transform.Translate(new Vector3(move.x, 0, move.y) * speed * Time.deltaTime);
    }
}
```

### 一次性输入 → 事件订阅
```csharp
void Start()
{
    // 攻击输入
    EventManager.Instance.Subscribe<InputEvents.PlayerAttackInputEvent>(
        e => { if (e.IsPressed) Attack(); }, this);
    
    // 跳跃输入
    EventManager.Instance.Subscribe<InputEvents.PlayerJumpInputEvent>(
        e => Jump(), this);
    
    // 交互输入（支持长按）
    EventManager.Instance.Subscribe<InputEvents.PlayerInteractInputEvent>(
        e => {
            if (e.Phase == InputEvents.InteractionPhase.Performed)
                Debug.Log($"交互完成，持续 {e.Duration} 秒");
        }, this);
}

void OnDestroy()
{
    EventManager.Instance.UnsubscribeAll(this);  // 重要：清理订阅
}
```

## ⚙️ 输入控制

```csharp
// 游戏状态控制
InputManager.Instance.EnableGameplayInput();   // 启用游戏输入
InputManager.Instance.DisableGameplayInput();  // 禁用游戏输入
InputManager.Instance.SetPausedContext();      // 暂停状态（禁用游戏，启用UI）
InputManager.Instance.DisableAllInput();       // 禁用所有输入（过场动画）

// UI 集成示例
public class PauseMenu : MonoBehaviour
{
    void OnEnable() => InputManager.Instance.SetPausedContext();
    void OnDisable() => InputManager.Instance.EnableGameplayInput();
}
```

## 📝 API 参考

### 状态查询
```csharp
Vector2 InputManager.Instance.GetMoveDirection()     // 移动方向（归一化）
Vector2 InputManager.Instance.GetRawMoveInput()      // 原始移动输入
Vector2 InputManager.Instance.GetLookDelta()         // 视角变化增量
bool InputManager.Instance.IsAttackPressed()         // 攻击键状态
bool InputManager.Instance.IsSprintPressed()         // 冲刺键状态
bool InputManager.Instance.IsCrouchPressed()         // 蹲伏键状态
```

### 事件类型
- `PlayerMoveInputEvent` - 移动输入（Vector2 MoveDirection, Vector2 RawInput）
- `PlayerLookInputEvent` - 视角输入（Vector2 LookDelta）
- `PlayerAttackInputEvent` - 攻击输入（bool IsPressed）
- `PlayerSprintInputEvent` - 冲刺输入（bool IsPressed）
- `PlayerJumpInputEvent` - 跳跃输入
- `PlayerCrouchInputEvent` - 蹲伏输入（bool IsPressed）
- `PlayerInteractInputEvent` - 交互输入（InteractionPhase Phase, float Duration）
- `PlayerPreviousInputEvent` - 切换到上一个
- `PlayerNextInputEvent` - 切换到下一个
- `InputContextChangedEvent` - 输入上下文切换（InputContext Context）

### 输入上下文
- `Gameplay` - 正常游戏
- `UI` - UI 交互模式
- `Paused` - 暂停状态
- `Disabled` - 全部禁用

## ⚡ 快速参考

### 常用代码片段

#### 基础移动控制
```csharp
using Core.Input;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    
    void Update()
    {
        Vector2 input = InputManager.Instance.GetMoveDirection();
        Vector3 movement = new Vector3(input.x, 0, input.y);
        transform.Translate(movement * speed * Time.deltaTime);
    }
}
```

#### 攻击事件监听
```csharp
using Core.Events;
using Core.Input;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    void Start()
    {
        EventManager.Instance.Subscribe<InputEvents.PlayerAttackInputEvent>(
            OnAttack, this);
    }
    
    void OnAttack(InputEvents.PlayerAttackInputEvent e)
    {
        if (e.IsPressed) Debug.Log("攻击！");
    }
    
    void OnDestroy()
    {
        EventManager.Instance.UnsubscribeAll(this);
    }
}
```

#### 输入状态控制
```csharp
using Core.Input;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("测试输入控制")]
    public KeyCode pauseKey = KeyCode.P;
    public KeyCode resumeKey = KeyCode.O;
    
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            InputManager.Instance.SetPausedContext();
            Debug.Log("游戏暂停，输入已禁用");
        }
        
        if (Input.GetKeyDown(resumeKey))
        {
            InputManager.Instance.EnableGameplayInput();
            Debug.Log("游戏恢复，输入已启用");
        }
    }
}
```

### 使用场景对比

| 场景 | 推荐方式 | 原因 | 代码示例 |
|-----|---------|------|----------|
| **连续移动** | ✅ 状态查询 | 需要每帧检查 | `GetMoveDirection()` |
| **按钮点击** | ✅ 事件订阅 | 响应式，性能好 | 订阅 `PlayerAttackInputEvent` |
| **UI响应** | ✅ 事件订阅 | 避免轮询 | 订阅相关输入事件 |
| **摄像机控制** | ✅ 状态查询 | 需要平滑处理 | `GetLookDelta()` |
| **技能释放** | ✅ 事件订阅 | 即时响应 | 订阅对应按键事件 |

### 常见问题速查

| 问题 | 解决方案 | 代码 |
|-----|---------|------|
| **移动没反应** | 检查输入是否启用 | `InputManager.Instance.EnableGameplayInput()` |
| **事件没触发** | 检查是否正确订阅 | `EventManager.Instance.Subscribe<T>(callback, this)` |
| **忘记清理订阅** | 在 OnDestroy 中清理 | `EventManager.Instance.UnsubscribeAll(this)` |
| **菜单时仍能移动** | 切换到暂停上下文 | `InputManager.Instance.SetPausedContext()` |
| **InputManager 为 null** | 确保 GameManager 已初始化 | 检查场景中是否有 GameManager |

### 调试技巧

#### 实时监控输入
```csharp
void Update()
{
    // 监控移动输入
    Vector2 move = InputManager.Instance.GetMoveDirection();
    if (move.magnitude > 0.1f)
        Debug.Log($"移动: {move}");
    
    // 监控按键状态
    if (InputManager.Instance.IsAttackPressed())
        Debug.Log("攻击键按下");
    
    // 监控输入上下文
    Debug.Log($"当前上下文: {InputManager.Instance.GetCurrentContext()}");
}
```

#### 输入上下文切换监听
```csharp
void Start()
{
    EventManager.Instance.Subscribe<InputEvents.InputContextChangedEvent>(
        e => Debug.Log($"输入上下文变更: {e.Context}"), 
        this);
}
```

## 🏗️ 架构设计

```
Unity InputSystem → InputManager → 事件发布/状态缓存 → 游戏系统使用
```

- **InputManager**：单例 MonoBehaviour，继承 `BaseInstance<InputManager>`
- **混合模式**：事件驱动（响应式）+ 状态查询（连续输入）
- **松耦合**：通过 `EventManager` 与其他系统通信
- **自动初始化**：在 `GameManager` 中自动创建和初始化

## 💡 最佳实践

### ✅ 推荐做法
- 移动控制用状态查询：`GetMoveDirection()`
- 按钮响应用事件订阅：攻击、跳跃、交互
- 订阅事件时传入 `owner`：`Subscribe(callback, this)`
- 在 `OnDestroy` 中清理：`UnsubscribeAll(this)`
- UI 打开时禁用游戏输入：`SetPausedContext()`

### ❌ 避免做法
- 不要用事件处理连续输入（性能差）
- 不要在事件回调中做重计算
- 不要忘记取消事件订阅（内存泄漏）

## 🧪 测试

### 快速测试
1. 运行游戏，检查 Console：`[InputManager] Gameplay input enabled.`
2. 按 WASD 测试移动：`Debug.Log(InputManager.Instance.GetMoveDirection())`
3. 按鼠标左键测试攻击事件

### 集成测试
参考 `InputManagerExample.cs` 获取完整测试示例。

## 🔧 集成示例

### 与 MovementSystem 集成

```csharp
// 方案A：事件驱动（响应式）
public class MovementSystem
{
    void Initialize()
    {
        EventManager.Instance.Subscribe<InputEvents.PlayerMoveInputEvent>(
            OnPlayerMoveInput, this);
    }
    
    void OnPlayerMoveInput(InputEvents.PlayerMoveInputEvent e)
    {
        if (heroUnit != null)
        {
            heroUnit.TargetVelocity = e.MoveDirection * heroUnit.MoveSpeed;
        }
    }
}

// 方案B：状态查询（推荐，适合连续移动）
public class MovementSystem
{
    void UpdateMovement(float deltaTime)
    {
        if (heroUnit != null)
        {
            Vector2 input = InputManager.Instance.GetMoveDirection();
            heroUnit.Position += input * heroUnit.MoveSpeed * deltaTime;
        }
    }
}
```

### 与 AbilitySystem 集成

```csharp
public class AbilitySystem
{
    void Initialize()
    {
        // 攻击输入
        EventManager.Instance.Subscribe<InputEvents.PlayerAttackInputEvent>(
            e => {
                if (e.IsPressed && heroUnit != null)
                    TriggerAbility(heroUnit, "PrimaryAttack");
            }, this);
        
        // 技能切换
        EventManager.Instance.Subscribe<InputEvents.PlayerNextInputEvent>(
            e => SwitchToNextAbility(), this);
        
        EventManager.Instance.Subscribe<InputEvents.PlayerPreviousInputEvent>(
            e => SwitchToPreviousAbility(), this);
    }
}
```

### UI 系统集成

```csharp
// 暂停菜单
public class PauseMenu : MonoBehaviour
{
    void OnEnable()
    {
        InputManager.Instance.SetPausedContext();
        Debug.Log("暂停菜单打开，游戏输入已禁用");
    }
    
    void OnDisable()
    {
        InputManager.Instance.EnableGameplayInput();
        Debug.Log("暂停菜单关闭，游戏输入已恢复");
    }
}

// 设置菜单
public class SettingsMenu : MonoBehaviour
{
    void OnEnable()
    {
        InputManager.Instance.DisableGameplayInput();
        InputManager.Instance.EnableUIInput();
    }
    
    void OnDisable()
    {
        InputManager.Instance.EnableGameplayInput();
    }
}

// 过场动画控制器
public class CutsceneController : MonoBehaviour
{
    void StartCutscene()
    {
        InputManager.Instance.DisableAllInput();
        Debug.Log("过场动画开始，所有输入已禁用");
    }
    
    void EndCutscene()
    {
        InputManager.Instance.EnableGameplayInput();
        Debug.Log("过场动画结束，输入已恢复");
    }
}
```

## 🔧 故障排除

**问题：InputManager.Instance 为 null**
- 确保 GameManager 在场景中且已初始化

**问题：输入没有响应**
- 检查输入上下文：`InputManager.Instance.GetCurrentContext()`
- 确认输入已启用：`EnableGameplayInput()`

**问题：编译错误 "Cannot resolve symbol 'InputSystem_Actions'"**
- `InputSystem_Actions.cs` 应在 `Core/Input/` 目录中
- 如果在 Assets 根目录，手动移动到此处

## ⚡ 性能优化

### 输入轮询 vs 事件驱动

```csharp
// ❌ 避免：在 Update 中轮询所有输入
void Update()
{
    // 每帧检查所有输入，即使没有按下
    if (InputManager.Instance.IsAttackPressed()) { /* ... */ }
    if (InputManager.Instance.IsSprintPressed()) { /* ... */ }
    if (InputManager.Instance.IsCrouchPressed()) { /* ... */ }
    // ... 更多检查
}

// ✅ 推荐：事件驱动 + 必要的状态查询
void Start()
{
    // 一次性设置事件监听
    EventManager.Instance.Subscribe<InputEvents.PlayerAttackInputEvent>(OnAttack, this);
    EventManager.Instance.Subscribe<InputEvents.PlayerJumpInputEvent>(OnJump, this);
}

void Update()
{
    // 仅查询需要连续处理的输入
    Vector2 move = InputManager.Instance.GetMoveDirection();
    if (move.magnitude > 0.1f) // 只在实际移动时处理
    {
        ApplyMovement(move);
    }
}
```

### 内存管理

```csharp
// ✅ 正确的生命周期管理
public class PlayerController : MonoBehaviour
{
    void Start()
    {
        // 订阅时传入 owner
        EventManager.Instance.Subscribe<InputEvents.PlayerAttackInputEvent>(OnAttack, this);
    }
    
    void OnDestroy()
    {
        // 批量清理，防止内存泄漏
        EventManager.Instance.UnsubscribeAll(this);
    }
    
    // ❌ 避免在事件回调中创建 GC
    void OnAttack(InputEvents.PlayerAttackInputEvent e)
    {
        // 避免：var message = $"Attack: {e.IsPressed}";
        // 避免：new List<>(), LINQ 操作等
        
        // ✅ 推荐：简单的逻辑处理
        isAttacking = e.IsPressed;
    }
}
```

## 🔗 扩展

### 添加新输入动作
1. 在 `InputSystem_Actions.inputactions` 中添加动作
2. 在 `InputEvents` 中定义事件类型
3. 在 `InputManager` 中添加回调和注册
4. 重新生成 C# 类

### 移动端支持
- 架构已支持
- 需在场景中添加 Unity InputSystem 的虚拟摇杆组件

#### 移动端实现步骤

1. **添加虚拟摇杆**
```csharp
// 在 Canvas 中添加 On-Screen Stick
// 1. 右键 UI Canvas > UI > On-Screen Stick
// 2. 设置 Control Path 为 "<Gamepad>/leftStick"
// 3. 或直接绑定到 "Move" 动作
```

2. **添加虚拟按钮**
```csharp
// 添加攻击按钮
// 1. 右键 UI Canvas > UI > On-Screen Button  
// 2. 设置 Control Path 为 "<Gamepad>/buttonWest"
// 3. 或绑定到 "Attack" 动作
```

3. **移动端输入检测**
```csharp
using UnityEngine;
using Core.Input;

public class MobileInputHandler : MonoBehaviour
{
    void Start()
    {
        // 检测移动端并启用虚拟控件
        if (Application.isMobilePlatform)
        {
            EnableMobileControls();
        }
    }
    
    void EnableMobileControls()
    {
        // 显示虚拟摇杆和按钮
        GameObject.Find("VirtualJoystick").SetActive(true);
        GameObject.Find("AttackButton").SetActive(true);
        
        Debug.Log("移动端控件已启用");
    }
}
```

4. **响应式 UI 布局**
```csharp
// 根据屏幕尺寸调整虚拟控件位置
[System.Serializable]
public class MobileUILayout
{
    public RectTransform joystick;
    public RectTransform attackButton;
    
    void AdjustForScreenSize()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        
        if (screenRatio > 1.7f) // 宽屏设备
        {
            joystick.anchoredPosition = new Vector2(-200, -150);
            attackButton.anchoredPosition = new Vector2(200, -150);
        }
        else // 传统 4:3 设备
        {
            joystick.anchoredPosition = new Vector2(-150, -100);
            attackButton.anchoredPosition = new Vector2(150, -100);
        }
    }
}
```

---

**InputSystem 已就绪，开始使用吧！** 🎮

