# Materials - 材质资源

## 概述
存放项目中使用的所有材质文件，按功能和用途分类管理。

## 材质分类

### 单位材质
- **PlayerMaterial.mat**：玩家单位材质
  - 醒目的颜色（如蓝色）
  - 简单的Unlit或Standard材质

- **EnemyMaterials/**：敌人材质集合
  - Enemy_Basic.mat（基础敌人-红色）
  - Enemy_Fast.mat（快速敌人-橙色）
  - 不同类型用不同颜色区分

### 环境材质
- **GroundMaterial.mat**：地面材质
  - 中性色调
  - 适当的反射率

- **BoundaryMaterial.mat**：边界材质
  - 半透明或线框
  - 调试时可见

### UI材质
- **UIMaterials/**：UI相关材质
  - 按钮材质
  - 面板背景材质

### 特效材质
- **EffectMaterials/**：特效材质
  - 激光材质
  - 粒子材质
  - 发光效果材质

## Demo阶段
- 使用简单的纯色材质
- Standard Shader即可满足需求
- 重点在颜色区分，而非复杂效果
