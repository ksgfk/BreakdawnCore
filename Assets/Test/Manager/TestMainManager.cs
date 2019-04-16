﻿using Breakdawn.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	public class TestMainManager : TemplateMainManager
	{
		private void Start()
		{
			TestUIManager.Instance.LoadPanel("Image");
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