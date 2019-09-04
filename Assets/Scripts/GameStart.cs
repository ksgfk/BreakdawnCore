using System.Collections.Generic;
using Breakdawn.Core;
using Breakdawn.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Breakdawn.Test
{
    public class GameStart : MonoBehaviour
    {
        private GameObject _attackPrefab;
        private GameObject _attack;

        private void Awake()
        {
            ResourceManager.Instance.Init(Paths.AssetConfig, Paths.Assets, Paths.StreamingAssets);
        }

        private void Start()
        {
            _attackPrefab =
                ResourceManager.Instance.GetAsset<GameObject>($"{Paths.Assets}/{Paths.Prefabs}/Attack.prefab");
            _attack = Instantiate(_attackPrefab);
        }

        public void OnButtonClick()
        {
            Destroy(_attack);
            ResourceManager.Instance.RecycleAsset(_attackPrefab, true);
            _attackPrefab = null;
            _attack = null;
        }

        public void OnAnotherButtonClick()
        {
            _attackPrefab =
                ResourceManager.Instance.GetAsset<GameObject>($"{Paths.Assets}/{Paths.Prefabs}/Attack.prefab");
            _attack = Instantiate(_attackPrefab);
        }
    }
}