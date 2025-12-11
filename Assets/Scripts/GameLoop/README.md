# GameLoop - 游戏循环管理

## 概述
管理游戏的核心循环和生命周期，协调各个子系统的初始化、更新和销毁。作为游戏的入口点和总控制器，确保各模块按正确的顺序执行。

## 系统架构

### Game/ - 游戏主控制器
- **GameManager**：游戏总管理器
  - 管理游戏的整体生命周期
  - 协调各子系统的初始化和更新
  - 处理游戏状态转换（开始、暂停、结束等）
  - 提供游戏级别的服务访问接口

### Config/ - 游戏配置管理
- **配置系统**：管理游戏的全局配置
  - 游戏难度设置和平衡参数
  - 系统性能和优化配置
  - 调试和开发者选项
  - 运行时配置的加载和应用

### GameDebug/ - 开发调试工具
- **CreateHeroOnStart**：开发阶段的英雄创建工具
  - 自动在游戏开始时创建玩家角色
  - 提供快速测试和开发的便利功能
  - 支持不同的测试场景配置

### Utils/ - 游戏循环工具
- **CreateObjects**：对象创建工具
  - 提供统一的游戏对象创建接口
  - 集成对象池和资源管理
  - 支持异步创建和延迟初始化

## 核心特性

### 生命周期管理
- **初始化阶段**：
  - 各子系统的有序初始化
  - 配置加载和验证
  - 资源预加载和准备
  - 依赖注入和服务注册

- **运行阶段**：
  - 统一的Update循环调度
  - 各系统的协调更新
  - 性能监控和优化
  - 错误处理和恢复

- **清理阶段**：
  - 资源释放和清理
  - 数据保存和持久化
  - 系统关闭和状态重置

### 状态管理
- **游戏状态**：
  - `Loading`：游戏加载中
  - `Playing`：正常游戏进行中
  - `Paused`：游戏暂停
  - `GameOver`：游戏结束
  - `Menu`：菜单界面

- **状态转换**：
  - 安全的状态切换机制
  - 状态转换的验证和回滚
  - 状态变化的事件通知
  - 状态相关的资源管理

### 系统调度
- **更新顺序**：确保各系统按正确顺序更新
```csharp
// 典型的更新顺序
1. InputSystem.Update()      // 输入处理
2. CombatSystem.Update()     // 战斗逻辑
3. MovementSystem.Update()   // 移动计算
4. WaveSystem.Update()       // 波次管理
5. UISystem.Update()         // 界面更新
```

- **时间管理**：
  - 统一的时间标准和Delta时间分发
  - 暂停状态下的时间控制
  - 慢动作和时间缩放效果
  - 固定时间步长的物理更新

### 服务定位器
- **全局服务访问**：提供各系统服务的统一访问点
- **依赖注入**：管理系统间的依赖关系
- **生命周期绑定**：服务的创建和销毁与游戏生命周期绑定
- **接口抽象**：通过接口访问服务，降低耦合度

## 设计特色

### 事件驱动架构
- **系统通信**：各系统通过事件进行通信
- **松耦合设计**：减少直接依赖，提高模块独立性
- **扩展性**：易于添加新系统和功能
- **调试友好**：事件流程易于追踪和调试

### 可配置性
- **外部配置**：核心参数通过配置文件控制
- **运行时调整**：支持运行时修改部分配置
- **环境适配**：根据设备性能自动调整配置
- **开发工具**：提供开发阶段的特殊配置选项

### 错误处理
- **异常捕获**：关键系统的异常捕获和处理
- **优雅降级**：系统故障时的备用方案
- **错误报告**：详细的错误日志和报告
- **自动恢复**：可能的错误自动恢复机制

### 性能优化
- **批量处理**：相关操作的批量执行
- **延迟加载**：非关键资源的延迟加载
- **内存管理**：智能的内存分配和回收
- **性能监控**：实时的性能指标监控

## 实现详情

### GameManager核心流程
```csharp
public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        InitializeCoreSystems();
        LoadConfigurations();
        RegisterEventHandlers();
    }

    private void Start()
    {
        StartGameLoop();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        
        // 按顺序更新各系统
        InputManager.Instance.Update(deltaTime);
        CombatManager.Instance.Tick(deltaTime);
        WaveManager.Instance.UpdateWaves(deltaTime);
        UIManager.Instance.Update(deltaTime);
    }
}
```

### 配置系统集成
- **ScriptableObject配置**：使用Unity的配置资源系统
- **JSON配置文件**：支持外部JSON配置文件
- **命令行参数**：支持启动参数配置
- **环境变量**：读取系统环境变量配置

### 调试工具集成
- **开发者控制台**：运行时调试命令系统
- **性能分析器**：集成Unity Profiler
- **可视化调试**：Scene视图中的调试绘制
- **热重载**：支持代码和配置的热重载

## Demo-V1实现状态

### 已实现功能
- **基础游戏循环**：完整的初始化、更新、清理流程
- **英雄创建工具**：开发阶段的快速测试工具
- **系统集成**：战斗、波次、UI等系统的统一管理
- **基础配置**：核心配置参数的管理

### 优化重点
- **启动性能**：优化游戏启动时间
- **内存使用**：减少运行时内存占用
- **更新效率**：优化每帧的更新开销
- **错误处理**：完善的异常处理机制

## 使用示例

### 获取游戏管理器
```csharp
// 访问游戏状态
var currentState = GameManager.Instance.CurrentGameState;

// 暂停游戏
GameManager.Instance.PauseGame();

// 重启游戏
GameManager.Instance.RestartGame();
```

### 配置系统使用
```csharp
// 获取配置
var difficulty = ConfigManager.Instance.GetConfig<GameDifficultyConfig>();

// 应用配置
ConfigManager.Instance.ApplyConfig(newConfig);
```

## 扩展指导

### 添加新的游戏状态
1. 在GameState枚举中添加新状态
2. 在GameManager中实现状态转换逻辑
3. 添加状态进入和退出的处理方法
4. 更新UI和其他系统的状态响应

### 集成新的子系统
1. 在GameManager的Initialize中添加系统初始化
2. 在Update循环中添加系统更新调用
3. 处理系统的依赖关系和初始化顺序
4. 添加系统的配置和调试支持

### 性能优化建议
- 使用Unity Jobs System进行并行计算
- 实现系统的LOD（细节层次）管理
- 添加性能预算和自适应质量设置
- 使用对象池减少内存分配
