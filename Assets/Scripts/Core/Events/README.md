# Events - 事件系统

## 概述
提供全局事件系统，实现模块间的松耦合通信。

## 主要功能
- **事件发布**：任何模块都可以发布事件
- **双重订阅方式**：支持监听器对象和委托两种订阅方式
- **事件类型**：定义所有游戏事件的类型和数据结构
- **生命周期管理**：自动管理事件监听器的注册和注销
- **批量管理**：支持按拥有者批量取消委托订阅

## 核心类设计
- `EventManager`：事件管理器（单例）
- `GameEvents`：游戏事件类型定义
- `IEventListener`：事件监听器接口
- `EventData`：事件数据基类
- `EventListener<T>`：泛型事件监听器基类

## 订阅方式对比

| 特性 | 监听器对象 | 委托订阅 |
|------|------------|----------|
| **适用场景** | 复杂业务逻辑 | 简单响应逻辑 |
| **代码复用** | ✅ 易于复用 | ❌ 难以复用 |
| **类型安全** | ✅ 编译时检查 | ✅ 编译时检查 |
| **生命周期管理** | 手动管理 | 自动批量管理 |
| **内存占用** | 较高 | 较低 |
| **开发效率** | 中等 | ✅ 高效 |

## 常用事件类型
- 单位死亡事件 (`UnitDeathEvent`)
- 波次开始/结束事件 (`WaveStartEvent`, `WaveEndEvent`)
- 玩家升级事件 (`PlayerLevelUpEvent`)
- UI刷新事件 (`UIRefreshEvent`)

## 使用方法

### 1. 发布事件
```csharp
// 发布单位死亡事件
var deathEvent = new GameEvents.UnitDeathEvent("Enemy_001", transform.position, "Player");
EventManager.Instance.PublishEvent(deathEvent);

// 发布玩家升级事件
var levelUpEvent = new GameEvents.PlayerLevelUpEvent(5, 1000);
EventManager.Instance.PublishEvent(levelUpEvent);
```

### 2. 监听器对象订阅（适合复杂逻辑）
```csharp
public class MyUnitDeathListener : EventListener<GameEvents.UnitDeathEvent>
{
    protected override void OnEventReceived(GameEvents.UnitDeathEvent eventData)
    {
        Debug.Log($"Unit {eventData.UnitId} died at {eventData.DeathPosition}");
        // 处理单位死亡逻辑
    }
}

// 注册和注销
var listener = new MyUnitDeathListener();
EventManager.Instance.Subscribe(listener);
EventManager.Instance.Unsubscribe(listener);
```

### 3. 委托订阅（适合简单逻辑）
```csharp
public class GameController : MonoBehaviour
{
    void Start()
    {
        // 直接方法订阅
        EventManager.Instance.Subscribe<GameEvents.PlayerLevelUpEvent>(OnPlayerLevelUp, this);
        
        // Lambda表达式订阅
        EventManager.Instance.Subscribe<GameEvents.UnitDeathEvent>(eventData => {
            Debug.Log($"Enemy {eventData.UnitId} was defeated!");
            UpdateKillCounter();
        }, this);
        
        // 复杂匿名方法订阅
        EventManager.Instance.Subscribe<GameEvents.WaveEndEvent>(eventData => {
            if (eventData.IsSuccess)
            {
                ShowVictoryUI();
                PlayVictorySound();
            }
            else
            {
                ShowDefeatUI();
            }
        }, this);
    }
    
    private void OnPlayerLevelUp(GameEvents.PlayerLevelUpEvent eventData)
    {
        playerLevelText.text = $"Level {eventData.NewLevel}";
        ShowLevelUpEffect();
    }
    
    void OnDestroy()
    {
        // 批量注销所有委托订阅
        EventManager.Instance.UnsubscribeAll(this);
    }
}
```

### 4. 混合使用两种订阅方式
```csharp
public class CombatSystem : MonoBehaviour
{
    private UnitDeathListener deathListener; // 复杂逻辑用监听器对象
    
    void Start()
    {
        // 监听器对象 - 处理复杂的单位死亡逻辑
        deathListener = new UnitDeathListener();
        EventManager.Instance.Subscribe(deathListener);
        
        // 委托订阅 - 简单的UI更新
        EventManager.Instance.Subscribe<GameEvents.PlayerLevelUpEvent>(eventData => {
            UpdatePlayerLevelUI(eventData.NewLevel);
        }, this);
    }
}
```

### 5. 性能配置
```csharp
public class GameManager : MonoBehaviour
{
    void Start()
    {
        // 生产环境关闭详细日志以提高性能
        EventManager.Instance.SetVerboseLogging(false);
        
        // 极限性能场景（如大量AI计算）可启用高性能模式
        // 注意：高性能模式会关闭异常日志，仅在性能关键场景使用
        if (isPerformanceCritical)
        {
            EventManager.Instance.SetHighPerformanceMode(true);
        }
        
        // 开发环境可以开启详细日志用于调试
        #if UNITY_EDITOR
        EventManager.Instance.SetVerboseLogging(true);
        #endif
    }
}
```

### 6. 自定义事件类型
```csharp
// 在GameEvents类中添加新的事件类型
public class CustomEvent : EventData
{
    public string Message { get; }
    
    public CustomEvent(string message)
    {
        Message = message;
    }
}
```

## 性能特性
- **零GC模式**：发布事件时不产生额外的内存分配
- **高吞吐量**：标准模式可处理10000+事件投递/100ms
- **超高性能模式**：关闭异常日志，可处理10000+事件投递/50ms
- **可配置日志**：生产环境可关闭详细日志提升性能
- **直接遍历**：避免创建临时集合，减少GC压力
- **分层优化**：正常模式保留异常安全，高性能模式追求极致速度

## 测试
事件系统包含完整的单元测试和集成测试，位于 `Assets/Tests/Core/` 目录：

### 测试文件
- `EventSystemTests.cs` - 基础单元测试
- `EventSystemIntegrationTests.cs` - 集成测试和性能测试
- `EventSystemPlayModeDemo.cs` - PlayMode演示测试
- `TestEventListeners.cs` - 测试用监听器

### 运行测试
1. 打开Unity Test Runner窗口 (Window → General → Test Runner)
2. 选择EditMode或PlayMode标签页
3. 运行相应的测试用例

### 测试覆盖
- ✅ 事件发布和订阅
- ✅ 监听器注册和注销
- ✅ 多监听器同时处理
- ✅ 异常处理
- ✅ 性能测试（压力测试）
- ✅ 边界情况处理
- ✅ 完整场景演示

## 注意事项
- 事件管理器使用单例模式，确保在使用前已初始化
- **监听器对象**必须在对象销毁时手动注销，避免内存泄漏
- **委托订阅**建议提供拥有者参数，可使用`UnsubscribeAll(owner)`批量清理
- 事件处理过程中的异常会被捕获并记录，不会影响其他监听器
- 事件数据包含时间戳，便于调试和分析
- **性能建议**：简单逻辑优先使用委托订阅，复杂逻辑使用监听器对象
- 同一事件可以同时有监听器对象和委托订阅者，两者互不干扰
