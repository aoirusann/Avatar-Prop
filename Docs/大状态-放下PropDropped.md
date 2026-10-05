---
description: 讲解 Prop dropped、Prop dropped uncalibrated、Reattach 及 released/reset/transition 等小状态的动画行为与迁移关系。
---

# 大状态：放下 Prop dropped

这一大状态处理"松手后停住"以及"抓不稳时重新校准"。包含多个小状态。

## Prop dropped right/left hand

- clip：`Right released.anim` / `Left released.anim`。
- 行为：
  - `Object container` 保持显示、`Tracker container` 打开、`Proximity` 关闭；tracker 保持小尺寸。
  - `Position` 源切到 source2 `Save position`（= 1）；`Rotation` 源切到 source1 `Save rotation`（= 1）。
  - `Save position` / `Save rotation` 的 `m_Enabled = 0`：**锁存体被关闭，冻结在松手前的姿态**。
- 效果：道具就停在松手的位置与朝向。原理见 [锁存机制](./锁存机制.md)。

## Prop dropped uncalibrated

- 由 `Expansion n Hand check` 在 `IsGrabbed` 变 false 时进入（还没抓稳就松了）。
- 行为同 `Prop dropped`（用 `Right released` clip），随后按距离条件去 `Reset root bone pos` 或 `Enabled 1 reset`。

## Reattach（重新校准）

- 由 `Grabbed` 进入：某个 `Prox < 0.3` 或 `TrackersCheck` 为 false（仍握着但姿势失效）；或由 `Prop dropped` 在"又抓住了"时进入。
- clip：`Reattach.anim`。关键帧：`Tracker container m_IsActive` 先是 0（`t0`），到 `t=0.167s` 变 1 —— 开头短暂关闭让 tracker 复位，然后重新打开三组 `Proximity` / `Hand checker`；tracker 小尺寸；`Position` 源回到 `Resyncy boi`；`Save position` 打开。
- 迁移：
  - `→ Grabbed (左/右)`：三点 `Prox > 0.57` + `TrackersCheck` + 某只手。
  - `→ Prop dropped …`：`IsGrabbed` 为 false 或 `Enable` 为 false。

## released reset / transition off

- `Right released resetttrt`、`Left released resetrr`、`Prop dropped transition off`、`Grabbed … transition off` 等是**过渡用小状态**，负责在"放下 → 复位"或"关闭 → Off"之间平滑切换。
- 例如 `Left released resetrr` 会按条件去 `Reattach`、`Reset root bone pos left` 或 `Off`。

## 迁移小结

```
Grabbed ──(松手 IsGrabbed 关)──► Prop dropped ──(又抓住)──► Reattach ──(抓稳)──► Grabbed
Grabbed ──(抓不稳 Prox<0.3 / TrackersCheck 假)──► Reattach
Prop dropped ──(DistanceTooFar)──► Reset root bone pos
Prop dropped ──(关开关)──► … transition off ──► Off
```

## 相关文档

- 总览：[状态机总览](./状态机总览.md)
- 锁存原理：[锁存机制](./锁存机制.md)
