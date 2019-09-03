using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class AsyncTest : MonoBehaviour
    {
        private LoadAssetAsyncRequest _request;
        private UnityObjectInfo<GameObject> _prefab;
        private UnityObjectInfo<GameObject> _test;
        private GameObject _real;

        private bool _init;

        private UnityObjectInfo<Sprite> _sprite;

        private void Start()
        {
            ResourceManager.Instance.Init(Paths.AssetConfig, Paths.Assets, Paths.StreamingAssets);
            ResourceManager.Instance.InitAsync(this);
//            _request = ResourceManager.Instance.GetAssetAsync(request => request.GetAsset(ref _prefab),
//                "Attack.prefab",
//                Paths.Assets,
//                Paths.Prefabs);

//            ResourceManager.Instance.GetAssetAsync(request => request.GetAsset(ref _test),
//                "Attack.prefab",
//                Paths.Assets,
//                Paths.Prefabs);

//            ResourceManager.Instance.CacheAssetAsync($"{Paths.Assets}/{Paths.Prefabs}/Attack.prefab");
            ObjectManager.Instance.Init(gameObject);
            ObjectManager.Instance.InitPool($"{Paths.Assets}/{Paths.Prefabs}/Attack.prefab", 10);
        }

//        private void Update()
//        {
//            if (_init)
//            {
//                return;
//            }
//
//            Debug.Log(_request.Progress);
//
//            if (!_request.IsDone)
//            {
//                return;
//            }
//
//            _real = Instantiate(_prefab.obj);
//            _init = true;
//            _request = null;
//            var a = ResourceManager.Instance;
//        }

        public void OnButtonClick()
        {
            Destroy(_real);
            ResourceManager.Instance.RecycleAsset(ref _prefab);
            var a = ResourceManager.Instance;
//            var a = ResourceManager.Instance;
        }

        public void OnAnotherButtonClick()
        {
            _request = ResourceManager.Instance.GetAssetAsync(request => request.GetAsset(ref _prefab),
                "Attack.prefab",
                Paths.Assets,
                Paths.Prefabs);
            _init = false;

            var a = ResourceManager.Instance;
        }
    }
}