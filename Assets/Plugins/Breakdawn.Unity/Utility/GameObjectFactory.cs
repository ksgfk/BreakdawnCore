using System;
using Breakdawn.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    public class GameObjectFactory : ObjectFactory<GameObject>
    {
        public GameObject Prefab { get; }

        public GameObjectFactory(GameObject prefab) : base(() => Object.Instantiate(prefab))
        {
            Prefab = prefab ? prefab : throw new ArgumentNullException(nameof(prefab));
        }
    }
}