using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Z3.UIBuilder.Editor
{
    public static class VisualElementEditorExtensions
    {
        public static VisualElement ToVisualElement(this Object obj)
        {
            return new InspectorElement(obj); // TODO: Review it
        }

        public static PropertyField ToPropertyField(this SerializedProperty property)
        {
            return new PropertyField(property) // auto bind by bindingPath = property.propertyPath
            {
                name = property.name
            };
        }

        public static T GetValue<T>(this SerializedProperty serializedProperty)
        {
            return (T)GetValue(serializedProperty);
        }

        public static object GetValue(this SerializedProperty serializedProperty)
        {
            return PropertyBuilder.ResolveProperty(serializedProperty);
        }
    }
}