using System;

namespace Breakdawn.Core
{
	public class EntrustFactory<T> : IFactory<T> where T : new()
	{
		private Func<T> template;

		public EntrustFactory(Func<T> setTemplate)
		{
			template = setTemplate;
		}

		public T Create()
		{
			return template();
		}

		public T SetTemplate(Func<T> set)
		{
			template = set;
			return set();
		}
	}
}
