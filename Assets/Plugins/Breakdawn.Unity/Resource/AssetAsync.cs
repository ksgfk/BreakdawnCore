using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    /// <summary>
    /// 异步资源请求
    /// </summary>
    public class AssetAsync : Asset
    {
        internal bool isDone;
        internal LoadComplete callbacks;

        public bool IsDone => Request?.isDone ?? isDone;

        public float Progress
        {
            get
            {
                if (!resource)
                {
                    return 1;
                }

                return Request?.progress ?? 0;
            }
        }

        public string AssetName => Info.assetName;
        internal AssetBundleRequest Request { get; set; }

        internal virtual Object Resource
        {
            get
            {
                if (resource)
                {
                    return resource;
                }

                if (Request == null)
                {
                    return null;
                }

                return Request.isDone ? Request.asset : null;
            }
        }

        public AssetAsync(AssetInfo info) : base(info)
        {
        }

        /// <summary>
        /// 建议只在LoadComplete委托内调用该方法
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        [CanBeNull]
        public override T GetAsset<T>()
        {
            if (!IsDone)
            {
                return null;
            }

            LastUseTime = DateTime.Now;
            refCount += 1;
            return resource as T;
        }
    }
}