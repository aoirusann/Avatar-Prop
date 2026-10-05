---
description: 讲解三组 Tracker 的位置如何由接触信号经 Tracker Blend Tree 连续计算得到，包括它们各自的接触传感器与 Position 约束来源。
---

# Tracker 位置与 Tracker Blend Tree

## 一组 tracker 的结构

`Tracker container` 下有三组，结构完全相同（以 `Hand` 为例）：

```
Hand  (localScale = 0.16)
├─ Proximity   ContactReceiver: Proximity, Sphere r=1, param = …/Hand/Prox
├─ X+          ContactReceiver: Proximity, Sphere r=1, localPos=(-0.28,-0.28,-0.28), param = …/Hand/X+
├─ X-          ContactReceiver: Proximity, Sphere r=1, localPos=(-0.28, 0.28, 0.28), param = …/Hand/X-
├─ Y+          ContactReceiver: Proximity, Sphere r=1, localPos=( 0.28,-0.28, 0.28), param = …/Hand/Y+
├─ Y-          ContactReceiver: Proximity, Sphere r=1, localPos=( 0.28, 0.28,-0.28), param = …/Hand/Y-
├─ VRCParentConstraint    源 = Resyncy boi  w1     （基准：抓取参考点）
├─ VRCPositionConstraint  (默认关闭) 源 = X+ X- Y+ Y- 自身
└─ VRCScaleConstraint     源 = 000（受状态动画控制 GlobalWeight）
```

- 5 个接收器都是 `Proximity`，输出 0~1 浮点；4 个方向接收器位于一个四面体的 4 个角（每个坐标 ±0.28）。
- `ParentConstraint` 让 tracker 的**基准位置**跟随抓取参考点 `Resyncy boi`。

## Tracker Blend Tree 做什么

`Tracker Blend Tree` 是一个 **Direct Blend Tree**，12 个 1D 子树，每个子树由**一个方向参数**驱动，输出**一条 source 权重曲线**：

```
Ring/X+ -> Tracker container/Ring finger | Sources.source0.Weight
Ring/X- -> Tracker container/Ring finger | Sources.source1.Weight
Ring/Y+ -> Tracker container/Ring finger | Sources.source2.Weight
Ring/Y- -> Tracker container/Ring finger | Sources.source3.Weight
Hand/X±Y±   -> Hand 的 source0..3
Middle/X±Y± -> Middle finger 的 source0..3
```

关键：它是**反相**的（实测 Start/End 值）：

```
参数 = 0（没碰到该方向的球） -> 该 source 权重 = 1.00
参数 = 1（碰到）             -> 该 source 权重 = 0.00
```

于是 `PositionConstraint` 就把 tracker 的位置在 **4 个角 + 自身** 之间做加权平均：手碰到哪个方向、就把那一项权重抽掉，tracker 的位置随之偏向另一侧。**tracker 的位置因此是"接触图案的函数"**，这就是"Tracker Blend Tree 算位置"。

## 最终得到的三点

三组 tracker 的最终位置，就是 [03-旋转链路](./03-旋转链路.md) 里 AimIK 用来定坐标架的三个点；而系统是否认为这三点"收敛有效"，由 [05-TrackerSync与TrackersCheck](./05-TrackerSync与TrackersCheck.md) 判定。

## 相关文档

- 旋转链路：[03-旋转链路](./03-旋转链路.md)
- 抓握校验：[05-TrackerSync与TrackersCheck](./05-TrackerSync与TrackersCheck.md)
