# PKUMakerVirtualSpace

123

---
## 命名空间`PKU.Item`

<br/>

### `ItemClass.cs`

包含两个不继承`MonoBehaviour`的类

`ItemData类`, 用于记录编号为itemID的物体的详细信息. 包括物体名字, 物体描述, 物体预制体等. 在名为`ItemDataList_SO`的`Scriptable Object`中填写信息.

`MapItem类`, 用于记录场景中的物体信息. 包括物体ID, 物体位置.

<br/>

### `ItemController类`

挂载在场景中所有物体预制体的基类预制体`ItemBase`上. 负责**单个**物体的初始化, 物体的拾起等.

<br/>

### `ItemGlobalManager类`

单例模式, 调用时需要用`ItemGlobalManager.Instance`的写法.

可以在场景中生成物体, 负责**场景中**物体的初始化, 还能根据物体ID返回物体详细信息.

<br/>

### `NetworkObjectPool对象池`

挂载在`NetworkObjectPool`空物体下. 需要在场景中生成的预制体记得在此添加. 添加的预制体需要有`NetworkObject`组件.

<br/>

### `ItemBase基类预制体`

包含`ItemController`脚本, `ItemGlobalManager`在对其初始化时, 会根据`itemID`给它分配对象池中的一个预制体作为子物体. 例如, 如果`itemID = 1001`, 在`ItemDataList_SO`中对应的`itemPrefab`是一个`ItemCube`, 那么就从对象池中取一个`ItemCube`(一定也是`NetworkObject`)作为`ItemBase`的子物体.

如果要销毁`ItemBase`, 要先销毁`ItemBase`下的子物体, 让子物体先返回对象池.

<br/>

### `PlayerPickUpItem类`

挂载在Player下, 负责碰撞检测. 如果碰撞发生, 调用`ItemController`中的`PickUpItem`方法, 得到`itemData`详细信息.