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
        internal bool isDone = false;
        internal LoadComplete callbacks;

        public bool IsDone => Request?.isDone ?? isDone;

        public float Progress
        {
            get
            {
                if (resource != null)
                {
                    return 1;
                }

                return Request?.progress ?? 0;
            }
        }

        public string AssetName => Info.assetName;
        internal AssetBundleRequest Request { get; set; }

        internal override Object Resource
        {
            get
            {
                if (resource != null)
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
        public T GetAsset<T>() where T : Object
        {
            if (!IsDone)
            {
                return null;
            }

            LastUseTime = DateTime.Now;
            RefCount++;
            return Resource as T;
        }
    }
}