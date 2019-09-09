using System;
using System.Collections;
using System.Collections.Generic;

namespace Breakdawn.Core
{
    public class BilateralDictionary<TK1, TK2> : IDictionary<TK1, TK2>
    {
        private readonly Dictionary<TK1, TK2> _kToV;
        private readonly Dictionary<TK2, TK1> _vTok;

        public ICollection<TK1> Keys => _kToV.Keys;
        public ICollection<TK2> Values => _kToV.Values;

        public int Count => _kToV.Count == _vTok.Count
            ? _kToV.Count
            : throw new InvalidOperationException($"双向字典数据不一致，kToV:{_kToV.Count}，vToK:{_vTok.Count}");

        public bool IsReadOnly => false;

        public BilateralDictionary(int capacity = 0)
        {
            _kToV = new Dictionary<TK1, TK2>(capacity);
            _vTok = new Dictionary<TK2, TK1>(capacity);
        }

        public BilateralDictionary(IDictionary<TK1, TK2> dict) : this(dict.Count)
        {
            foreach (var pair in dict)
            {
                if (!TryAdd(pair.Key, pair.Value))
                {
                    throw new InvalidOperationException($"重复的键值对:{pair.ToString()}");
                }
            }
        }

        public bool TryAdd(TK1 key, TK2 value)
        {
            if (_kToV.ContainsKey(key) || _vTok.ContainsKey(value))
            {
                return false;
            }

            _kToV.Add(key, value);
            _vTok.Add(value, key);
            return true;
        }

        public IEnumerator<KeyValuePair<TK1, TK2>> GetEnumerator()
        {
            return _kToV.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<TK1, TK2>>.Add(KeyValuePair<TK1, TK2> item)
        {
            if (!TryAdd(item.Key, item.Value))
            {
                throw new InvalidOperationException($"重复的键值对:{item.ToString()}");
            }
        }

        public void Clear()
        {
            _kToV.Clear();
            _vTok.Clear();
        }

        bool ICollection<KeyValuePair<TK1, TK2>>.Contains(KeyValuePair<TK1, TK2> item)
        {
            return _kToV.ContainsKey(item.Key) && _vTok.ContainsKey(item.Value);
        }

        void ICollection<KeyValuePair<TK1, TK2>>.CopyTo(KeyValuePair<TK1, TK2>[] array, int arrayIndex)
        {
            var t = arrayIndex;
            foreach (var e in _kToV)
            {
                array[t] = e;
                t++;
            }
        }

        bool ICollection<KeyValuePair<TK1, TK2>>.Remove(KeyValuePair<TK1, TK2> item)
        {
            var c = this as ICollection<KeyValuePair<TK1, TK2>>;
            return c.Contains(item) && RemoveKey(item.Key);
        }

        void IDictionary<TK1, TK2>.Add(TK1 key, TK2 value)
        {
            if (TryAdd(key, value))
            {
                throw new InvalidOperationException($"重复的键值对:{new KeyValuePair<TK1, TK2>(key, value).ToString()}");
            }
        }

        public bool ContainsKey(TK1 key)
        {
            return _kToV.ContainsKey(key) && _vTok.ContainsValue(key);
        }

        public bool ContainsValue(TK2 value)
        {
            return _vTok.ContainsKey(value) && _kToV.ContainsValue(value);
        }

        bool IDictionary<TK1, TK2>.Remove(TK1 key)
        {
            return RemoveKey(key);
        }

        public bool RemoveKey(TK1 key)
        {
            return RemoveFromDictionaries(key, _kToV, _vTok);
        }

        public bool RemoveValue(TK2 value)
        {
            return RemoveFromDictionaries(value, _vTok, _kToV);
        }

        private static bool RemoveFromDictionaries<TK, TV>(TK item, Dictionary<TK, TV> kv, Dictionary<TV, TK> vk)
        {
            if (!kv.TryGetValue(item, out var v))
            {
                return false;
            }

            if (!vk.ContainsValue(item))
            {
                return false;
            }

            if (!kv.Remove(item))
            {
                return false;
            }

            if (vk.Remove(v))
            {
                return true;
            }

            kv.Add(item, v);
            return false;
        }

        public bool TryGetValue(TK1 key, out TK2 value)
        {
            return _kToV.TryGetValue(key, out value);
        }

        public bool TryGetKey(TK2 value, out TK1 key)
        {
            return _vTok.TryGetValue(value, out key);
        }

        TK2 IDictionary<TK1, TK2>.this[TK1 key]
        {
            get => _kToV[key];
            set
            {
                _kToV[key] = value;
                _vTok[value] = key;
            }
        }
    }
}