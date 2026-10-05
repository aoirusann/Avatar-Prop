---
description: 讲解 Grabbed（左右手）这一抓取状态的行为、它与准备态的区别（传感器关闭、tracker 放大），以及它与 Reattach、Prop dropped 之间的迁移。
---

# 大状态：被抓取 Grabbed

包含 `Grabbed with left hand`、`Grabbed with right hand`，以及它们的 `transition off` 变体。

## 进入条件

由 `Expansion n Hand check` 或 `Reattach` 进入，条件（实测）：

- `TrackersCheck` 为 true；且
- `Hand/Prox > 0.57`、`Middle/Prox > 0.57`、`Ring/Prox > 0.57`；且
- 右手：`RightHand > 0.5 & LeftHand < 0.51`（左手反之）。

即"三点收敛 + 抓握到位 + 判定出手"。

## 这个状态做什么（与准备态相反）

| 属性 | 值 | 效果 |
|---|---|---|
| 三个 `Proximity` / `Hand checker` `m_IsActive` | 0 | **检测用的传感器关闭**（已经不需要） |
| tracker 的 ScaleConstraint `GlobalWeight` | 1 | tracker 被约束到源 `000` 的 scale 1，**放大** |
| `Position` 源权重 | source0 `Resyncy boi` = 1 | 道具位置跟手（见 [02-位移链路](./02-位移链路.md)） |
| `Rotation` 源权重 | source0 `Rotation`(AimIK) = 1 | 道具朝向跟 AimIK（见 [03-旋转链路](./03-旋转链路.md)） |
| `Save position` `m_Enabled` | 1 | 锁存体在后台跟踪（见 [11-锁存机制](./11-锁存机制.md)） |

## 迁移

- `→ Reattach`：某个 `Prox < 0.3`，或 `TrackersCheck` 为 false（还握着但姿势失效）。
- `→ Prop dropped …`（松手）：`IsGrabbed` 为 false。
- `→ Prop dropped …`（关开关）：`Enable` 为 false。
- `→ … transition off`：`Enable` 为 false。

## 一个常见现象：编辑器里接收器球"放大又缩回"

这在编辑器里很常见，原因写在 [04-Tracker位置与TrackerBlendTree](./04-Tracker位置与TrackerBlendTree.md) 的 tracker 结构与这里的状态切换：

- 大小来自 tracker 的 ScaleConstraint：`Grabbed` 设 `GlobalWeight=1`（大），`Expansion/Reattach` 设 `0`（小）。
- 从 `Grabbed` 出来的转移都是 `duration = 0`，所以一帧内就缩回，看起来"突然"。
- 编辑器里是用鼠标（`PhysBoneGrabHelper`）抓的，而手/指接触体固定在 T-pose 手骨上、不跟着被抓走的 PhysBone 走，导致 `Grabbed` 的条件保不住，于是 `Grabbed ↔ Expansion/Reattach` 反复横跳 → 周期性地放大/缩回。游戏里用真手抓时几何一致，通常不会这样。

## 相关文档

- 总览：[06-状态机总览](./06-状态机总览.md)
- 下一个大状态：[10-大状态-放下PropDropped](./10-大状态-放下PropDropped.md)
