﻿using System.Collections.Generic;
using Breakdawn.Core;
using Breakdawn.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Breakdawn.Test
{
    public class GameStart : MonoBehaviour
    {
        private UnityObjectInfo<GameObject> _attackPrefab;
        private UnityObjectInfo<GameObject> _a;
        private GameObject _attack;

        private UnityObjectInfo<AudioClip> _testClip;
        public AudioSource source;

        private void Awake()
        {
            ResourceManager.Instance.Init(Paths.AssetConfig, Paths.Assets, Paths.StreamingAssets);
        }

        private void Start()
        {
            ResourceManager.Instance.GetAsset(ref _attackPrefab,
                "Attack.prefab",
                Paths.Assets,
                Paths.Prefabs);

//            ResourceManager.Instance.GetAsset(ref _a,
//                "Attack.prefab",
//                Paths.Assets,
//                Paths.Prefabs);

            _attack = Instantiate(_attackPrefab.obj);
            ResourceManager.Instance.GetAsset(ref _testClip,
                "senlin.mp3",
                Paths.Assets,
                Paths.Sounds);
            source.clip = _testClip.obj;
            source.Play();
        }

        public void OnButtonClick()
        {
            Destroy(_attack);
            ResourceManager.Instance.RecycleAsset(ref _attackPrefab);
        }

        public void OnAnotherButtonClick()
        {
//            ResourceManager.Instance.GetAsset(ref _attackPrefab,
//                "Attack.prefab",
//                Paths.Assets,
//                Paths.Prefabs);
//            _attack = Instantiate(_attackPrefab.obj);
//
//            var a = AssetBundleManager.Instance;
//            var b = ResourceManager.Instance;

            source.Stop();
            source.clip = null;
            ResourceManager.Instance.RecycleAsset(ref _testClip);
        }
    }
}