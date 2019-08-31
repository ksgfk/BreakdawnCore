using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class AsyncTest : MonoBehaviour
    {
        private AsyncAssetRequest _request;
        private UnityObjectInfo<GameObject> _prefab;
        private GameObject _real;

        private bool _init;

        private void Start()
        {
            AssetBundleManager.Instance.Init(Paths.AssetConfig, Paths.Assets, Paths.StreamingAssets);
            ResourceManager.Instance.Init(this);
            _request = ResourceManager.Instance.GetAssetAsync(request => request.GetAsset(ref _prefab),
                "Attack.prefab",
                Paths.Assets,
                Paths.Prefabs);
        }

        private void Update()
        {
            if (_init)
            {
                return;
            }

            if (_request.IsDone)
            {
                var a = AssetBundleManager.Instance;
                var b = ResourceManager.Instance;
                _real = Instantiate(_prefab.obj);
                _init = true;
            }
            else
            {
                Debug.Log(_request.Progress);
            }
        }
    }
}