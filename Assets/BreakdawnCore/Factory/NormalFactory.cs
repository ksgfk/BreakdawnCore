namespace Breakdawn.Factory
{
	public class NormalFactory<T> : TemplateFactory<T> where T : new()
	{
		public NormalFactory(T template)
		{
			this.template = template;
		}

		public override T Create()
		{
			return new T();
		}
	}
}
