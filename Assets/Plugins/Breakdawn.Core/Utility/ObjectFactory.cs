using System;

namespace Breakdawn.Core
{
    public class ObjectFactory<T> : IFactory<T>
    {
        private readonly Func<T> _func;

        public ObjectFactory(Func<T> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public T Get()
        {
            return _func();
        }
    }
}