using System;
using Breakdawn.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    public class GameObjectFactory : IFactory<GameObject>
    {
        private readonly GameObject _prefab;

        public GameObjectFactory(GameObject prefab)
        {
            _prefab = prefab ? prefab : throw new ArgumentNullException(nameof(prefab));
        }

        public GameObject Get()
        {
            return Object.Instantiate(_prefab);
        }
    }
}