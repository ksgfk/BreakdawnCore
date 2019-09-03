using System;
using System.Collections.Generic;
using System.Diagnostics;
using Breakdawn.Unity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Breakdawn.Test
{
    class A
    {
        public int b;
    }

    public class EntityTest : Entity
    {
        private List<A> _a = new List<A>(100000);
        private Dictionary<int, A> _b = new Dictionary<int, A>(100000);
        private bool _init = false;
        private bool _d = true;

        private void Start()
        {
            for (var i = 0; i < 10000000; i++)
            {
                _a.Add(new A {b = i});
                _b.Add(i, new A {b = i});
            }
        }

        private void Update()
        {
            if (!_init && _d)
            {
                var sw = new Stopwatch();
                sw.Start();
                foreach (var a in _a)
                {
                    a.b++;
                }

                sw.Stop();
                Debug.Log(sw.ElapsedMilliseconds);
                _init = true;
            }

            if (_init && _d)
            {
                var sw = new Stopwatch();
                sw.Start();
                foreach (var a in _b.Values)
                {
                    a.b++;
                }

                sw.Stop();
                Debug.Log(sw.ElapsedMilliseconds);
                _d = false;
            }
        }
    }
}