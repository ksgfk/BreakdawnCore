using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Unity
{
    public class AsyncLoadRequest
    {
        public bool IsDone => Request?.isDone ?? false;
        public float Progress => Request?.progress ?? 0;
        public string AssetName => Asset.assetInfo.assetName;
        internal AssetBundleRequest Request { get; set; }
        internal AssetBundle Bundle { get; set; }
        internal Asset Asset { get; }

        internal readonly List<ResourceManager.LoadComplete> callbacks =
            new List<ResourceManager.LoadComplete>();

        public AsyncLoadRequest(Asset asset)
        {
            Asset = asset;
        }

        public void GetAsset<T>(ref UnityObjectInfo<T> obj) where T : Object
        {
            if (!IsDone)
            {
                return;
            }

            var res = Asset.asset as T;
            obj = new UnityObjectInfo<T>(res, Asset.assetInfo.assetName);
        }
    }
}