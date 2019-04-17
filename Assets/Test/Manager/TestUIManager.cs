using Breakdawn.Factory;
using Breakdawn.Manager;
using Breakdawn.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	[Singleton]
	public class TestUIManager : TemplateUIManager<TestUIManager>
	{
		private TestUIManager() : base(new ResourceFactory<GameObject>("UIPanel")) { }
	}
}
