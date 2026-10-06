---
description: 记录本 fork 相对上游 Avatar-Prop 的改动：把道具挂到头部、朝向跟随锚点、默认开启，以及不应对 PhysBone 属性做动画的注意事项。
---

# 本 Fork 的改动

本仓库是 [Avatar-Prop](https://github.com/ThatFatKidsMom/Avatar-Prop) 的 fork，为 Shinano 做了以下修改，其余机制与上游一致。

## 1. 把"家"挂到头部

- `Reset position` 重命名为 **`Reset transform`**。
- 其上的 VRCFury **Armature Link** 目标从"无骨骼"改为 **Head 骨骼**（HumanBodyBones.Head）。
- 效果：道具的"复位锚点 / 初始位置"由世界锚点改为**跟随头部**。它就是 `Position` 约束的 `source1`，在 `Off` / 启用态被取用。

## 2. 朝向跟随锚点

- 道具 `Position/Rotation` 的 `VRCRotationConstraint` 新增第三个来源 **`source2 = Reset transform`**。
- 挂点 / 未抓取态取 `source2`，使道具在"挂在头上"时朝向跟随锚点（而非手柄的 AimIK）。
- 对应地，17 个状态 clip 里都加了 `Position/Rotation → Sources.source2.Weight` 曲线。

## 3. 默认开启

- `AvatarProp/Enable` 的默认值改为 **1**（`Avatar prop parameters.asset` 与控制器里的 `m_DefaultBool`）。
- 因此加载后（状态机入口仍是 `Off`）会立刻进入 `Enabled`，看上去就是"开机即显示"。

## 4. 关于 PhysBone 的注意事项

- `Bone` 的 `Immobile` 保持上游设计值 **1**（松手后手柄锚在世界、不随 avatar 走动）。
- **不要**给状态 clip 添加任何 PhysBone 属性的动画曲线（如 `immobile`）。PhysBone 属性不可被动画驱动，但 Unity 编辑器在 play 时仍会按曲线写入，从而把 `immobile` 写成 0、破坏"世界锚定"（表现为移动角色时手柄被拖着绕圈）。

## 5. 多实例：自定义 Contact 标签去重

- 新增一个小 NDMF 工具（`NDMF/`）：标记组件 `ContactTagRenamer` + 构建期 pass。装多份 prop 时，构建期给每个实例的自定义 Contact 标签加上唯一后缀，避免不同道具通过共享标签互相触发。
- 目标标签在组件上配置（本 prefab 填了 `AvatarProp.Distance`、`Avatarprop.DistanceCheck`）；未打标记的实例不处理。
- 详见 [参数与接触标签](./参数与接触标签.md)。

## 相关文档

- 系统总览：[系统总览](./系统总览.md)
- 位移：[位移链路](./位移链路.md)
- 旋转：[旋转链路](./旋转链路.md)
- 锁存：[锁存机制](./锁存机制.md)
