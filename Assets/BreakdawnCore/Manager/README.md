# Managers/管理者

## Description/说明
* 目前已实现简单的UI管理者（TemplateUIManager）。
* 主管理者正在完善（TemplateMainManager）。

## Example/例子
UI管理：
```
[Singleton]
public class TestUIManager : TemplateUIManager<TestUIManager>
{
	private TestUIManager() { }

	protected override string SetUIPrefabPath()
	{
		return "UIPanel";
	}
}
```
然后在Resources/UIPanel里放一个名为Image的UI Image

主管理：
```
public class TestMainManager : TemplateMainManager
{
	private void Start()
	{
		TestUIManager.Instance.LoadPanel("Image");
	}

	protected override void LunchDevelop() { }

	protected override void LunchProduction() { }

	protected override void LunchTest() { }
}
```
把主管理挂载到游戏物体上，Inspector面板中该脚本下会有个Environment选项，可以选择运行环境。
启动后可以看到Canvas下生成了Image(Clone)

## BE CAREFUL/注意
* UI管理必须打上SingletonAttribute特性
* UI管理的构造函数必须是无参且私有的，因为会检查单例
* 主管理需要挂载到游戏物体上
* 主管理的Environment值为Production时不会检查单例正确性