using System;
using System.Collections;
using System.Collections.Generic;

namespace Breakdawn.Core
{
    public class FastLinkedList<T> : ICollection<T>
    {
        private readonly LinkedList<T> _list;
        private readonly Dictionary<T, LinkedListNode<T>> _nodeDict;

        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public LinkedListNode<T> First => _list.First;
        public LinkedListNode<T> Last => _list.Last;

        public FastLinkedList()
        {
            _list = new LinkedList<T>();
            _nodeDict = new Dictionary<T, LinkedListNode<T>>();
        }

        /// <param name="list">传入链表，注意，调用该构造函数时会将该字段置空</param>
        public FastLinkedList(ref LinkedList<T> list)
        {
            _list = list;
            _nodeDict = new Dictionary<T, LinkedListNode<T>>(_list.Count);
            var node = _list.First;
            while (node != null)
            {
                ParameterCheck(node);
                _nodeDict.Add(node.Value, node);
                node = node.Next;
            }

            list = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>) _list).GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            AddLast(item);
        }

        public void Clear()
        {
            _list.Clear();
            _nodeDict.Clear();
        }

        public bool Contains(T item)
        {
            return item != null && _nodeDict.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var t = arrayIndex;
            foreach (var e in _list)
            {
                array[t] = e;
                t++;
            }
        }

        public bool Remove(T item)
        {
            if (item == null)
            {
                return false;
            }

            if (!_nodeDict.TryGetValue(item, out var node))
            {
                return false;
            }

            _list.Remove(node);
            _nodeDict.Remove(item);
            return true;
        }

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            ParameterCheck(newNode);
            _list.AddBefore(node, newNode);
            _nodeDict.Add(newNode.Value, newNode);
        }

        public void AddBefore(LinkedListNode<T> node, T newElement)
        {
            ParameterCheck(newElement);
            var newNode = new LinkedListNode<T>(newElement);
            AddBefore(node, newNode);
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            ParameterCheck(newNode);
            _list.AddAfter(node, newNode);
            _nodeDict.Add(newNode.Value, newNode);
        }

        public void AddAfter(LinkedListNode<T> node, T newElement)
        {
            ParameterCheck(newElement);
            var newNode = new LinkedListNode<T>(newElement);
            AddAfter(node, newNode);
        }

        public void AddFirst(T newElement)
        {
            ParameterCheck(newElement);
            var node = _list.AddFirst(newElement);
            _nodeDict.Add(newElement, node);
        }

        public void AddFirst(LinkedListNode<T> node)
        {
            ParameterCheck(node);
            _list.AddFirst(node);
            _nodeDict.Add(node.Value, node);
        }

        public void AddLast(T newElement)
        {
            ParameterCheck(newElement);
            var node = _list.AddLast(newElement);
            _nodeDict.Add(newElement, node);
        }

        public void AddLast(LinkedListNode<T> node)
        {
            ParameterCheck(node);
            _list.AddLast(node);
            _nodeDict.Add(node.Value, node);
        }

        public LinkedListNode<T> Find(T value)
        {
            return _nodeDict.TryGetValue(value, out var result) ? result : null;
        }

        public void RemoveFirst()
        {
            var first = _list.First;
            KeyCheck(first.Value);
            _nodeDict.Remove(first.Value);
            _list.RemoveFirst();
        }

        public void RemoveLast()
        {
            var last = _list.Last;
            KeyCheck(last.Value);
            _nodeDict.Remove(last.Value);
            _list.RemoveLast();
        }

        private void ParameterCheck(LinkedListNode<T> node)
        {
            ParameterCheck(node.Value);
        }

        private void ParameterCheck(T node)
        {
            if (_nodeDict.ContainsKey(node))
            {
                throw new ArgumentException("不能插入重复元素");
            }
        }

        private void KeyCheck(T node)
        {
            if (!_nodeDict.ContainsKey(node))
            {
                throw new Exception($"链表中存在该元素，字典中不存在，这是个bug！");
            }
        }
    }
}