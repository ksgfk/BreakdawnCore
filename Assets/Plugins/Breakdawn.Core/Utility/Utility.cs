using System;

namespace Breakdawn.Core
{
    public static class Utility
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="obj">将要转换的实例</param>
        /// <typeparam name="T">将要转换的类型</typeparam>
        /// <typeparam name="TR">目标类型</typeparam>
        /// <returns>转换后结果</returns>
        /// <exception cref="InvalidCastException">转换失败时抛出</exception>
        public static TR TypeCast<T, TR>(T obj)
        {
            if (obj is TR result)
            {
                return result;
            }

            throw new InvalidCastException($"类型错误，T:{typeof(T).FullName}，应为{obj.GetType().FullName}");
        }
    }
}