namespace Breakdawn.Factory
{
	public abstract class TemplateFactory<T> : IFactory<T>
	{
		protected T template;
		protected string name;

		public string Name { get => name; }

		public abstract T Create();
	}
}
