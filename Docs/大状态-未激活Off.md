---
description: 讲解未激活状态 Off 的行为（隐藏道具、关闭追踪、把道具放在复位点），以及它的进出迁移。
---

# 大状态：未激活 Off

`Off` 是状态机的入口默认状态，使用 `Off.anim`。注意：本 fork 里 `AvatarProp/Enable` 默认 = 1，所以加载后通常立刻离开 `Off` 进入 `Enabled`；只有把开关关掉才会回到 `Off`。

## 这个状态做什么

| 属性 | 值 | 效果 |
|---|---|---|
| `Object container` `m_IsActive` | 0 | 道具隐藏 |
| `Tracker container` `m_IsActive` | 0 | 追踪器整组关闭 |
| `Cull` `GlobalWeight` | 0 | 剔除辅助体权重 0 |
| 三个 tracker `GlobalWeight`（ScaleConstraint） | 0 | tracker 保持初始小尺寸 |
| `Position` 源权重 | source1 `Reset transform` = 1 | 道具位置锁到复位锚点 |
| `Bone base` `m_Enabled` | 1；源 source0 `Reset transform` = 1 | 手柄底座跟随复位锚点 |
| `Bone`（PhysBone） | 关（靠状态控制） | 手柄不参与 |
| `Save position` `m_Enabled` | 1；source1（Hand） = 0.6 | 锁存体在追踪 |

> 另外 `Off 1` 是同一个 clip 的变体，行为一致。

## 迁移

- 进入：从 `Enabled`、`On reset`、`Enabled 1 reset` 等状态，当 `Enable` 为 false（关掉菜单）时经 `transition off` 回到 `Off`。
- 离开：`Off → Enabled`，条件是 `Enable` 为 true。

## 相关文档

- 总览：[状态机总览](./状态机总览.md)
- 下一个大状态：[大状态-准备与校准](./大状态-准备与校准.md)
