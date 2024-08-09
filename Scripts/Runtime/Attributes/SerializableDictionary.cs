using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Z3.UIBuilder.Core
{
    [Serializable]
    public class Pair<TKey, TValue>
    {
        public TKey key;
        public TValue value;
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>, IDictionary
    {
        [SerializeField] private List<Pair<TKey, TValue>> pairs = new List<Pair<TKey, TValue>>();

        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        //  IDictionary<TK, TV> implementation
        public ICollection<TKey> Keys => dictionary.Keys;
        public ICollection<TValue> Values => dictionary.Values;
        public int Count => dictionary.Count;
        public bool IsReadOnly => false;

        // IDictionary implementation
        public object this[object key]
        {
            get => dictionary[(TKey)key];
            set => dictionary[(TKey)key] = (TValue)value;
        }

        public bool IsSynchronized => false;
        public object SyncRoot => this;
        public bool IsFixedSize => ((IDictionary)dictionary).IsFixedSize;
        ICollection IDictionary.Keys => dictionary.Keys;
        ICollection IDictionary.Values => dictionary.Values;

        // ISerializationCallbackReceiver methods
        public void OnBeforeSerialize()
        {
            pairs = Convert(dictionary);
        }

        public void OnAfterDeserialize()
        {
            dictionary = pairs.ToDictionary(p => p.key, p => p.value);
        }

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public static List<Pair<TKey, TValue>> Convert(Dictionary<TKey, TValue> dictionary)
        {
            return dictionary.Select(p => new Pair<TKey, TValue>
            {
                key = p.Key,
                value = p.Value
            }).ToList();
        }

        //  IDictionary<TK, TV> implementation
        public void Add(TKey key, TValue value) => dictionary.Add(key, value);
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        public bool Remove(TKey key) => dictionary.Remove(key);
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
        public void Add(KeyValuePair<TKey, TValue> item) => dictionary.Add(item.Key, item.Value);
        public void Clear() => dictionary.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => dictionary.Contains(item);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<TKey, TValue> item) => dictionary.Remove(item.Key);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();

        // IDictionary implementation
        public void Add(object key, object value) => dictionary.Add((TKey)key, (TValue)value);
        public bool Contains(object key) => dictionary.ContainsKey((TKey)key);
        public void Remove(object key) => dictionary.Remove((TKey)key);
        public void CopyTo(Array array, int index) => ((IDictionary)dictionary).CopyTo(array, index);
        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)dictionary).GetEnumerator();

        // Implicit operators
        public static implicit operator SerializableDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            return new SerializableDictionary<TKey, TValue>() { pairs = Convert(dictionary) };
        }

        public static implicit operator Dictionary<TKey, TValue>(SerializableDictionary<TKey, TValue> serializableDictionary)
        {
            return serializableDictionary.dictionary;
        }
    }
}