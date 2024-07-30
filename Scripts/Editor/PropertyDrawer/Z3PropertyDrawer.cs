using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Used to draw custom properties
    /// </summary>
    public abstract class Z3PropertyDrawer<TProperty> : Z3Drawer
    {
        private TProperty resolvedValue;

        // Properties
        protected SerializedProperty SerializedProperty { get; private set; }
        protected TProperty ResolvedValue => resolvedValue ??= ResolveValue();

        // Core
        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty = property;

            VisualElement visualElement = CreateVisualElement();

            EditorBuilder.ApplyAttributes(property, visualElement, fieldInfo);

            return visualElement;
        }

        // Utility methods
        protected TProperty ResolveValue() => SerializedProperty.GetValue<TProperty>();

        protected TTarget GetTarget<TTarget>() where TTarget : Object
        {
            return SerializedProperty.serializedObject.targetObject as TTarget;
        }
        protected virtual VisualElement CreateVisualElement() => SerializedProperty.ToPropertyField();
    }
}