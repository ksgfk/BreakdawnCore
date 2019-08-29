using System.Collections.Generic;
using Breakdawn.Core;
using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class GameStart : MonoBehaviour
    {
        private GameObject _attackPrefab;
        private GameObject _attack;

        private void Awake()
        {
            AssetBundleManager.Instance.LoadConfig("Assets/StreamingAssets");
        }

        private void Start()
        {
            _attackPrefab = ResourceManager.Instance.GetAsset<GameObject>("Attack.prefab");
            _attack = Instantiate(_attackPrefab);
            var a = AssetBundleManager.Instance;
        }

        public void OnButtonClick()
        {
            Destroy(_attack);
            AssetBundleManager.Instance.ReleaseAsset("Attack.prefa");
            var a = AssetBundleManager.Instance;
        }
    }
}