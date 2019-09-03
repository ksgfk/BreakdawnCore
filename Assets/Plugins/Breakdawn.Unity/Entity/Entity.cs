using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Unity
{
    public abstract class Entity : MonoBehaviour
    {
        private readonly List<IEntityComponent> _monoComponents = new List<IEntityComponent>();
        private readonly Dictionary<Type, Component> _components = new Dictionary<Type, Component>();

        private void Update()
        {
            foreach (var component in _monoComponents)
            {
                component.OnUpdate();
            }
        }

        public T GetEntityComponent<T>() where T : Component
        {
            var type = typeof(T);
            if (_components.TryGetValue(type, out var component))
            {
                return component as T;
            }

            if (!TryGetComponent(type, out component))
            {
                Debug.LogWarning($"不存在{type.FullName}");
                return null;
            }

            _components.Add(type, component);
            return component as T;
        }

        public T AddEntityComponent<T>(bool canRepeat = false) where T : Component
        {
            var type = typeof(T);

            if (canRepeat)
            {
                return AddComponentWithDict<T>();
            }

            if (_components.ContainsKey(type))
            {
                Debug.LogWarning($"已经存在{type.FullName}");
                return null;
            }

            if (!TryGetComponent(type, out _))
            {
                return AddComponentWithDict<T>();
            }

            Debug.LogWarning($"已经存在{type.FullName}");
            return null;
        }

        private T AddComponentWithDict<T>() where T : Component
        {
            var type = typeof(T);
            var c = gameObject.AddComponent<T>();
            if (c == null)
            {
                throw new ArgumentException($"无法添加{type.FullName}");
            }

            if (c is IEntityComponent ec)
            {
                _monoComponents.Add(ec);
            }

            _components.Add(type, c);
            return c;
        }

        public void RemoveEntityComponent<T>()
        {
            var type = typeof(T);
            if (type == typeof(GameObject) || type == typeof(Transform))
            {
                throw new InvalidOperationException();
            }

            if (_components.TryGetValue(type, out var component))
            {
                Destroy(component);
                _components.Remove(type);
                if (component is IEntityComponent ec)
                {
                    _monoComponents.Remove(ec);
                }

                return;
            }

            if (TryGetComponent(type, out component))
            {
                Destroy(component);
            }

            Debug.LogWarning($"不存在{type.FullName}");
        }
    }
}