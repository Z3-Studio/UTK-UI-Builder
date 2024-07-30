using UnityEditor;
using Z3.UIBuilder.Core;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Z3.UIBuilder.Editor
{
    using Object = UnityEngine.Object;

    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class DictionaryDrawer : PropertyDrawer
    {
        private SerializableDictionary<object, object> dictionary;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            PropertyField amountField = new PropertyField(property.FindPropertyRelative("_Keys"));
            PropertyField nameField = new PropertyField(property.FindPropertyRelative("_Values"), "Fancy Name");

            root.Add(amountField);
            root.Add(nameField);

            return root;
        }
    }
}