namespace Breakdawn.Core
{
    public interface IFactory<out T>
    {
        T Get();
    }
}