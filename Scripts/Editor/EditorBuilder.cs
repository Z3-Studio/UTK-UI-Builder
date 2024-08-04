using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils;

namespace Z3.UIBuilder.Editor
{
    using Editor = UnityEditor.Editor;

    /// <summary>
    /// This is a helper class for creating and binding VisualElements in Unity's UI system. 
    /// It contains several methods for creating fields of various types, including buttons, sliders, and text input fields. 
    /// It also has a method for binding fields to VisualElements in a root element, and a method for generating VisualElements for all fields in an object.
    /// </summary>
    public static class EditorBuilder
    {
        [Obsolete("TODO: Find a better way to combine Property and Attribute drawers")]
        internal static bool DrawByPropertyDrawer(SerializedProperty serializedProperty, FieldInfo fieldInfo, out VisualElement visualElement)
        {
            foreach ((Type key, Type value) in UIBuilderCache.PropertyDrawers)
            {
                if (key.IsAssignableFrom(fieldInfo.FieldType))
                {
                    Z3Drawer attributeDrawer = Activator.CreateInstance(value) as Z3Drawer;
                    value.GetField("m_FieldInfo", ReflectionUtils.InstanceAccess).SetValue(attributeDrawer, fieldInfo);
                    visualElement = attributeDrawer.CreatePropertyGUI(serializedProperty);
                    return true;
                }
            }

            visualElement = null;
            return false;
        }

        /// <summary> Bind Visual Elements + Attributes </summary>
        public static void BuildEditorWindow(EditorWindow editorWindow)
        {
            //GenerateElementsAndAttributes(editorWindow.rootVisualElement, editorWindow);
        }

        public static VisualElement BuildEditor(Editor editor) // used from Z3Editor and mini inspectors
        {
            // Alternative
            VisualElement root = new();

            InspectorElement.FillDefaultInspector(root, editor.serializedObject, editor);

            GenerateElementsAndAttributes(root, editor.target);

            return root;
        }

        public static void ApplyAttributes(SerializedProperty serializedProperty, VisualElement propertyField, MemberInfo memberInfo)
        {
            List<Z3VisualElementAttribute> attributes = memberInfo.GetCustomAttributes<Z3VisualElementAttribute>().ToList();
            if (attributes.Count == 0)
                return;

            propertyField.ExecuteWhenAttach(() =>
            {
                foreach (Z3VisualElementAttribute attribute in attributes)
                {
                    Type attributeType = attribute.GetType();
                    if (UIBuilderCache.AttributeDrawers.TryGetValue(attributeType, out IZ3AttributeDrawer drawer))
                    {
                        drawer.Init(serializedProperty, propertyField, memberInfo, attribute);
                        drawer.Draw();
                    }
                }
            });
        }

        public static void GenerateElementsAndAttributes(VisualElement root, object target)
        {
            // TODO: Fix draw order
            //VisualElement aux = new VisualElement();
            //root.Add(aux);

            root.ExecuteWhenAttach(() =>
            {
                // Define execution order.
                // - Remember to create nested implementations

                // Declared Visual Elements
                GenerateVisualElements(root, target);

                // Only editor classes 
                if (root is not PropertyField)
                {
                    UIElementBuilder.BindFields(target, root);
                }

                IEnumerable<MemberInfo> members = ReflectionUtils.GetAllMembers(target, ReflectionUtils.AllDeclared);
                foreach (Z3InspectorMemberAttributeProcessor processor in UIBuilderCache.AttributeProcessor)
                {
                    processor.ProcessMembers(target, members, root);
                }
            });
        }

        /// <summary>
        /// Generates VisualElements for all fields in the specified target object, and adds them to the provided root element. 
        /// It ignore visual elements with <see cref="UIElementAttribute"/>
        /// </summary>
        /// <remarks>
        /// Ex: int, float, string, bool, enum, objects, UnityEngine.Object, LayerMask, buttons, etc...
        /// </remarks>
        private static void GenerateVisualElements(VisualElement root, object target)
        {
            IEnumerable<FieldInfo> query = ReflectionUtils.GetAllFields(target)
                .Where(f => typeof(VisualElement).IsAssignableFrom(f.FieldType) && f.GetCustomAttribute<UIElementAttribute>() == null);

            foreach (FieldInfo field in query)
            {
                VisualElement newElement;

                // Instantiate field
                newElement = field.FieldType switch
                {
                    Type t when t == typeof(IntegerField) => BaseFieldBuilder.CreateIntegerFieldFromVisualElement(field, target),
                    Type t when t == typeof(SliderInt) => SliderIntBuilder.CreateSliderIntFromVisualElement(field, target),
                    Type t when t == typeof(Button) => ButtonBuilder.CreateButtonFromVisualElementField(field, target),
                    _ => DefaultCase()
                };

                VisualElement DefaultCase()
                {
                    Debug.Log($"NotImplementedException\nField Name: {field.Name}, Type: {field.FieldType}");
                    return Activator.CreateInstance(field.FieldType) as VisualElement;
                }

                // Add root new instance
                if (root is PropertyField prop)
                {
                    prop.Q<Foldout>().Add(newElement);
                }
                else
                {
                    root.Add(newElement);
                }
            }
        }

        public static IBaseFieldReader GetElement(object target, FieldInfo field, Type fieldType)
        {
            IBaseFieldReader element = GetElement(fieldType);
            element.Bind(target, field);
            return element;
        }

        public static IBaseFieldReader GetElement(object target, PropertyInfo property, Type fieldType)
        {
            IBaseFieldReader element = GetElement(fieldType);
            element.Bind(target, property);
            return element;
        }

        public static IBaseFieldReader GetElement(Type fieldType) => BaseFieldBuilder.CreateBaseField(fieldType);
    }
}