using System;
using System.Collections;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.Utils;

namespace Z3.UIBuilder.Editor
{
    public class PropertyWindow : Z3EditorWindow
    {
        private object property;
        private MemberInfo member;

        public static PropertyWindow OpenWindow(string title, object property, Type type, MemberInfo memberInfo = null)
        {
            PropertyWindow window = CreateInstance<PropertyWindow>();
            window.titleContent = new GUIContent(title);
            window.property = property;
            window.member = memberInfo;
            window.Show();

            return window;
        }

        public static PropertyWindow OpenWindow(string title, object property)
        {
            PropertyWindow window = CreateInstance<PropertyWindow>();
            window.titleContent = new GUIContent(title);
            window.property = property;
            window.Show();

            return window;
        }

        protected override void CreateGUI()
        {
            if (property == null)
            {
                Close();
                return;
            }

            if (property is IList enumerable)
            {
                //IBaseFieldReader baseField = EditorBuilder.GetElement(property, propertyInfo, type);
                //rootVisualElement.Add(baseField.VisualElement);

                Type subType = enumerable.GetType().GetGenericArguments()[0];

                ListView listView = new ListView(enumerable)
                {
                    allowAdd = true,
                    allowRemove = true,
                    headerTitle = ReflectionUtils.TypeToNiceString(subType),
                    showAddRemoveFooter = true,
                    showBorder = true,
                    showBoundCollectionSize = true,
                    showFoldoutHeader = true,
                    reorderable = true,
                    makeItem = () =>
                    {
                        return new VisualElement();
                    },
                    bindItem = (v, i) =>
                    {
                        v.Clear();
                        IBaseFieldReader baseField = EditorBuilder.GetElement(subType);
                        VisualElement valueField = baseField.VisualElement;
                        // Bind
                        baseField.CreateGetSet
                        (
                            () => enumerable[i],
                            newValue => enumerable[i] = newValue
                        );

                        // Save changes
                        baseField.OnValueChangedAfterBlur += () =>
                        {
                            if (enumerable[i] == baseField.Value)
                                return;

                            enumerable[i] = baseField.Value;
                            //OnValueChange?.Invoke();
                        };

                        // Remove Label of the value field
                        baseField.SetLabel(string.Empty);

                        v.Add(valueField);
                    }
                };
                rootVisualElement.Add(listView);
                return;
            }

            PropertyField propertyField = PropertyWrapper.CreateAsPropertyField(property, member);
            //propertyField.RegisterCallbackOnce<BlurEvent>(x =>
            //{

            //});

            rootVisualElement.Add(propertyField);

            // TODO: Use this
            //PropertyBuilder.DrawInstance(rootVisualElement, property, member);
        }
    }
}
