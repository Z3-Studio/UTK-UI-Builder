using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Used to display <see cref="SerializableInterface{TInterface}"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableInterface<>), true)]
    public class SerializableInterfacePropertyDrawer : Z3PropertyDrawer<ISerializableInterface>
    {
        protected override VisualElement CreateVisualElement()
        {
            ObjectField objectField = new()
            {
                value = ResolvedValue.SerializedObject,
                objectType = ResolvedValue.InterfaceType,
                label = SerializedProperty.displayName
            };
            objectField.BindProperty(SerializedProperty);

            return objectField;
        }
    }
}