using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    public class SceneReferenceAttribute : PropertyAttribute { }


    [CustomPropertyDrawer(typeof(SceneReferenceAttribute))]
    public class SceneReferenceAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.String)
                return null;

            // Create a container for the property
            VisualElement container = new();

            // Create a field for the property
            ObjectField field = new(property.displayName);
            //field.styleSheets Editor
            field.objectType = typeof(SceneAsset);
            container.Add(field);

            // Set the initial value of the field
            SceneAsset oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
            field.value = oldScene;

            // Update the property value when the field value changes
            field.RegisterValueChangedCallback(e =>
            {
                SceneAsset newScene = e.newValue as SceneAsset;
                if (newScene != oldScene)
                {
                    property.stringValue = AssetDatabase.GetAssetPath(newScene);
                }
            });
            field.bindingPath = property.propertyPath;
            field.Bind(property.serializedObject);
            // Return the container
            return container;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                SceneAsset oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
                SceneAsset newScene = EditorGUI.ObjectField(position, label, oldScene, typeof(SceneAsset), false) as SceneAsset;
                if (newScene != oldScene)
                {
                    property.stringValue = AssetDatabase.GetAssetPath(newScene);
                }
            }
            else
            {
                // If the property is not an string, use the default drawer
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}