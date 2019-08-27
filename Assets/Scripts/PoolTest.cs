using System.Collections.Generic;
using Breakdawn.Core;
using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class PoolTest : MonoBehaviour
    {
//        public GameObject cube;
//        public GameObject parent;
//
//        private ObjectPool<GameObject> _pool;

        private void Awake()
        {
//            var factory = new ObjectFactory<GameObject>(() => Instantiate(cube, parent.transform, true).Hide());
//            _pool = new ObjectPool<GameObject>(factory, 5);
//            _pool.OnGetObject += go => go.Show();
//            _pool.OnRecycling += go => go.Hide();
//            _pool.OnRelease += Destroy;
                AssetBundleManager.Instance.LoadConfig(Application.streamingAssetsPath);
        }

//        private float _last;
//        private Stack<GameObject> _s = new Stack<GameObject>();

        private void Update()
        {
//            if (Time.time > 5 && Time.time <= 20)
//            {
//                if (Time.time - _last > 1)
//                {
//                    _s.Push(_pool.Get());
//                    _last = Time.time;
//                }
//            }
//
//            if (Time.time > 20)
//            {
//                if (_s.Count > 0)
//                {
//                    _pool.Recycle(_s.Pop());
//                }
//            }
//
//            if (Time.time > 30)
//            {
//                _pool.Release();
//            }
        }
    }
}