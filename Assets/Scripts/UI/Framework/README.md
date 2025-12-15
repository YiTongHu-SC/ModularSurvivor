# Unity MVC UI Framework

## 概述

这是一个为Unity设计的轻量级MVC UI框架，提供了完整的UI管理解决方案。框架采用渐进式开发思路，当前包含基础MVC架构和高级UI管理功能。

### 主要特性
- ✅ **传统MVC架构** - 完整的Model-View-Controller分离
- ✅ **UI栈管理** - 支持UI堆叠、层级路由和栈式返回逻辑  
- ✅ **生命周期管理** - Open/Close/CloseTop等统一API
- ✅ **输入阻塞控制** - Popup/Loading自动阻塞底层输入
- ✅ **事件驱动架构** - 基于现有EventManager的松耦合通信
- ✅ **自动数据绑定** - View自动订阅Model变更事件

## 🏗️ 架构设计

```
MVCManager (单例)
├── 传统MVC组件
│   ├── Model-View-Controller 架构
│   ├── 事件驱动数据绑定
│   └── 生命周期管理
└── UI栈管理系统 (新增)
    ├── UIStack - 界面栈管理
    ├── UILayer - 层级路由系统
    ├── BaseUIController - UI控制器基类
    └── 输入阻塞控制
```

## 📁 框架结构

### 核心接口
- **IModel<T>** - 数据模型接口，定义数据访问规范
- **IView<T>** - 视图接口，定义UI更新规范  
- **IController<TModel, TView>** - 控制器接口，定义控制逻辑规范
- **IUIController** - UI控制器接口，扩展UI管理功能

### 传统MVC组件
- **BaseModel<T>** - 数据模型基类，集成EventManager事件通知
- **BaseView<T>** - UI视图基类，自动处理数据绑定和事件监听
- **BaseController<TModel, TView>** - 控制器基类，处理业务逻辑和用户交互

### UI管理系统 (新增)
- **BaseUIController<TModel, TView>** - UI控制器基类，支持栈管理
- **UIStack** - UI栈管理器，处理界面堆叠和返回逻辑
- **UILayer** - 层级定义：Background(0) → HUD(100) → Window(200) → Popup(300) → Loading(400) → System(500)
- **UILayerAttribute** - 层级配置属性

### 扩展组件
- **SimpleModel<T>** - 简单数据模型，适用于基础数据类型
- **EventDrivenModel<T, TEvent>** - 事件驱动模型，集成EventManager
- **ModelVariants** - 模型变体和工厂方法

### 管理系统
- **MVCManager** - 统一管理器，管理传统MVC和UI栈
- **演示组件** - 完整的示例和测试工具

## 🎯 核心功能

### 1. UI生命周期管理
```csharp
// 打开UI
MVCManager.Instance.Open<MainMenuUIController>(openArgs);

// 关闭特定UI
MVCManager.Instance.Close<MainMenuUIController>();

// 关闭栈顶UI
MVCManager.Instance.CloseTop();

// 状态查询
bool isOpen = MVCManager.Instance.IsUIOpen<MainMenuUIController>();
bool hasBlocker = MVCManager.Instance.HasInputBlocker();
```

### 2. 界面栈管理
- 支持UI堆叠显示，Window/Popup可以同时存在
- LIFO栈式返回逻辑
- 层级分组管理和输入阻塞检测

### 3. 层级路由系统
- 自动将UI挂载到正确的显示层级
- 6个预定义层级，支持自定义层级
- 通过`UILayerAttribute`配置UI层级属性

### 4. 输入阻塞控制
- Popup/Loading等UI可以阻塞底层输入
- 自动管理输入穿透逻辑
- 实时阻塞状态查询

## 🚀 使用指南

### 方式一：传统MVC模式

#### 1. 创建MVC组件
```csharp
// Model - 血量数据模型
var model = new HeroHealthModel(100f, 100f);
model.SetCurrentHealth(80f); // 自动触发UI更新

// View - 血量条视图
public class HeroHpBarView : BaseView<HeroHealthData>
{
    public override void UpdateView(HeroHealthData data)
    {
        barImage.fillAmount = data.HealthPercentage;
    }
}

// Controller - 血量控制器
var controller = HeroHealthController.CreateMVC(gameObject);
MVCManager.Instance.RegisterController(controller);
```

#### 2. 快速使用
```csharp
// 简单MVC组合
var model = new SimpleModel<float>(100f);
var view = gameObject.AddComponent<MyView>();
var controller = new MyController();
controller.Initialize(model, view);
MVCManager.Instance.RegisterController(controller);

// 使用现成的血量条示例
var hpBar = gameObject.AddComponent<HeroHpBarMVC>();
```

