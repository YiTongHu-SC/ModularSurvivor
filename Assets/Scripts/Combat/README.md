﻿# Combat - 战斗系统

## 概述
实现完整的自动战斗机制，包括技能系统、移动AI、伤害计算、Buff效果等。采用类似幸存者游戏的自动攻击模式，所有战斗逻辑在2D坐标系中计算。

## 系统架构

### Systems/ - 战斗核心系统
- **CombatManager**：战斗系统总控制器，集成所有子系统
- **AbilitySystem**：技能系统，管理技能释放和更新
- **MovementSystem**：移动系统，处理单位移动和AI
- **DamageSystem**：伤害系统，计算和应用伤害
- **BuffSystem**：Buff系统，管理增益减益效果
- **ViewSystem**：表现层系统，管理3D视觉效果

### Ability/ - 技能系统
- **BaseAbility**：技能基类，定义技能生命周期
- **AbilityFactory**：技能工厂，负责技能创建和配置
- **技能类型**：
  - `AbilityLaserStrike`：激光攻击技能
  - `AbilityHitOnceOnCollision`：碰撞攻击技能
  - `AbilityPlayerInput`：玩家输入驱动技能
- **Data/**：技能配置数据和参数定义

### Movement/ - 移动系统
- **IMovementStrategy**：移动策略接口
- **移动策略实现**：
  - `StraightChaseStrategy`：直线追击策略
  - `SimpleMoveStrategy`：简单移动策略
- **MovementData**：移动数据结构（Vector2方向+速度）

### Buff/ - 增益减益系统
- **BuffSystem**：Buff核心管理器
- **BaseBuff**：Buff效果基类
- **BuffFactory**：Buff工厂和创建管理
- **支持的Buff类型**：
  - 速度提升/减缓（SpeedBoost/SpeedReduction）
  - 攻击力提升/减少（AttackBoost/AttackReduction）  
  - 持续伤害/治疗（Poison/Regeneration）
- **特性**：支持叠加、时间管理、事件通知

### Actors/ - 战斗单位
- **Actor**：战斗单位核心类，封装单位数据和行为
- **ActorData**：战斗单位数据配置

### Views/ - 表现层组件
- **BaseUnitPresentation**：单位表现基类
- **UnitView**：单位视图组件
- **ViewFactory**：视图工厂，管理3D表现创建
- **专用视图**：
  - `LaserStrikeView`：激光攻击特效
  - 其他技能视觉效果

### Data/ - 数据配置
- 战斗相关的配置数据结构
- 武器、技能、单位属性等数据定义

## 核心特性

### 自动战斗机制
- 玩家角色自动寻找最近敌人攻击
- 支持多种攻击类型：射线、弹道、范围攻击
- 智能目标选择和攻击时机控制

### 技能系统特色
- **生命周期管理**：完整的技能创建、更新、销毁流程
- **数据驱动**：技能参数通过ScriptableObject配置
- **扩展性设计**：易于添加新的技能类型
- **视觉分离**：技能逻辑与特效表现完全分离

### 移动AI系统
- **策略模式**：多种移动行为策略可切换
- **智能追击**：敌人自动追击玩家或最近目标
- **碰撞避免**：基础的碰撞检测和避免机制
- **性能优化**：高效的路径计算和更新

### Buff效果系统
- **完整的Buff生命周期**：应用、叠加、更新、移除
- **多种效果类型**：属性修改、持续伤害/治疗
- **事件驱动**：Buff状态变化通过事件通知
- **性能优化**：Dictionary快速查找，自动清理

## 设计亮点

### 2D逻辑架构
- 所有战斗计算在2D平面进行，简化复杂度
- 3D表现层仅负责视觉效果，不影响游戏逻辑
- 支持2D碰撞检测和区域计算

### 事件驱动通信
- 模块间通过事件系统松耦合通信
- 支持UI更新、音效触发等响应机制
- 便于调试和扩展功能

### 可配置设计
- 技能、Buff、移动参数都可外部配置
- 支持热更新和数值调优
- 便于关卡设计和平衡性调整

### 性能考虑
- 对象池管理减少GC压力
- 批量更新提升效率
- 智能的碰撞检测优化

## 使用示例

```csharp
// 获取战斗管理器
var combat = CombatManager.Instance;

// 创建技能
combat.AbilitySystem.CreateAbility(abilityData, casterActor);

// 应用Buff
combat.BuffSystem.ApplyBuff(unitId, buffId);

// 设置移动策略
combat.MovementSystem.SetMovementStrategy(unit, new StraightChaseStrategy());
```

## 扩展指导

### 添加新技能
1. 继承`BaseAbility`创建技能类
2. 定义对应的数据配置类
3. 在`AbilityFactory`中注册新技能类型
4. 创建对应的视觉效果组件

### 添加新Buff
1. 继承`BaseBuff`实现效果逻辑
2. 在`BuffFactory`中注册新Buff类型
3. 在`BuffSystem`中添加相应处理逻辑

### 添加新移动策略
1. 实现`IMovementStrategy`接口
2. 在需要的地方设置新的移动策略
