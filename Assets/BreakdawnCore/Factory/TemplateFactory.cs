namespace Breakdawn.Factory
{
	public abstract class TemplateFactory<T> : IFactory<T>
	{
		protected T template;

		public abstract T Create();
	}
}
