using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using System.Reflection;

namespace Z3.UIBuilder.Editor
{
    /// <summary> Used to display any object in editor without inheriting <see cref="Object"/></summary>
    public class PropertyWrapper : ScriptableObject
    {
        [SerializeReference] public object property;

        public static PropertyField CreateAsPropertyField(object instance, MemberInfo memberInfo)
        {
            if (instance == null)
                return new() { name = $"PropertyWrapper:{null}" };

            // Instantiate Wrapper
            PropertyWrapper genericProperty = CreateInstance<PropertyWrapper>();
            genericProperty.property = instance;

            // Create SerializedObject to get the SerializedProperty
            SerializedObject serializedObject = new SerializedObject(genericProperty);
            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(property));

            PropertyField propertyField = serializedProperty.ToPropertyField();

            // Note: Bind(serializedObject) is also valid?
            propertyField.BindProperty(serializedObject);
            propertyField.name = $"PropertyWrapper:{instance.GetType().Name}";

            //if (memberInfo != null) // TODO: Remove this way
            //{
            //    EditorBuilder.ApplyAttributes(serializedProperty, propertyField, memberInfo);
            //}
            //else
            //{
                EditorBuilder.ProcessAttributes(serializedProperty, propertyField);
                // TODO: Remove GenerateElements from PropertyBuilder.cs
                //EditorBuilder.GenerateElementsAndAttributes(propertyField, instance);
            //}

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