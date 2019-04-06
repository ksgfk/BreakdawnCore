using System;

namespace Breakdawn.Test
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
