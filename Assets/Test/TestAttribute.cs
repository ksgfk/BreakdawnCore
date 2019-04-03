using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TestAttribute : Attribute
	{
		private int id;

		public int Id => id;

		public TestAttribute(int id)
		{
			this.id = id;
		}
	}
}
