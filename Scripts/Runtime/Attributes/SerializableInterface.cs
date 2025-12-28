using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Z3.UIBuilder.Core
{
    public interface ISerializableInterface
    {
        Object SerializedObject { get; }
        Type InterfaceType { get; }
    }

    // Note: Is possible to use SerializedReference + CustomDrawerAttribute. Like SerializedInterfaceReferenceAttribute
    [Serializable]
    public class SerializableInterface<TInterface> : ISerializationCallbackReceiver, ISerializableInterface where TInterface : class
    {
        [SerializeField] private Object serializedObject;

        Object ISerializableInterface.SerializedObject => serializedObject;
        Type ISerializableInterface.InterfaceType => typeof(TInterface);

        public TInterface Interface { get => serializedObject as TInterface; set => serializedObject = value as Object; }

        public void OnAfterDeserialize()
        {
            Interface = serializedObject as TInterface;
        }

        public void OnBeforeSerialize()
        {
            serializedObject = Interface as Object;
        }

        public static implicit operator SerializableInterface<TInterface>(TInterface tInterface)
        {
            return new SerializableInterface<TInterface>() { Interface = tInterface };
        }

        public static implicit operator TInterface(SerializableInterface<TInterface> serializedInterface)
        {
            return serializedInterface.Interface;
        }
    }
}