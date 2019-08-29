using System.Collections.Generic;
using Breakdawn.Core;
using Breakdawn.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Breakdawn.Test
{
    public class GameStart : MonoBehaviour
    {
        private UnityObjectInfo<GameObject> _attackPrefab;
        private GameObject _attack;

        private UnityObjectInfo<AudioClip> _testClip;
        public AudioSource source;

        private void Awake()
        {
            AssetBundleManager.Instance.Init("Assets/StreamingAssets");
        }

        private void Start()
        {
            ResourceManager.Instance.GetAsset("Assets/Prefabs/Attack.prefab", ref _attackPrefab);
            _attack = Instantiate(_attackPrefab.obj);
            var a = AssetBundleManager.Instance;

            ResourceManager.Instance.GetAsset("Assets/Sounds/senlin.mp3", ref _testClip);
            source.clip = _testClip.obj;
            source.Play();
        }

        public void OnButtonClick()
        {
            Destroy(_attack);
            ResourceManager.Instance.RecycleAsset(ref _attackPrefab, false);
        }

        public void OnAnotherButtonClick()
        {
//            ResourceManager.Instance.GetAsset("Attack.prefab", ref _attackPrefab);
//            _attack = Instantiate(_attackPrefab.obj);
//
//            var a = AssetBundleManager.Instance;
//            var b = ResourceManager.Instance;

            source.Stop();
            source.clip = null;
            ResourceManager.Instance.RecycleAsset(ref _testClip, false);
        }
    }
}