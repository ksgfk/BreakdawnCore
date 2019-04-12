# EventBus/事件总线

## Description/说明
创建新委托集合（[TempletEvents<T, ACT>][1]）的时候的时候，调用EventBus的`EventBus.Instance.CreateEvents<T, ACT>();`建议T是个枚举，ACT隶属于Delegate，需要传入委托集合string类型的名字（可以自己创建个枚举）。

创建好集合后，可以直接调用集合的`Register();`或者使用`EventBus.Instance.AddEvent();`，需要传入3个参数。

使用委托集合的`GetEvent();`就可以获取委托，直接执行就可以了。

使用委托集合的`Remove();`可以删除一个委托。使用`EventBus.Instance.RemoveEvents();`可以删除委托集合（如果你没有把它作为变量保存，它就真的没了）。

MonoMessage这个类实现了可挂载脚本的事件，如果在OnDestory阶段需要做一些操作，需要写在`OnBeforeDestroy();`方法中，避免直接使用OnDestory造成的一些问题。

## Example/例子
```
private void Start()
{
	EventBus.Instance.CreateEvents<MyEventTest, Action>("myNoParma").Register(MyEventTest.A, () => { Debug.Log("233"); });
	EventBus.Instance.CreateEvents<MyEventTest, Action<int>>("myOneParma").Register(MyEventTest.A, Hello);
	EventBus.Instance.CreateEvents<MyEventTest, Func<int>>("myNoParmaR").Register(MyEventTest.A, Hello);
}

private void Update()
{
	var a = EventBus.Instance.GetEvents<MyEventTest, Action>("myNoParma").GetEvent(MyEventTest.A);
	a();
	var b = EventBus.Instance.GetEvents<MyEventTest, Action<int>>("myOneParma").GetEvent(MyEventTest.A);
	b(233);
	var c = EventBus.Instance.GetEvents<MyEventTest, Func<int>>("myNoParmaR").GetEvent(MyEventTest.A);
	Debug.Log(c());
}

private void Hello(int a)
{
	Debug.Log($"HEllo,{a}");
}

private int Hello()
{
	return 666;
}
```

将该脚本挂到物体上，启动，unity控制台就会疯狂刷233、HEllo,233、666啦。

```
public class MonoEvent : MonoMessage<Action<object>>
{
	private void Start()
	{
		Register("Hello", Hello);
		StringPool.MonoEventToString = ToString();
		this.InvokeCoroutine(() => { Destroy(gameObject); }, 5F);
	}

	private void Hello(object a)
	{
		Debug.Log($"跨脚本通信:{a}");
	}

	public override void OnBeforeDestroy() { }
}

public static class StringPool
{
	public static string MonoEventToString;
}
```
将MonoEvent挂载到物体身上，然后在需要通信的地方写
```
var d = EventBus.Instance.GetEvents<string, Action<object>>(StringPool.MonoEventToString).GetEvent("Hello");
d(233);
```

就可以在控制台输出`跨脚本通信:233`了。

[1]:https://github.com/ksgfk/BreakdawnCore/blob/master/Assets/BreakdawnCore/Event/TempletEvents.cs