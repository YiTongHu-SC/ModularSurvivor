# MVC UI Framework - MVP 基础版本

## 概述

这是一个轻量级的 Unity UI MVC 框架，提供了基础的 UI 管理功能。框架采用渐进式开发的思路，当前为最小可行版本（MVP），包含核心的 UI 管理功能。

## 核心功能

### 1. UI 生命周期管理
- `Open<T>(args)` - 打开指定类型的 UI
- `Close<T>()` - 关闭指定类型的 UI  
- `CloseTop()` - 关闭栈顶 UI

### 2. 界面栈管理
- 支持 UI 堆叠显示
- Window/Popup 等不同类型的 UI 可以同时存在
- 统一的返回逻辑（栈式管理）

### 3. 层级路由系统
- 自动将 UI 挂载到正确的显示层级
- 支持多种预定义层级：Background、HUD、Window、Popup、Loading、System
- 通过 `UILayerAttribute` 配置 UI 层级属性

### 4. 输入阻塞控制
- Popup/Loading 等 UI 可以阻塞底层输入
- 自动管理输入穿透逻辑
- `HasInputBlocker()` 检查当前是否有阻塞输入的 UI

## 架构设计

```
MVCManager (单例)
├── UI Stack Management (UIStack)
├── Layer Management (UILayer)
├── Controller Registry
└── Lifecycle Management
```

### 核心组件

#### 1. MVCManager
- **职责**: 统一管理所有 UI 控制器的生命周期
- **功能**: UI 栈管理、层级路由、输入控制
- **使用**: `MVCManager.Instance.Open<T>()`

#### 2. UIStack
- **职责**: 管理 UI 的堆叠显示
- **功能**: 栈式 UI 管理、层级分组、输入阻塞检测

#### 3. UILayer
- **职责**: 定义 UI 显示层级
- **层级**: Background(0) → HUD(100) → Window(200) → Popup(300) → Loading(400) → System(500)

#### 4. BaseUIController<TModel, TView>
- **职责**: UI 控制器基类
- **功能**: 生命周期管理、自动层级配置
- **接口**: `IUIController`

## 使用指南

### 1. 创建 UI 控制器

```csharp
// 使用 UILayerAttribute 配置 UI 属性
[UILayer(UILayer.Window, blockInput: false, allowStack: false)]
public class MainMenuUIController : BaseUIController<MainMenuModel, MainMenuView>
{
    protected override void OnAfterOpen(object args)
    {
        // UI 打开后的逻辑
    }

    protected override void OnBeforeClose()
    {
        // UI 关闭前的逻辑
    }
}
```

### 2. 使用 MVCManager

```csharp
// 打开 UI
MVCManager.Instance.Open<MainMenuUIController>(openArgs);

// 关闭特定 UI
MVCManager.Instance.Close<MainMenuUIController>();

// 关闭栈顶 UI
MVCManager.Instance.CloseTop();

// 检查 UI 状态
bool isOpen = MVCManager.Instance.IsUIOpen<MainMenuUIController>();
bool hasBlocker = MVCManager.Instance.HasInputBlocker();
```

### 3. UI 层级配置

```csharp
// 不同类型的 UI 层级配置示例

[UILayer(UILayer.HUD, blockInput: false, allowStack: true)]
public class GameHUDController : BaseUIController<HUDModel, HUDView> { }

[UILayer(UILayer.Window, blockInput: false, allowStack: false)]
public class InventoryController : BaseUIController<InventoryModel, InventoryView> { }

[UILayer(UILayer.Popup, blockInput: true, allowStack: true)]
public class ConfirmDialogController : BaseUIController<DialogModel, DialogView> { }

[UILayer(UILayer.Loading, blockInput: true, allowStack: false)]
public class LoadingController : BaseUIController<LoadingModel, LoadingView> { }
```

## 文件结构

```
Assets/Scripts/UI/Framework/
├── MVCManager.cs              # 主管理器
├── UILayer.cs                 # 层级定义和属性
├── UIStack.cs                 # UI 栈管理
├── BaseUIController.cs        # UI 控制器基类
├── IController.cs             # 控制器接口
├── IView.cs                   # 视图接口
├── IModel.cs                  # 模型接口
├── BaseController.cs          # 基础控制器实现
├── BaseView.cs                # 基础视图实现
├── BaseModel.cs               # 基础模型实现
└── MVCFrameworkUsageExample.cs # 使用示例
```

## 示例代码

查看 `MVCFrameworkUsageExample.cs` 了解完整的使用示例，包括：
- 如何打开/关闭 UI
- 如何处理 UI 栈
- 如何测试输入阻塞
- 如何获取调试信息

## 键盘控制（开发调试）

在示例场景中，可以使用以下按键进行测试：
- `M` - 打开主菜单
- `S` - 打开设置
- `Escape` - 关闭栈顶 UI
- `I` - 显示栈信息和统计

## 扩展计划

当前为 MVP 版本，后续可以扩展的功能：
- UI 动画系统
- 资源管理和异步加载
- UI 数据绑定系统
- 事件系统集成
- UI 状态持久化
- 更复杂的层级管理

## 注意事项

1. **单例模式**: MVCManager 使用单例模式，确保在场景中只有一个实例
2. **层级配置**: 通过 `UILayerAttribute` 配置 UI 属性，避免硬编码
3. **资源管理**: 当前版本未包含 UI Prefab 加载，需要手动管理 UI 资源
4. **性能考虑**: UI 栈操作为 O(n) 复杂度，大量 UI 时需要优化
5. **线程安全**: 当前版本非线程安全，需要在主线程使用

## 总结

该 MVC UI 框架提供了完整的 MVP 功能集，包括 UI 生命周期管理、栈式显示、层级路由和输入控制。代码结构清晰，易于扩展和维护，适合作为 Unity 项目的 UI 管理基础框架。
