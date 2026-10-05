---
description: 讲解 Delay position 这个恒等于抓取参考点的中转物体的作用，以及它为何被用来打断约束回路。
---

# Delay position

## 它是什么

```
Delay position   （root 的直接子物体）
  VRCPositionConstraint 源 = Resyncy boi : w1
  enabled = true，且——所有动画里都没有任何曲线动过它
```

所以它**恒等于抓取参考点 `Resyncy boi` 的位置**，自己不做任何加工，也不参与抓取时的显示。

## 它在哪条链路上

```
抓取时（实时）：   道具 Position 直接跟 Resyncy boi

锁定链路：         Resyncy boi
                       │
                       ▼
                  Delay position          ← 就是它（纯中转）
                       │  w1
                       ▼
                  Save position (+ Hand 0.6)
                       │  w1
                       ▼
                  道具 Position（放下时切到这个来源）
```

它是 `Save position`（松手时被冻结的快照，见 [11-锁存机制](./11-锁存机制.md)）的**输入端**。

## 为什么叫 "Delay"

因为这里存在一条约束**回路**，需要靠"多隔一层"来打断：

- `Bone base` 的约束源包含 `Position`（放下/复位时动画会把权重切过去），也就是**手柄底座反过来跟道具的位置**。
- 放下时：`Position` ← `Save position` ← `Delay position` ← `Resyncy boi` ← `Bone`(PhysBone) ← `Bone base` ← 又回到 `Position`。

于是形成环路：`Position → Bone base → Bone → Resyncy boi → … → Position`。VRChat 的约束解算按依赖顺序求值、每多一层就滞后一帧；直接成环会有问题。插入 `Delay position` 制造**一帧延迟**，把回路拆开，让解算稳定——这正是它叫 `Delay` 的原因。

## 相关文档

- 锁存机制：[11-锁存机制](./11-锁存机制.md)
- 位移链路：[02-位移链路](./02-位移链路.md)
