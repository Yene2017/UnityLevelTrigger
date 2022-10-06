# LevelTrigger

基于Unity游戏引擎的触发器简易模块，可以自定义触发器形状和触发逻辑代替Unity自带基于物理的触发器组件。

触发器形状包含

- 球体
- 不可旋转的圆柱体
- 不可旋转的多边形棱柱（支持凹多边形）

触发逻辑可以编写基于MonoBehaviour的组件的派生类，配合编辑器操作编辑参数，具体使用在游戏内有两种方式。

1.派生类直接用于游戏逻辑。

2.派生类作为工具导出编辑数据，在游戏内通过TriggerManager创建对象。

- 通过 Container 的静态创建函数构造触发器形状

- 通过 CreateLayer(int layerId, Container[] array, bool additive) 设定一组相同业务逻辑的触发器

- 通过 RegisterListener(int layerId, Transform transform)  注册监听一组触发器

- 通过 RegisterCallback(actionEnter, actionExit, actionChanged, actionLayerRemove, actionUnregister) 监听触发器系统更新形状和注册位置Overlap关系变化传递出的消息

  