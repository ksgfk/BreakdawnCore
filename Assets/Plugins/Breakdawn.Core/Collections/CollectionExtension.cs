using System.Collections.Generic;

namespace Breakdawn.Core
{
    public static class CollectionExtension
    {
        public static bool IsEmpty<T>(this Queue<T> queue)
        {
            return queue.Count == 0;
        }

        public static bool TryDequeue<T>(this Queue<T> queue, out T value)
        {
            if (queue.IsEmpty())
            {
                value = default;
                return false;
            }

            value = queue.Dequeue();
            return true;
        }

        public static bool IsEmpty<T>(this LinkedList<T> linkedList)
        {
            return linkedList.Count == 0;
        }
    }
}