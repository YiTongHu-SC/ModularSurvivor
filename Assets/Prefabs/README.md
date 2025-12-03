# Prefabs - 预制体资源

## 概述
存放所有游戏预制体，按功能分类组织，便于管理和复用。

## 目录结构

### Units/
- **用途**：单位相关预制体
- **包含**：
  - Player.prefab（玩家单位）
  - Enemy_Basic.prefab（基础敌人）
  - Enemy_Fast.prefab（快速敌人）
  - 其他单位变体

### Environment/
- **用途**：环境和场景预制体
- **包含**：
  - Ground.prefab（地面）
  - Boundaries.prefab（边界）
  - 环境装饰物件

### UI/
- **用途**：UI相关预制体
- **包含**：
  - WaveInfoPanel.prefab
  - GameHUD.prefab
  - 菜单界面预制体

### Effects/
- **用途**：特效预制体
- **包含**：
  - 攻击特效
  - 伤害特效
  - 环境特效

## 命名规范
- 使用PascalCase命名
- 功能前缀 + 具体名称
- 版本后缀（如有必要）
