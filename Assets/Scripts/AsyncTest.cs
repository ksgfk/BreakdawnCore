using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class AsyncTest : MonoBehaviour
    {
        private AsyncLoadRequest _request;
        private UnityObjectInfo<GameObject> _prefab;
        private GameObject _real;

        private bool _init;

        private void Start()
        {
            AssetBundleManager.Instance.Init("Assets/StreamingAssets");
            ResourceManager.Instance.Init(this);
            _request = ResourceManager.Instance.GetAssetAsync("Assets/Prefabs/Attack.prefab",
                request => request.GetAsset(ref _prefab));
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