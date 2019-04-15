using System;

namespace Breakdawn.Factory
{
	public class CanSetFactory<T> : TemplateFactory<T> where T : new()
	{
		public CanSetFactory(Func<T> setTemplate)
		{
			template = setTemplate();
		}

		public override T Create()
		{
			return template;
		}

		public void SetTemplate(Func<T> set)
		{
			template = set();
		}
	}
}
