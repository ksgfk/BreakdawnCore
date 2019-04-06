using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	[Test(1)]
	public class MyTestClass
	{
		public int Id { get; }

		public MyTestClass(int id)
		{
			Id = id;
		}
	}
}
