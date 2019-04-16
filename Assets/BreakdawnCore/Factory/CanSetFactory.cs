using System;

namespace Breakdawn.Factory
{
	public class CanSetFactory<T> : TemplateFactory<T> where T : new()
	{
		private Func<T> t;

		public CanSetFactory(Func<T> setTemplate)
		{
			template = setTemplate();
			t = setTemplate;
		}

		public override T Create()
		{
			return t();
		}

		public T SetTemplate(Func<T> set)
		{
			template = set();
			t = set;
			return template;
		}
	}
}
