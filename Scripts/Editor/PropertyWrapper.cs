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

        public static PropertyField CreateAsPropertyFieldMember(object instance, MemberInfo memberInfo) // Used for arrays like in VariableList of NodeGraph
        {
            if (instance == null)
                return new() { name = $"PropertyWrapper:{null}" };

            BuildWrapper(instance, out PropertyField propertyField, out SerializedProperty serializedProperty);

            EditorBuilder.ApplyAttributes(serializedProperty, propertyField, memberInfo);

            return propertyField;
        }

        public static PropertyField CreateAsPropertyField(object instance, bool generateElements)
        {
            if (instance == null)
                return new() { name = $"PropertyWrapper:{null}" };

            BuildWrapper(instance, out PropertyField propertyField, out SerializedProperty serializedProperty);

            if (generateElements) // TODO: remove this, used to create elements by constructor, like LevelDesignTools
            {
                EditorBuilder.GenerateElementsAndAttributes(propertyField, instance);

            }
            else
            {
                // BEST CASE
                EditorBuilder.ProcessAttributes(serializedProperty, propertyField);
            }

            return propertyField;
        }

        private static void BuildWrapper(object instance, out PropertyField propertyField, out SerializedProperty serializedProperty)
        {
            // Instantiate Wrapper
            PropertyWrapper genericProperty = CreateInstance<PropertyWrapper>();
            genericProperty.property = instance;

            // Create SerializedObject to get the SerializedProperty
            SerializedObject serializedObject = new SerializedObject(genericProperty);
            serializedProperty = serializedObject.FindProperty(nameof(property));

            propertyField = serializedProperty.ToPropertyField();

            // Note: Bind(serializedObject) is also valid?
            propertyField.BindProperty(serializedObject);
            propertyField.name = $"PropertyWrapper:{instance.GetType().Name}";
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