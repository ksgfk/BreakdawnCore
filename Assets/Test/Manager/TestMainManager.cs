using Breakdawn.Core;
using UnityEngine;

namespace Breakdawn.Test
{
	[Singleton]
	public class TestMainManager : TemplateMainManager<TestMainManager>
	{
		private void Start()
		{
			TestUIManager.Instance.LoadPanel("Image1", 1);
			//TestUIManager.Instance.LoadPanel("Image1", 2);
		}

		protected override void AwakeLunchDevelop()
		{
			Debug.Log("Dev");
		}

		protected override void AwakeLunchProduction()
		{
			Debug.Log("Pro");
		}

		protected override void AwakeLunchTest()
		{
			Debug.Log("Test");
		}
	}
}
