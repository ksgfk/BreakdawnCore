using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class AsyncTest : MonoBehaviour
    {
        private AssetAsync _async;
        private AssetAsync _test;
        private GameObject _prefab;
        private GameObject _t;
        private GameObject _ins;

        private void Awake()
        {
            ResourceManager.Instance.Init(Paths.AssetConfig, Paths.Assets, Paths.StreamingAssets);
            ResourceManager.Instance.InitAsync(this);
        }

        private void Start()
        {
            _async = ResourceManager.Instance.GetAssetAsync($"{Paths.Assets}/{Paths.Prefabs}/Attack.prefab"
                , request => _prefab = request.GetAsset<GameObject>());
            _test = ResourceManager.Instance.GetAssetAsync($"{Paths.Assets}/{Paths.Prefabs}/Attack.prefab"
                , request => _t = request.GetAsset<GameObject>());
        }

        public void OnButtonClick()
        {
            if (_prefab != null)
            {
                _ins = Instantiate(_prefab);
            }
            else
            {
                if (_async != null)
                {
                    Debug.Log(_async.Progress);
                }
                else
                {
                    _async = ResourceManager.Instance.GetAssetAsync($"{Paths.Assets}/{Paths.Prefabs}/Attack.prefab"
                        , request => _prefab = request.GetAsset<GameObject>());
                }
            }
        }

        public void OnAnotherButtonClick()
        {
            Destroy(_ins);
            _ins = null;
            ResourceManager.Instance.RecycleAsset(_prefab, false);
            _async = null;
            _prefab = null;
        }
    }
}