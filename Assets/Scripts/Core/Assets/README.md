# 资源加载系统 (Asset System)

## 概述

资源加载系统是一个为ModularSurvivor项目设计的，支持从Resources平滑迁移到Addressables的统一资源管理解决方案。该系统解决了资源访问收口、生命周期管理、依赖显式化、可观测性和可扩展性等核心问题。

## 核心特性

### 🎯 设计目标
- **访问收口**：业务层不直接接触Resources.Load，避免字符串路径散落
- **生命周期可控**：通过Scope管理资源的加载与释放
- **依赖显式化**：通过Manifest明确定义需要预加载的资源
- **可观测性**：提供加载状态、耗时统计、内存监控等信息
- **可扩展性**：设计支持从Resources平滑迁移到Addressables

### 🏗️ 架构分层
系统分为以下4个层次：
1. **Key/Catalog层**：AssetKey → 资源路径映射
2. **Provider层**：统一的资源加载接口
3. **Cache/Handle层**：资源缓存与引用计数管理
4. **Loader/Pipeline层**：批量加载与流程编排

## 核心组件

### AssetKey
```csharp
AssetKey key = "ui:main_menu";  // 业务层只使用键，不使用路径
```
- 类型安全的资源键结构
- 支持隐式字符串转换
- 统一的资源标识符

### AssetCatalog (ScriptableObject)
```csharp
// 维护 AssetKey → Resources路径 的映射关系
// 将来可以替换为 AssetKey → Addressables Key 映射
```
- Key到路径的映射配置
- 支持资源类型和标签分类
- 便于资源路径统一管理

### IAssetProvider
统一的资源访问门面：
```csharp
// 同步加载（仅限小资源）
AssetHandle<T> Load<T>(AssetKey key);

// 异步加载
Task<AssetHandle<T>> LoadAsync<T>(AssetKey key);

// 批量异步加载
Task<AssetHandle<T>[]> LoadBatchAsync<T>(AssetKey[] keys);

// 实例化预制体
Task<GameObject> InstantiateAsync(AssetKey key);
```

### AssetHandle<T>
资源句柄，封装加载状态和生命周期：
```csharp
if (handle.IsValid)
{
    var prefab = handle.Asset;  // 获取资源
}
```
- 包含加载状态、错误信息、引用计数
- 类似Addressables的AsyncOperationHandle概念
- 支持引用计数管理

### AssetScope
资源生命周期容器：
```csharp
// 全局作用域 - 游戏启动到退出
var globalScope = assetSystem.GlobalScope;

// 关卡作用域 - 进入关卡到退出关卡
var levelScope = assetSystem.CreateLevelScope("level_001", runId);
levelScope.ReleaseAll();  // 退出关卡时一键释放所有资源
```

预定义作用域：
- **GlobalScope**：全局常驻资源（Loading UI、字体、通用资源）
- **FrontendScope**：前端界面资源（主菜单相关）
- **LevelScope(levelId/runId)**：关卡资源（每次进关卡创建新的）

### AssetManifest (ScriptableObject)
资源清单，定义批量加载的资源列表：
```csharp
// 包含资源键、类型、权重、必需性、标签等信息
// 支持按标签过滤、按必需性分组
```

### LoadingPipeline
加载流程编排器：
```csharp
var pipeline = LoadingPipelineBuilder.CreateLevelPipeline(levelId, runId, levelManifest, enemyManifest);
await pipeline.ExecuteAsync(progressCallback);
```

分阶段加载：
1. **场景加载** (30%)
2. **关卡必需资源** (40%)
3. **敌人资源** (20%)
4. **收尾处理** (10%)

### MemoryMaintenanceService
内存维护服务，控制UnloadUnusedAssets的执行策略：
```csharp
// 触发条件：
// - 连续切关卡N次
// - 内存使用超过阈值
// - 距离上次维护超过时间间隔

service.NotifyLevelSwitch();        // 通知关卡切换
service.NotifyReturnToMainMenu();   // 通知回到主菜单
```

## 使用指南

### 1. 初始化系统
```csharp
// 通过AssetSystemInitializer自动初始化
// 或手动初始化：
var assetSystem = new AssetSystem(assetCatalog);
```

### 2. 配置资源目录
创建AssetCatalog ScriptableObject，配置资源映射：
```
ui:loading -> UI/LoadingView
ui:hud -> UI/HUD/HUDView
enemy:slime:prefab -> Enemies/Slime
enemy:slime:config -> Enemies/SlimeConfig
```

