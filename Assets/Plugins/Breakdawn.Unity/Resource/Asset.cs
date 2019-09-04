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
        private int _refCount;

        public AssetInfo Info { get; }

        internal virtual Object Resource
        {
            get
            {
                LastUseTime = DateTime.Now;
                RefCount++;
                return resource != null ? resource : ResourceManager.LoadObject(Info, IsSprite);
            }
        }

        internal DateTime LastUseTime { get; set; }

        public bool IsSprite { get; }

        internal int RefCount
        {
            get => _refCount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException($"引用计数不能小于0，name:{Info.assetName}，refCount:{value}");
                }

                _refCount = value;
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