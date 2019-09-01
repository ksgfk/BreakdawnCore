using System;
using System.Collections.Generic;
using Breakdawn.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    public class AsyncAssetRequest : Asset
    {
        internal List<ResourceManager.LoadComplete> callbacks = new List<ResourceManager.LoadComplete>();

        public bool IsDone => Request?.isDone ?? false;
        public float Progress => Request?.progress ?? 0;
        public string AssetName => Info.assetName;
        internal AssetBundleRequest Request { get; set; }
        internal AssetBundle Bundle { get; set; }

        internal override Object Resource
        {
            get => Request.isDone ? Request.asset : null;
            set => throw new InvalidOperationException($"异步加载不可以设置资源");
        }

        public AsyncAssetRequest(AssetInfo info) : base(info)
        {
        }

        /// <summary>
        /// 建议只在ResourceManager.LoadComplete委托内调用该方法
        /// </summary>
        /// <param name="obj">资源本体</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void GetAsset<T>(ref UnityObjectInfo<T> obj) where T : Object
        {
            if (!IsDone)
            {
                return;
            }

            obj = new UnityObjectInfo<T>(Utility.TypeCast<Object, T>(Resource), Info.assetName);
        }
    }
}