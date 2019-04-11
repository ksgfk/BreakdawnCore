# EventBus/事件总线

## Description/说明
目前已实现[无参委托][2]和[单参委托][3]，若想实现更多参数委托，可以继承[AbstractEvents][1]和[IEventType][4]接口，具体实现可以参照已实现的两种。

创建新委托的时候，首先需要一个实现了[IEventType][4]接口的类，建议直接继承[AbstractEventsNo][2]和[AbstractEventsOne][3]~~别吐槽类名~~。然后使用EventBus类下的[Add()][5]静态方法。执行委托的时候，使用[Execute()][6]。

## Example/例子
```
public enum MyEventTest { A }

public class TestMyEvent : AbstractEventsNo<MyEventTest> { }

public class HangOnObject : MonoBehaviour
{
	private TestMyEvent myEvent;

	private void Start()
	{
		myEvent = new TestMyEvent();
		EventBus.Add(myEvent, MyEventTest.A, () => { Debug.Log("C# is the best language!"); });
	}

	private void Update()
	{
		EventBus.Execute(myEvent, MyEventTest.A);
	}
}
```

将HangOnObject挂到物体上，启动，unity控制台就会疯狂刷文字啦。

[1]:https://github.com/ksgfk/BreakdawnCore/blob/master/Assets/BreakdawnCore/Event/AbstractEvents.cs
[2]:https://github.com/ksgfk/BreakdawnCore/blob/master/Assets/BreakdawnCore/Event/AbstractEventsNo.cs
[3]:https://github.com/ksgfk/BreakdawnCore/blob/master/Assets/BreakdawnCore/Event/AbstractEventsOne.cs
[4]:https://github.com/ksgfk/BreakdawnCore/blob/master/Assets/BreakdawnCore/Event/IEventType.cs
[5]:https://github.com/ksgfk/BreakdawnCore/blob/9363590061472d376bf2ec8d676b274b6da29df6/Assets/BreakdawnCore/Event/EventBus.cs#L8
[6]:https://github.com/ksgfk/BreakdawnCore/blob/9363590061472d376bf2ec8d676b274b6da29df6/Assets/BreakdawnCore/Event/EventBus.cs#L18