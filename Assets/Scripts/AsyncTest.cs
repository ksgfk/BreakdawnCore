using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class AsyncTest : MonoBehaviour
    {
        private AssetAsync _async;
        private GameObject _prefab;
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
            ResourceManager.Instance.CacheAsync("Assets/Models/benghuai/ModelBoomOceanSoulOre.png");
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
            ResourceManager.Instance.RecycleAsset(_prefab, true);
            _async = null;
            _prefab = null;
        }
    }
}