using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    public struct UnityObjectInfo<T> where T : Object
    {
        /// <summary>
        /// 资源
        /// </summary>
        public readonly T obj;

        /// <summary>
        /// 该资源的原始名称
        /// </summary>
        public readonly string rawName;

        public UnityObjectInfo(T obj, string rawName)
        {
            this.obj = obj;
            this.rawName = rawName;
        }

        public bool IsValid()
        {
            return obj != null && !string.IsNullOrEmpty(rawName);
        }
    }
}