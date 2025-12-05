# Buff System - 增益减益效果系统

## 概述

Buff系统是一个轻量级的增益/减益效果管理系统，支持各种类型的临时状态修改。

## 核心组件

### BuffData
- **作用**：Buff模板数据配置
- **属性**：
  - ID：唯一标识符
  - Name：显示名称
  - Type：效果类型（枚举）
  - Duration：持续时间
  - Value：效果数值
  - CanStack：是否可叠加

### Buff
- **作用**：Buff实例，管理单个Buff的生命周期
- **功能**：
  - 时间管理和自动过期
  - 叠加逻辑处理
  - 效果值计算

### BuffSystem
- **作用**：Buff系统主控制器
- **功能**：
  - 应用/移除Buff
  - 更新所有Buff状态
  - 处理持续效果（DOT/HOT）
  - 管理Buff模板

## 支持的Buff类型

### 属性修改类
- **SpeedBoost**：增加移动速度（乘法）
- **SpeedReduction**：减少移动速度（乘法）
- **AttackBoost**：增加攻击力（乘法）
- **AttackReduction**：减少攻击力（乘法）

### 持续效果类
- **Poison**：持续伤害（DOT）
- **Regeneration**：持续治疗（HOT）

## 使用示例

```csharp
// 获取Buff系统实例
var buffSystem = CombatManager.Instance.BuffSystem;

// 应用速度提升Buff到单位1001
buffSystem.ApplyBuff(1001, 1);

// 移除指定Buff
buffSystem.RemoveBuff(1001, 1);

// 获取单位所有活跃Buff
var buffs = buffSystem.GetUnitBuffs(1001);

// 清除单位所有Buff
buffSystem.ClearUnitBuffs(1001);
```

## 事件系统集成

系统会自动发布以下事件：
- **BuffAppliedEvent**：Buff被应用时
- **BuffRemovedEvent**：Buff被移除时

可以通过EventManager订阅这些事件：

```csharp
EventManager.Instance.Subscribe<GameEvents.BuffAppliedEvent>(this);
```

## 扩展说明

### 添加新的Buff类型

1. 在`BuffType`枚举中添加新类型
2. 在`BuffSystem.InitializeBuffTemplates()`中注册模板
3. 在`ApplyBuffEffect()`和`RemoveBuffEffect()`中处理效果
4. 对于持续效果，在`ProcessContinuousEffect()`中添加逻辑

### 自定义Buff模板

```csharp
var customBuff = new BuffData(
    id: 100,
    name: "自定义效果", 
    type: BuffType.SpeedBoost,
    duration: 10f,
    value: 3f,
    canStack: true
);
```

## 性能优化

- 使用Dictionary进行O(1)查找
- 过期Buff自动清理
- 避免不必要的计算
- 支持批量操作

## 注意事项

1. **叠加机制**：只有CanStack=true的Buff才能叠加
2. **时间管理**：系统需要在Update中调用UpdateBuffs()
3. **属性依赖**：需要UnitData包含相应的属性字段
4. **线程安全**：当前实现不是线程安全的

## 相关文件

- `BuffData.cs` - 数据结构定义
- `Buff.cs` - Buff实例类
- `BuffSystem.cs` - 系统主逻辑
- `BuffSystemDemo.cs` - 使用演示
