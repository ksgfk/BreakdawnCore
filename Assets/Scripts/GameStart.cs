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
            _attackPrefab = AssetBundleManager.Instance.GetAssetDependAB("Attack.prefab")
                .LoadAsset<GameObject>("Attack");
            _attackPrefab = AssetBundleManager.Instance.GetAssetDependAB("Attack.prefab")
                .LoadAsset<GameObject>("Attack");
            var a = AssetBundleManager.Instance;
            _attack = Instantiate(_attackPrefab);

            var raw = new LinkedList<int>();
            raw.AddLast(1);
            raw.AddLast(2);
            raw.AddLast(3);
            var l = new FastLinkedList<int>(ref raw);
            var node = l.Find(2);
            var b = l.Contains(default);
            Debug.Log(b);
        }

        public void OnButtonClick()
        {
            Destroy(_attack);
            AssetBundleManager.Instance.ReleaseAsset("Attack.prefab");
            var a = AssetBundleManager.Instance;
        }
    }
}