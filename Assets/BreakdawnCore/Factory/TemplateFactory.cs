namespace Breakdawn.Factory
{
	public abstract class TemplateFactory<T> : IFactory<T>
	{
		public abstract T Create();
	}
}