### 方式二：UI栈管理模式 (推荐)

#### 1. 创建UI控制器
```csharp
// 使用UILayerAttribute配置UI属性
[UILayer(UILayer.Window, blockInput: false, allowStack: false)]
public class MainMenuUIController : BaseUIController<MainMenuModel, MainMenuView>
{
    protected override void OnAfterOpen(object args)
    {
        // UI打开后的逻辑
    }

    protected override void OnBeforeClose()
    {
        // UI关闭前的逻辑
    }
}
```

#### 2. UI层级配置示例
```csharp
[UILayer(UILayer.HUD, blockInput: false, allowStack: true)]
public class GameHUDController : BaseUIController<HUDModel, HUDView> { }

[UILayer(UILayer.Popup, blockInput: true, allowStack: true)]
public class ConfirmDialogController : BaseUIController<DialogModel, DialogView> { }

[UILayer(UILayer.Loading, blockInput: true, allowStack: false)]
public class LoadingController : BaseUIController<LoadingModel, LoadingView> { }
```

#### 3. 打开参数传递
```csharp
// 定义打开参数
public class MainMenuOpenArgs
{
    public int UserLevel { get; set; }
    public string FromScene { get; set; }
}

// 传递参数打开UI
var args = new MainMenuOpenArgs { UserLevel = 15, FromScene = "Game" };
MVCManager.Instance.Open<MainMenuUIController>(args);
```

## ✨ 核心特性详解

### 🔗 事件驱动架构
- 利用现有EventManager实现松耦合通信
- Model变更自动触发View更新
- 支持全局事件广播和本地事件监听

### 🎯 自动数据绑定
- View自动订阅Model变更事件
- 支持可配置的更新模式（自动/手动）
- 内存安全的事件订阅/取消订阅

### ⚡ 性能优化
- 可配置的更新频率控制
- 智能的变更检测，避免无意义更新
- 支持批量操作和延迟更新
- UI栈操作为O(n)复杂度，适合中小型UI数量

### 🛠️ 开发友好
- 完整的生命周期管理
- 丰富的调试工具和日志系统
- Context Menu支持，便于Inspector调试
- 键盘快捷键测试：M(主菜单) S(设置) ESC(关闭) I(信息)

## 🧪 测试和演示

### 演示脚本
```csharp
// 添加框架演示到场景
var demo = gameObject.AddComponent<MVCFrameworkDemo>();
var usageExample = gameObject.AddComponent<MVCFrameworkUsageExample>();
var testScene = gameObject.AddComponent<MVCTestScene>();
```

### 示例UI控制器
框架包含完整的示例实现：
- **MainMenuUIController** - 主菜单示例（Window层级）
- **SettingsUIController** - 设置弹窗示例（Popup层级，阻塞输入）
- **HeroHpBarMVC** - 血量条示例（传统MVC模式）

### 调试工具
```csharp
// 获取框架统计信息
Debug.Log(MVCManager.Instance.GetStatistics());

// 获取UI栈信息
Debug.Log(MVCManager.Instance.GetUIStackInfo());

// 启用调试日志
MVCManager.Instance.SetDebugLogging(true);
```

## 📂 文件结构

```
Assets/Scripts/UI/Framework/
├── 核心接口
│   ├── IController.cs             # 控制器接口
│   ├── IView.cs                   # 视图接口
│   └── IModel.cs                  # 模型接口
├── 传统MVC组件
│   ├── BaseController.cs          # 基础控制器实现
│   ├── BaseView.cs                # 基础视图实现
│   ├── BaseModel.cs               # 基础模型实现
│   └── ModelVariants.cs           # 模型变体
├── UI管理系统
│   ├── MVCManager.cs              # 统一管理器
│   ├── UILayer.cs                 # 层级定义和属性
│   ├── UIStack.cs                 # UI栈管理
│   └── BaseUIController.cs        # UI控制器基类
├── 示例和测试
│   ├── MVCFrameworkDemo.cs        # 传统MVC演示
│   ├── MVCFrameworkUsageExample.cs # UI栈使用示例
│   ├── MVCTestScene.cs            # 测试场景辅助
│   └── DemoUIControllers.cs       # 示例UI控制器
└── 文档
    ├── README.md                  # 本文档
    ├── USAGE.md                   # 详细使用说明
    └── IMPLEMENTATION_SUMMARY.md  # 实现总结
```

## 🎨 设计原则

