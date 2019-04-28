namespace Breakdawn.Core
{
	[System.Obsolete("没有用的")]
	public abstract class TemplateFactory<T> : IFactory<T>
	{
		public abstract T Create();
	}
}
