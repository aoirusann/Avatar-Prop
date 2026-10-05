---
description: 讲解 Cull 物体是什么、由什么驱动，并说明它是本地/同步的包围盒辅助体，而不是按观察者相机距离做的剔除。
---

# Cull

## 它是什么（实测）

| 项 | 值 |
|---|---|
| 网格 | 1×1×1 的 `Cube` |
| 位置 | 根物体原点 (0,0,0) |
| 材质 | 无（`sharedMaterial = null`），所以玩家看不见 |
| 层 | `UiMenu`（Layer 12） |
| 组件 | `MeshFilter`、`MeshRenderer`、一个 `VRCScaleConstraint`（源 = `000`，scale=1） |

## 由什么驱动

状态机用动画切换它的 **`GlobalWeight`**：

- `Off` / `Off 1` 状态：`GlobalWeight = 0`
- 其余所有状态：`GlobalWeight = 1`

也就是它随"系统开/关"一起启用或停用。

## 它不是"按观察者距离剔除"

- Avatar 上的逻辑**拿不到"观察者到道具的距离"**，所以 `Cull` 是**本地 + 同步参数驱动**的，所有客户端算出来一致。你"看着正常、别人看着缩小"这种分视角效果**不是它做的**（那是 VRChat 游戏自带的 avatar 远近剔除，与此无关）。
- 它更像一个**不可见、钉在世界锚点上的包围盒/剔除辅助体**：给 VRChat 一个稳定的、与世界锚点绑定的渲染体，用来影响对 avatar/道具的包围盒与剔除判定，并随系统开关切换。它精确触发的剔除行为属于推断，但"本地、非分观察者"是确定的。

## 真正的"距离"逻辑另有其物

系统里唯一和距离有关的是：

```
Distance check (receiver, tag = Avatarprop.DistanceCheck, 半径 2m, 挂在 Bone base)
Distance checker (sender, 跟随 Resyncy boi)
  → 超阈值 ⇒ DistanceTooFar ⇒ 状态机走 Reset root bone pos
```

它测的是**系统内部两点（手柄底座 / 抓取参考点）之间的距离**，和"谁在看、离多远"无关。

## 相关文档

- 状态机：[状态机总览](./状态机总览.md)
- 参数与标签：[参数与接触标签](./参数与接触标签.md)
