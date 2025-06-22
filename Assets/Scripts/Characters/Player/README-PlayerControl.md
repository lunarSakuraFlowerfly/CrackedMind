# 玩家控制管理系统使用说明

## 概述

玩家控制管理系统用于在特定情况下（如对话、查看物品信息、交互等）禁用玩家的移动和操作能力，使游戏体验更加沉浸。

## 组件说明

### PlayerControlManager 组件

这是玩家控制管理系统的核心组件，负责启用和禁用玩家的移动控制。

#### 属性：
- `playerMovementScript`: 引用玩家主移动脚本
- `additionalControlScripts`: 其他需要禁用的控制脚本数组
- `disablePhysics`: 是否在禁用控制时同时禁用物理系统

### 与交互系统的集成

玩家控制管理系统已与以下系统集成：

1. **对话系统**：玩家在对话期间不能移动
2. **物品交互系统**：玩家在查看物品信息时不能移动
3. **被动交互系统**：玩家在触发被动交互时不能移动

## 安装步骤

1. 将 `PlayerControlManager.cs` 脚本添加到玩家角色对象上
2. 在Inspector中设置以下引用：
   - `Player Movement Script`: 拖放主要的玩家移动控制脚本
   - `Additional Control Scripts`: 添加其他需要禁用的控制脚本（如跳跃、攻击等）
3. 确保玩家对象有 "Player" 标签

## 使用方法

### 在脚本中手动控制

你可以在任何脚本中使用以下代码控制玩家移动：

```csharp
// 禁用玩家控制
PlayerControlManager.Instance.DisableControl();

// 启用玩家控制
PlayerControlManager.Instance.EnableControl();

// 检查玩家控制状态
bool canPlayerMove = PlayerControlManager.Instance.IsControlEnabled();
```

### 通过事件监听控制状态变化

你可以监听控制状态变化的事件：

```csharp
private void OnEnable()
{
    PlayerControlManager.Instance.OnControlDisabled += HandlePlayerControlDisabled;
    PlayerControlManager.Instance.OnControlEnabled += HandlePlayerControlEnabled;
}

private void OnDisable()
{
    PlayerControlManager.Instance.OnControlDisabled -= HandlePlayerControlDisabled;
    PlayerControlManager.Instance.OnControlEnabled -= HandlePlayerControlEnabled;
}

private void HandlePlayerControlDisabled()
{
    // 处理玩家控制被禁用的逻辑
    Debug.Log("玩家控制已禁用");
}

private void HandlePlayerControlEnabled()
{
    // 处理玩家控制被启用的逻辑
    Debug.Log("玩家控制已启用");
}
```

## 自定义扩展

你可以根据项目需求自定义扩展控制系统：

1. 修改 `DisableControl()` 和 `EnableControl()` 方法，添加更多需要禁用的组件
2. 在其他系统中使用 `PlayerControlManager.Instance` 控制玩家移动

## 排错指南

如果玩家控制没有正常切换：

1. 检查 PlayerControlManager 是否已正确添加到玩家对象上
2. 确认已设置 playerMovementScript 引用
3. 确认玩家对象有 "Player" 标签
4. 检查控制台是否有相关警告或错误信息 