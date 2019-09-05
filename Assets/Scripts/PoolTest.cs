using System.Collections.Generic;
using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class PoolTest : MonoBehaviour
    {
        private Queue<GameObject> _obj = new Queue<GameObject>();

        private void Awake()
        {
            ResourceManager.Instance.Init(Paths.AssetConfig, Paths.Assets, Paths.StreamingAssets);
            ObjectManager.Instance.Init(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                var go = ObjectManager.Instance.Get("Assets/Prefabs/Cube.prefab");
                go.transform.position = new Vector3(Mathf.PerlinNoise(Time.time, 0) * 5,
                    Mathf.PerlinNoise(Time.time, 0) * 5,
                    Mathf.PerlinNoise(Time.time, 0) * 5);
                _obj.Enqueue(go);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log(ObjectManager.Instance.Recycle(_obj.Dequeue()));
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                ObjectManager.Instance.DestroyPool("Assets/Prefabs/Cube.prefab");
            }
        }
    }
}