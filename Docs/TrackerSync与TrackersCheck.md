---
description: 讲解 TrackerSync 如何对三组 Tracker 位置求平均，以及 TrackersCheck 参数如何作为"抓握是否收敛有效"的校验信号。
---

# TrackerSync 与 TrackersCheck

## TrackerSync：三点的平均

`Tracker container/TrackerSync` 上有一个 `VRCPositionConstraint`：

```
源 = Hand w1 + Middle finger w1 + Ring finger w1
```

也就是三组 tracker 位置的**加权平均（质心）**。它不参与显示，只用于下面的判定。

## TrackersCheck：抓握校验

`TrackerSync` 上还有一个 `VRCContactReceiver`：

```
tag = AvatarProp.Distance     type = Constant     radius = 0.2     param = AvatarProp/TrackersCheck
```

它要重叠的发送体是 `Bone base/Bone/Resyncy boi`：

```
tag = AvatarProp.Distance     radius = 0.05
```

`Constant` 类型表示：只要两者重叠就持续输出 1，否则 0。于是：

> **`TrackersCheck = true` 当且仅当 三组 tracker 的平均位置（TrackerSync）与抓取参考点（Resyncy boi）重叠。**

它本质是一个**"基于 Contact 的姿势估计是否可信、抓握是否到位"的校验位**：tracker 的位置会被接触偏移带偏，若三点没有收敛到参考点附近，平均点就会离开，`TrackersCheck` 变 false。

## 状态机怎么用它

- 进入 `Grabbed` 需要 `TrackersCheck == true`，并且三个 `Prox > 0.57`（见 [大状态-被抓取Grabbed](./大状态-被抓取Grabbed.md)）。
- 在 `Grabbed` 里若 `TrackersCheck` 变 false（仍握着），退回 `Reattach` 重新校准（见 [大状态-放下PropDropped](./大状态-放下PropDropped.md)）。

即：三个 `Prox` 负责"手在不在"，`TrackersCheck` 负责"三点是否收敛、估计是否可信"。

## 相关文档

- Tracker 位置计算：[Tracker位置与TrackerBlendTree](./Tracker位置与TrackerBlendTree.md)
- 状态机：[状态机总览](./状态机总览.md)
