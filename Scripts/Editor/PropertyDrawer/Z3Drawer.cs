using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Used to draw Properties by <see cref="Z3PropertyDrawer{T}"/> and Attributes by <see cref="Z3AutoAttributeDrawer"/>
    /// </summary>
    public abstract class Z3Drawer : PropertyDrawer
    {
        /// <summary>
        /// Used to apply attributes in Property Fields
        /// </summary>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return PropertyBuilder.BuildPropertyDrawer(property, fieldInfo);
        }

        /// <summary>
        /// Used to display the override window correctly, otherwise it will show "No GUI Implemented"
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{
        //    return EditorGUI.GetPropertyHeight(property, label, true);
        //}
    }
}