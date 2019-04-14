using System;

namespace Breakdawn.Factory
{
	public class CanSetFactory<T> : TemplateFactory<T> where T : ICloneable
	{
		private Func<T> setTemplate;

		public CanSetFactory(T template, Func<T> setTemplate)
		{
			this.template = template;
			this.setTemplate = setTemplate;
		}

		public override T Create()
		{
			return (T)template.Clone();
		}

		public void SetTemplate(Func<T> set)
		{
			setTemplate = set;
			template = setTemplate();
		}
	}
}
