# CameraController 开发文档

## 功能概述
CameraController是一个用于Unity 2D游戏的相机控制组件，提供平滑的相机跟随功能。主要用于跟随玩家角色，并支持边界限制和平滑移动效果。

## 核心功能
1. **目标跟随**
   - 自动跟随指定目标（默认为玩家）
   - 支持自定义偏移量
   - 可动态切换跟随目标

2. **平滑移动**
   - 可配置的跟随速度
   - 平滑插值移动
   - 可开关平滑跟随功能

3. **边界限制** 
   - 可设置相机移动的X轴和Y轴边界
   - 动态边界调整
   - 可启用/禁用边界限制

## 参数配置

### 跟随设置
```csharp
[Header("跟随设置")]
[SerializeField] private Transform target;        // 跟随目标
[SerializeField] private float followSpeed = 5f;  // 跟随速度
[SerializeField] private Vector3 offset;          // 相机偏移量
[SerializeField] private bool useSmoothFollow;    // 是否使用平滑跟随
[SerializeField] private float smoothTime = 0.3f; // 平滑时间
```

### 边界设置
```csharp
[Header("边界设置")]
[SerializeField] private bool useBounds;          // 是否启用边界
[SerializeField] private float minX = -10f;       // X轴最小值
[SerializeField] private float maxX = 10f;        // X轴最大值
[SerializeField] private float minY = -10f;       // Y轴最小值
[SerializeField] private float maxY = 10f;        // Y轴最大值
```

## 公共方法

### SetTarget
```csharp
public void SetTarget(Transform newTarget)
```
设置新的跟随目标。
- 参数：
  - newTarget：新的跟随目标Transform

### SetBounds
```csharp
public void SetBounds(float minX, float maxX, float minY, float maxY)
```
设置相机移动边界。
- 参数：
  - minX：X轴最小值
  - maxX：X轴最大值
  - minY：Y轴最小值
  - maxY：Y轴最大值

### ClearBounds
```csharp
public void ClearBounds()
```
清除相机边界限制。

## 使用示例

### 基础设置
```csharp
// 1. 添加组件到主相机
Camera mainCamera = Camera.main;
CameraController controller = mainCamera.gameObject.AddComponent<CameraController>();

// 2. 设置跟随目标
controller.SetTarget(playerTransform);

// 3. 设置边界
controller.SetBounds(-10f, 10f, -10f, 10f);
```

### 运行时调整
```csharp
// 切换跟随目标
controller.SetTarget(newTarget);

// 更新边界
controller.SetBounds(newMinX, newMaxX, newMinY, newMaxY);

// 移除边界限制
controller.ClearBounds();
```

## 最佳实践

1. **相机设置**
   - 确保相机的Projection设置为"Orthographic"用于2D游戏
   - 调整相机的Size以适应游戏视野需求

2. **性能优化**
   - 使用LateUpdate进行相机更新
   - 避免频繁更改跟随目标
   - 合理设置smoothTime值（建议0.1-0.3之间）

3. **边界设置**
   - 根据关卡大小设置适当的边界值
   - 考虑相机的正交大小when设置边界
   - 预留适当的边界缓冲区

## 注意事项

1. **初始化**
   - 组件会自动查找带有PlayerMovement的物体作为默认目标
   - 如果未找到目标，会在控制台输出警告

2. **性能考虑**
   - 使用LateUpdate确保在所有更新后执行
   - 平滑移动可能在低性能设备上造成轻微延迟

3. **边界限制**
   - 边界值应考虑相机的正交大小
   - 确保minX < maxX，minY < maxY

## 调试建议

1. 启用Unity的Scene视图Gizmos
2. 观察相机移动是否平滑
3. 检查边界限制是否正确生效
4. 监控控制台输出的警告信息

## 扩展建议

1. 添加相机震动效果
2. 实现区域切换过渡
3. 添加缩放功能
4. 实现多目标跟随
5. 添加前瞻功能