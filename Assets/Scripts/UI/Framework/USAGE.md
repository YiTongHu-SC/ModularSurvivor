# MVC框架使用说明

## 概述
这是一个专为Unity设计的轻量级MVC框架，集成了现有的事件系统，提供标准化的UI架构模式。

## 快速开始

### 1. 基础MVC组合
```csharp
// 创建模型
var model = new SimpleModel<float>(100f);

// 创建视图（继承BaseView）
var view = gameObject.AddComponent<MyView>();

// 创建控制器（继承BaseController）
var controller = new MyController();
controller.Initialize(model, view);
controller.Start();

// 注册到管理器
MVCManager.Instance.RegisterController(controller);
```

### 2. 使用现成的血量条示例
```csharp
// 方法1：使用HeroHpBarMVC组件（推荐）
var hpBarObject = new GameObject("HeroHealthBar");
var hpBarMVC = hpBarObject.AddComponent<HeroHpBarMVC>();
// 自动处理MVC初始化和生命周期

// 方法2：手动创建
var controller = HeroHealthController.CreateMVC(hpBarObject);
MVCManager.Instance.RegisterController(controller);
```

## 核心组件

### Model - 数据层
**BaseModel<T>**: 所有模型的基类
- 提供数据变更通知
- 支持事件驱动更新
- 自动内存管理

**SimpleModel<T>**: 简单数据模型
```csharp
var healthModel = new SimpleModel<float>(100f);
healthModel.OnValueChanged += (newValue) => Debug.Log($"Health: {newValue}");
healthModel.SetValue(80f); // 触发事件
```

**EventDrivenModel<T, TEvent>**: 集成EventManager的模型
```csharp
public class HealthModel : EventDrivenModel<float, HealthChangedEvent>
{
    protected override HealthChangedEvent CreateEvent(float oldValue, float newValue)
    {
        return new HealthChangedEvent(oldValue, newValue);
    }
}
```

### View - 视图层
**BaseView<T>**: 所有视图的基类
- 自动数据绑定
- Unity生命周期集成
- 可配置更新模式

```csharp
public class HealthBarView : BaseView<float>
{
    [SerializeField] private Image healthBar;
    
    public override void UpdateView(float health)
    {
        healthBar.fillAmount = health / 100f;
    }
}
```

### Controller - 控制层
**BaseController<TModel, TView>**: 所有控制器的基类
- 连接Model和View
- 处理业务逻辑
- 管理生命周期

```csharp
public class HealthController : BaseController<HealthModel, HealthBarView>
{
    protected override void OnInitialize()
    {
        View.BindModel(Model);
    }
    
    public void TakeDamage(float damage)
    {
        var currentHealth = Model.GetValue();
        Model.SetValue(Mathf.Max(0, currentHealth - damage));
    }
}
```

## 最佳实践

### 1. 项目结构
```
UI/
├── Framework/          # MVC框架核心
├── Game/
│   ├── MVC/           # 游戏相关MVC实现
│   │   ├── Models/    # 数据模型
│   │   ├── Views/     # UI视图
│   │   └── Controllers/ # 控制器
│   └── Legacy/        # 传统UI组件（逐步迁移）
```

### 2. 数据流向
```
External Data -> Model -> View -> Display
     ↑                      ↓
Controller <- User Input <- UI Events
```

### 3. 事件处理
```csharp
// 推荐：使用Model的事件系统
Model.OnValueChanged += HandleValueChange;

// 可选：使用全局EventManager
EventManager.Instance.Subscribe<HealthChangedEvent>(OnHealthChanged);
```

### 4. 生命周期管理
```csharp
// 在MonoBehaviour中
private MyController _controller;

void Start()
{
    _controller = new MyController();
    _controller.Initialize(model, view);
    MVCManager.Instance.RegisterController(_controller);
    _controller.Start();
}

void OnDestroy()
{
    MVCManager.Instance.UnregisterController(_controller);
}
```

## 性能考虑

### 1. 更新频率控制
```csharp
// 在Controller中设置更新间隔
public void SetUpdateInterval(float interval)
{
    _updateInterval = interval;
}
```

### 2. 视图优化
```csharp
// 禁用自动更新，手动控制
view.SetAutoUpdate(false);
// 在需要时手动刷新
view.RefreshView();
```

### 3. 批量操作
```csharp
// 批量更新多个属性时，临时禁用事件
model.OnValueChanged -= HandleChange;
model.SetValue(newValue1);
model.SetValue(newValue2);
model.OnValueChanged += HandleChange;
model.SetValue(finalValue); // 只触发一次更新
```

## 调试工具

### 1. 启用调试日志
```csharp
MVCManager.Instance.SetDebugLogging(true);
controller.SetDebugLogging(true);
view.SetDebugLogging(true);
model.SetDebugLogging(true);
```

### 2. 运行时信息
```csharp
// 获取MVC统计信息
Debug.Log(MVCManager.Instance.GetStatistics());

// 检查特定控制器
var healthControllers = MVCManager.Instance.GetControllers<HealthController>();
```

### 3. Editor工具
在Inspector中使用Context Menu:
- "Test Health" - 测试血量显示
- "Show MVC Info" - 显示MVC状态信息

## 迁移指南

### 从传统UI组件迁移到MVC

#### 原始代码（HeroHpBar.cs）
```csharp
public class HeroHpBar : MonoBehaviour
{
    public Image BarImage;
    
    private void Update()
    {
        BarImage.fillAmount = GetHeroHpFraction();
    }
}
```

#### MVC版本
```csharp
// 1. 创建Model
var model = new HeroHealthModel(100f, 100f);

// 2. 转换View
public class HeroHpBarView : BaseView<HeroHealthData>
{
    public override void UpdateView(HeroHealthData data)
    {
        BarImage.fillAmount = data.HealthPercentage;
    }
}

// 3. 创建Controller
var controller = new HeroHealthController();
controller.Initialize(model, view);
```

## 扩展示例

### 自定义模型
```csharp
public class PlayerStatsModel : BaseModel<PlayerStats>
{
    public void LevelUp()
    {
        var stats = Value;
        stats.Level++;
        stats.Experience = 0;
        SetValue(stats);
    }
}
```

### 复合视图
```csharp
public class PlayerStatsView : BaseView<PlayerStats>
{
    [SerializeField] private Text levelText;
    [SerializeField] private Image expBar;
    
    public override void UpdateView(PlayerStats stats)
    {
        levelText.text = $"Level {stats.Level}";
        expBar.fillAmount = stats.Experience / stats.MaxExperience;
    }
}
```

## 故障排除

### 常见问题

1. **视图不更新**
   - 检查Model是否正确绑定到View
   - 确认AutoUpdate是否启用
   - 验证OnValueChanged事件是否正确订阅

2. **内存泄漏**
   - 确保调用Dispose()方法
   - 检查事件订阅是否正确取消
   - 使用MVCManager管理控制器生命周期

3. **性能问题**
   - 调整更新频率
   - 考虑使用对象池
   - 避免在Update中频繁调用SetValue

### 调试技巧
```csharp
// 检查MVC状态
Debug.Log($"Controller Ready: {controller.IsControllerReady()}");
Debug.Log($"View Bound: {view.Model != null}");
Debug.Log($"Model Value: {model.GetValue()}");
```
