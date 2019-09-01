using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class AsyncTest : MonoBehaviour
    {
        private AsyncAssetRequest _request;
        private UnityObjectInfo<GameObject> _prefab;
        private UnityObjectInfo<GameObject> _test;
        private GameObject _real;

        private bool _init;

        private UnityObjectInfo<Sprite> _sprite;

        private void Start()
        {
            ResourceManager.Instance.Init(Paths.AssetConfig, Paths.Assets, Paths.StreamingAssets);
            ResourceManager.Instance.InitAsync(this);
            _request = ResourceManager.Instance.GetAssetAsync(request => request.GetAsset(ref _prefab),
                "Attack.prefab",
                Paths.Assets,
                Paths.Prefabs);

            ResourceManager.Instance.GetAssetAsync(request => request.GetAsset(ref _sprite),
                "ModelBoomOceanSoulOre.png",
                Paths.Assets,
                Paths.Models,
                "benghuai");
        }

        private void Update()
        {
            if (_init)
            {
                return;
            }

            if (_request.IsDone)
            {
                _real = Instantiate(_prefab.obj);
                _init = true;
            }
            else
            {
                Debug.Log(_request.Progress);
            }
        }

        public void OnButtonClick()
        {
            Destroy(_real);
            ResourceManager.Instance.RecycleAsset(ref _prefab);
            var a = ResourceManager.Instance;
        }

        public void OnAnotherButtonClick()
        {
            _request = ResourceManager.Instance.GetAssetAsync(request => request.GetAsset(ref _prefab),
                "Attack.prefab",
                Paths.Assets,
                Paths.Prefabs);
            _real = Instantiate(_prefab.obj);
            var a = ResourceManager.Instance;
        }
    }
}