using System;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.UIBuilder.Core
{
    [Serializable]
    public class SerializableDictionary<TK, TV> : Dictionary<TK, TV>, ISerializationCallbackReceiver
    {
        [SerializeField] List<TK> _keys;
        [SerializeField] List<TV> _values;

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in _myDictionary)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        private new Dictionary<TK, TV> _myDictionary;

        public TV this[TK i]
        {
            get { return _myDictionary[i]; }
            set { _myDictionary[i] = value; }
        }

        public void OnAfterDeserialize()
        {
            _myDictionary = new Dictionary<TK, TV>();
            for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
                _myDictionary.Add(_keys[i], _values[i]);
        }

        public void Set(Dictionary<TK, TV> dictionary)
        {

            _myDictionary = dictionary;
        }
        public Dictionary<TK, TV> Get()
        {
            return _myDictionary;
        }
    }

    //[Serializable]
    //public class Ingredient
    //{
    //    public string name;
    //    public int amount = 1;
    //}

}