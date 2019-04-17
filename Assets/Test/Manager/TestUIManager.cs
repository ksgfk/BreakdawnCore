using Breakdawn.Core;
using UnityEngine;

namespace Breakdawn.Test
{
	[Singleton]
	public class TestUIManager : TemplateUIManager<TestUIManager>
	{
		private TestUIManager() : base(new ResourceFactory<GameObject>("UIPanel")) { }
	}
}
