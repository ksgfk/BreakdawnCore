using System;
using UnityEngine;

namespace Breakdawn.Unity
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected void InitInstance()
        {
            if (Instance != null)
            {
                Debug.LogWarning($"单例类{typeof(T).FullName}已经初始化过了,不需要重复初始化");
                return;
            }

            if (this is T singleton)
            {
                Instance = singleton;
            }
            else
            {
                throw new ArgumentException($"单例类{typeof(T).FullName}类型错误");
            }
        }

        protected void DisposeInstance()
        {
            Instance = null;
        }
    }
}