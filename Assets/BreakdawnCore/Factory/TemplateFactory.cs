namespace Breakdawn.Core
{
	public abstract class TemplateFactory<T> : IFactory<T>
	{
		public abstract T Create();
	}
}