### 渐进式迁移
- 可与现有UI组件平滑集成
- 支持逐步将传统UI转换为MVC模式
- 向后兼容，不影响现有代码
- 两种使用模式并存：传统MVC + UI栈管理

### 轻量级实现
- 最小化框架开销，保持Unity性能
- 基于接口的设计，支持灵活扩展
- 避免过度工程化，保持简单易用

### 模块化设计
- 核心组件独立开发和测试
- 支持按需引入特定功能
- 清晰的依赖关系和职责划分

## 📈 扩展指导

### 自定义Model
```csharp
// 继承BaseModel实现自定义逻辑
public class PlayerStatsModel : BaseModel<PlayerStats>
{
    public void LevelUp()
    {
        var stats = Value;
        stats.Level++;
        SetValue(stats);
    }
}

// 或使用EventDrivenModel集成事件系统
public class HealthModel : EventDrivenModel<float, HealthChangedEvent>
{
    protected override HealthChangedEvent CreateEvent(float oldValue, float newValue)
    {
        return new HealthChangedEvent(oldValue, newValue);
    }
}
```

### 自定义View
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

### 自定义UI控制器
```csharp
[UILayer(UILayer.Window, blockInput: false, allowStack: true)]
public class InventoryUIController : BaseUIController<InventoryModel, InventoryView>
{
    protected override void OnBeforeOpen(object args)
    {
        // 打开前逻辑：加载物品数据
        if (args is InventoryOpenArgs openArgs)
        {
            Model.LoadInventory(openArgs.PlayerId);
        }
    }

    protected override void OnAfterOpen(object args)
    {
        // 打开后逻辑：播放动画等
        View.PlayOpenAnimation();
    }
}
```

## 🔧 性能优化

### 更新频率控制
```csharp
// 在Controller中设置更新间隔
controller.SetUpdateInterval(0.1f); // 每0.1秒更新一次

// 禁用自动更新，手动控制
view.SetAutoUpdate(false);
view.RefreshView(); // 手动刷新
```

### 批量操作
```csharp
// 批量更新时临时禁用事件
model.OnValueChanged -= HandleChange;
model.SetValue(newValue1);
model.SetValue(newValue2);
model.OnValueChanged += HandleChange;
model.SetValue(finalValue); // 只触发一次更新
```

### 内存管理
- 使用MVCManager统一管理控制器生命周期
- 确保调用Dispose()方法清理资源
- 正确管理事件订阅/取消订阅

## 📋 最佳实践

### 1. 项目组织
```
UI/
├── Framework/          # MVC框架核心
├── Game/
│   ├── Traditional/   # 传统MVC组件
│   │   ├── Models/    # 数据模型
│   │   ├── Views/     # UI视图
│   │   └── Controllers/ # 控制器
│   ├── UIStack/       # UI栈管理组件
│   │   ├── Menus/     # 菜单UI
│   │   ├── Popups/    # 弹窗UI
│   │   └── HUD/       # HUD UI
│   └── Legacy/        # 传统UI组件（逐步迁移）
```

### 2. 数据流向
```
External Data -> Model -> View -> Display
     ↑                      ↓
Controller <- User Input <- UI Events
```

### 3. 使用建议
- **选择合适的模式**：简单UI用传统MVC，复杂UI管理用UI栈模式
- **层级规划**：根据UI功能选择合适层级，谨慎使用输入阻塞
- **事件处理**：优先使用Model事件系统，需要时使用全局EventManager
- **生命周期**：使用MVCManager统一管理，避免内存泄漏
- **性能考虑**：合理设置更新频率，避免频繁的UI刷新

## 🚨 注意事项

1. **单例模式**: MVCManager使用单例模式，确保场景中只有一个实例
2. **层级配置**: 通过UILayerAttribute配置UI属性，避免硬编码
3. **资源管理**: 当前版本未包含UI Prefab加载，需要手动管理UI资源
4. **线程安全**: 当前版本非线程安全，需要在主线程使用
5. **兼容性**: 新的UI栈系统与传统MVC组件完全兼容

## 📚 相关文档
- **USAGE.md** - 详细的使用说明、API参考和最佳实践
- **IMPLEMENTATION_SUMMARY.md** - 实现总结和技术细节
- **代码注释** - 内联文档和API说明

---

✅ **状态**: 完整MVC框架已实现，包含传统MVC架构和现代UI管理系统  
🎯 **版本**: MVP基础版本，支持渐进式扩展  
🚀 **下一步**: 可根据项目需求扩展动画系统、资源管理等高级功能

