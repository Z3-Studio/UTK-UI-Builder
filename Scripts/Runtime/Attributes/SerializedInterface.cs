using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Z3.UIBuilder.Core
{
    [Serializable]
    public class SerializedInterface<TInterface> : ISerializationCallbackReceiver where TInterface : class
    {
        [SerializeField] private Object serializedObject;

        public TInterface Interface { get; private set; }

        public void OnAfterDeserialize()
        {
            Interface = serializedObject as TInterface;
        }

        public void OnBeforeSerialize()
        {
            serializedObject = Interface as Object;
        }

        public static implicit operator SerializedInterface<TInterface>(TInterface tInterface)
        {
            return new SerializedInterface<TInterface>() { Interface = tInterface };
        }

        public static implicit operator TInterface(SerializedInterface<TInterface> serializedInterface)
        {
            return serializedInterface.Interface;
        }
    }
}