### 3. 加载全局资源
```csharp
var globalManifest = LoadGlobalManifest();
await assetSystem.LoadManifestAsync(globalManifest, AssetSystem.GlobalScopeName);
```

### 4. 关卡加载流程
```csharp
// 创建关卡作用域
var levelScope = assetSystem.CreateLevelScope(levelId, runId);

// 使用加载管线
var pipeline = LoadingPipelineBuilder.CreateLevelPipeline(levelId, runId, levelManifest);
var success = await pipeline.ExecuteAsync(progress => UpdateLoadingBar(progress));

// 退出关卡时释放
levelScope.ReleaseAll();
assetSystem.ReleaseScope(levelScope.Name);
```

### 5. 运行时使用
```csharp
// 获取资源
var handle = await levelScope.AcquireAsync<GameObject>("enemy:slime:prefab");
if (handle.IsValid)
{
    var enemy = Instantiate(handle.Asset);
}

// 实例化预制体
var enemyInstance = await levelScope.InstantiateAsync("enemy:slime:prefab", parent);
```

## 目录规范

### Resources目录结构（建议）
```
Resources/
├── _Global/          # 全局资源
│   ├── UI/
│   └── Audio/
├── UI/              # UI资源
│   ├── Menus/
│   └── HUD/
├── Levels/          # 关卡相关
│   ├── Level001/
│   └── Level002/
├── Prefabs/         # 预制体
│   ├── Enemies/
│   └── Weapons/
└── Audio/           # 音频资源
    ├── BGM/
    └── SFX/
```

### 资源键命名规范
- UI资源：`ui:name` (如 ui:main_menu, ui:hud)
- 敌人资源：`enemy:type:asset_type` (如 enemy:slime:prefab, enemy:slime:config)
- 关卡资源：`level:id:type` (如 level:001:config)
- 音频资源：`audio:type:name` (如 audio:bgm:level001)
- 特效资源：`vfx:name` (如 vfx:hit_normal)

## 内存管理策略

### 释放策略
1. **退出关卡时**：
   - 执行 `LevelScope.ReleaseAll()`
   - 清理关卡实例对象
   - 通知 MemoryMaintenanceService

2. **内存维护触发条件**：
   - 连续切关卡3次
   - 内存使用超过500MB
   - 距离上次维护超过5分钟

3. **维护执行时机**：
   - 回到主界面后延迟执行（避免影响切换体验）
   - 或满足触发条件时延迟执行

### 监控指标
- 当前内存使用量
- 资源加载耗时统计
- 作用域资源数量
- 内存维护频次和效果

## 扩展性设计

### 迁移到Addressables
当需要迁移到Addressables时，只需要：
1. 实现 `AddressablesAssetProvider`
2. 将AssetCatalog的映射从Resources路径改为Addressables键
3. 替换AssetSystem中的Provider实现

业务代码无需任何修改。

### 对象池集成
资源系统专注于Asset管理，对象池专注于Instance管理：
- Asset缓存：缓存Prefab/Sprite等资产
- 对象池：缓存Instantiate出来的GameObject实例
- 两者生命周期独立，便于分别优化

## 调试与监控

### 内存统计
```csharp
var memoryStats = MemoryMaintenanceService.Instance.GetMemoryStats();
Debug.Log(memoryStats.ToString());
```

### 作用域状态
```csharp
var scopeInfos = AssetSystemUtils.GetAllScopeInfos();
foreach (var info in scopeInfos)
{
    Debug.Log(info.ToString());
}
```

### 加载进度追踪
```csharp
var pipeline = LoadingPipelineBuilder.CreateLevelPipeline(levelId, runId);
await pipeline.ExecuteAsync(status => {
    Debug.Log($"Phase: {status.CurrentPhase}, Progress: {status.OverallProgress:P}");
});
```

## 最佳实践

1. **资源键统一管理**：使用AssetSystemUtils中的常量，避免硬编码字符串
2. **作用域合理划分**：按生命周期将资源分组到不同作用域
3. **清单分类管理**：大清单拆分为小清单，支持按需加载
4. **内存定期维护**：合理配置内存维护策略，避免频繁GC
5. **异步优先**：运行时尽量使用异步加载，避免卡顿
6. **错误处理**：合理设置失败策略，处理资源加载失败情况

通过这套资源系统，可以实现"敌人上百"规模下的高效资源管理，同时为未来的技术升级预留了平滑迁移路径。
