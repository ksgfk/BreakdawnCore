using System;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    /// <summary>
    /// 资源实例
    /// </summary>
    public class Asset
    {
        internal Object resource;
        protected int refCount;

        public AssetInfo Info { get; }
        internal DateTime LastUseTime { get; set; }
        public bool IsSprite { get; }

        internal int RefCount
        {
            get => refCount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException($"引用计数不能小于0，name:{Info.assetName}，refCount:{value}");
                }

                refCount = value;
            }
        }

        internal Asset(AssetInfo info)
        {
            Info = info;
            if (info.assetName.EndsWith(".png") ||
                info.assetName.EndsWith(".jpg") ||
                info.assetName.EndsWith(".bmp"))
            {
                IsSprite = true;
            }
            else
            {
                IsSprite = false;
            }

            refCount = 0;
        }

        public virtual T GetAsset<T>() where T : Object
        {
            var result = resource ? resource : ResourceManager.LoadObject(Info, IsSprite);
            if (!result)
            {
                throw new ArgumentException();
            }

            resource = result;
            LastUseTime = DateTime.Now;
            refCount += 1;
            return result as T;
        }

        public override bool Equals(object obj)
        {
            return obj is Asset asset && Info.Equals(asset.Info);
        }

        public override int GetHashCode()
        {
            return Info.GetHashCode();
        }
    }
}