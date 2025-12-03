# Configs - 配置文件

## 概述
存放游戏的所有配置数据，使用ScriptableObject实现可视化编辑和运行时加载。

## 配置分类

### 战斗配置
- **Combat/**：战斗系统配置
  - WeaponConfigs.asset（武器参数配置）
  - CombatSettings.asset（全局战斗设置）
  - DamageSettings.asset（伤害计算配置）

### 波次配置
- **Waves/**：波次系统配置
  - WaveSequences.asset（波次序列定义）
  - DifficultySettings.asset（难度曲线配置）
  - SpawnSettings.asset（生成参数配置）

### 单位配置
- **Units/**：单位属性配置
  - PlayerConfig.asset（玩家属性）
  - EnemyConfigs.asset（敌人属性集合）
  - UnitSettings.asset（单位系统设置）

### 游戏配置
- **Game/**：全局游戏配置
  - GameSettings.asset（游戏全局设置）
  - BalanceConfig.asset（数值平衡配置）

## 配置原则
- 所有数值都应可配置
- 支持运行时热更新（开发阶段）
- 便于策划人员调整参数
- 版本控制友好

## Demo阶段重点
- 建立基础的配置框架
- 实现核心数值的可配置化
- 确保配置系统的可扩展性
