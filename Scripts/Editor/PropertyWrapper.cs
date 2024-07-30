using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;

namespace Z3.UIBuilder.Editor
{
    /// <summary> Used to display any object in editor without inheriting <see cref="Object"/></summary>
    public class PropertyWrapper : ScriptableObject
    {
        [SerializeReference] public object property;

        public static PropertyField CreateAsPropertyField(object instance)
        {
            // Instantiate Wrapper
            PropertyWrapper genericProperty = CreateInstance<PropertyWrapper>();
            genericProperty.property = instance;

            // Create SerializedObject to get the SerializedProperty
            SerializedObject serializedObject = new SerializedObject(genericProperty);
            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(property));

            PropertyField propertyField = serializedProperty.ToPropertyField();
            propertyField.Bind(serializedObject);
            return propertyField;
        }

        public static InspectorElement CreateAsInspectorElement(object instance)
        {
            // Instantiate Wrapper
            PropertyWrapper genericProperty = CreateInstance<PropertyWrapper>();
            genericProperty.property = instance;

            // Create SerializedObject to bind in Inspector
            SerializedObject serializedObject = new SerializedObject(genericProperty);

            return new InspectorElement(serializedObject);
        }
    }
}