using System.Collections.Generic;
using Breakdawn.Core;
using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class GameStart : MonoBehaviour
    {
        private UnityObjectInfo<GameObject> _attackPrefab;
        private GameObject _attack;

        private void Awake()
        {
            AssetBundleManager.Instance.LoadConfig("Assets/StreamingAssets");
        }

        private void Start()
        {
            ResourceManager.Instance.GetAsset("Attack.prefab", ref _attackPrefab);
            _attack = Instantiate(_attackPrefab.obj);
            var a = AssetBundleManager.Instance;
        }

        public void OnButtonClick()
        {
            Destroy(_attack);
            ResourceManager.Instance.RecycleAsset(ref _attackPrefab);

            var a = AssetBundleManager.Instance;
            var b = ResourceManager.Instance;
        }

        public void OnAnotherButtonClick()
        {
            ResourceManager.Instance.GetAsset("Attack.prefab", ref _attackPrefab);
            _attack = Instantiate(_attackPrefab.obj);

            var a = AssetBundleManager.Instance;
            var b = ResourceManager.Instance;
        }
    }
}