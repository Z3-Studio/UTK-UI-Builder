using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class TypeSelectorDrawer : Z3AttributeDrawer<TypeSelectorAttribute>
    {
        protected override void Draw()
        {
            VisualElement.Insert(0, new TypeSelector(SerializedProperty));
        }
    }

    public class TypeSelector : VisualElement
    {
        private struct Null { } // Useful class


        PropertyResolver member;

        private Action<object> set;
        private Func<object> get;

        private bool dirty;

        private object Value
        {
            get => get();
            set => set(value);
        }

        public TypeSelector(MemberInfo memberInfo, object target)
        {
            VisualElement v = DrawAsProperty(memberInfo, target);
            Add(v);
        }

        public TypeSelector(IList list, string fieldName = null)
        {
            VisualElement v = DrawAsArray(list, fieldName);
            Add(v);
        }

        public TypeSelector(SerializedProperty property)
        {
            member = new PropertyResolver(property);
            member.GetMemberInfoWithParent(out MemberInfo memberInfo, out object target);

            if (member.IsArray)
            {
                Type propertyType = null;
                if (memberInfo is PropertyInfo propertyInfo)
                {
                    propertyType = propertyInfo.PropertyType;
                }
                else if (memberInfo is FieldInfo fieldInfo)
                {
                    propertyType = fieldInfo.FieldType;
                }
                else
                    throw new NotImplementedException();

                Value ??= Activator.CreateInstance(propertyType);
                IList list = (IList)Value;

                VisualElement v = DrawAsArray(list, property.displayName);
                Add(v);
            }
            else
            {
                VisualElement v = DrawAsProperty(memberInfo, target);
                Add(v);
            }

            //RegisterCallback<DetachFromPanelEvent>(e =>
            //{
            //    property.serializedObject.ApplyModifiedProperties();
            //});
        }

        private static VisualElement DrawAsArray(IList list, string fieldName = null)
        {
            Type elementType = list.GetType().GenericTypeArguments[0];
            ListViewBuilder<object, LabelView> listView = null;

            Z3ListViewConfig config = Z3ListViewConfig.DefaultTemplate<LabelView>();
            config.listName = fieldName;
            config.addEvent = () =>
            {
                List<Type> derivedTypes = ReflectionUtils.GetDeriveredConcreteTypes(elementType).ToList();
                SelectionPopup<Type>.Open(elementType.Name, derivedTypes, AddItem, t => t.Name.ToNiceString());
            };

            VisualElement inspectElement = new();

            listView = new(list, config);
            listView.OnSelectChange += DrawSelection;

            if (list.Count > 0)
            {
                DrawSelection(list[0]);
            }

            VisualElement root = new();
            root.Add(listView);
            root.Add(inspectElement);

            return root;

            void AddItem(Type type)
            {
                object newInstance = Activator.CreateInstance(type);
                list.Add(newInstance);
                listView.Rebuild(true);

                //dirty = true;
            }

            void DrawSelection(object item)
            {

                inspectElement.Clear();
                VisualElement itemView = PropertyBuilder.BuildVisualElement(item);
                inspectElement.Add(itemView);
            }
        }

        private VisualElement DrawAsProperty(MemberInfo memberInfo, object target)
        {
            Type propertyType = null;
            if (memberInfo is PropertyInfo propertyInfo)
            {
                propertyType = propertyInfo.PropertyType;
                set = newValue => propertyInfo.SetValue(target, newValue);
                get = () => propertyInfo.GetValue(target);
            }
            else if (memberInfo is FieldInfo fieldInfo)
            {
                propertyType = fieldInfo.FieldType;
                set = newValue => fieldInfo.SetValue(target, newValue);
                get = () => fieldInfo.GetValue(target);
            }

            // Get all derived concrete types
            List<Type> derivedTypes = ReflectionUtils.GetDerivedConcreteTypesInAssembly(propertyType).ToList();
            derivedTypes.Insert(0, typeof(Null));

            int index = Value == null ? 0 : derivedTypes.IndexOf(Value.GetType());

            VisualElement itemView = new();
            PopupField<Type> dropdownField = new(memberInfo.Name.ToNiceString(), derivedTypes, index, t => t?.Name, t => t?.Name);

            dropdownField.RegisterValueChangedCallback(evt =>
            {
                Type selectedType = evt.newValue;
                Type currentType = Value?.GetType();

                if (selectedType == typeof(Null))
                {
                    selectedType = null;
                }

                if (currentType != selectedType)
                {
                    if (selectedType != null)
                    {
                        Value = Activator.CreateInstance(selectedType);
                    }
                    else
                    {
                        Value = null;
                    }

                    //property.serializedObject.ApplyModifiedProperties();
                    dirty = true;
                }

                itemView.Clear();
                VisualElement field = PropertyBuilder.BuildVisualElement(Value);
                itemView.Add(field);
            });

            VisualElement field = PropertyBuilder.BuildVisualElement(Value);
            itemView.Add(field);

            VisualElement root = new();
            root.Add(dropdownField);
            root.Add(itemView);


            return root;
        }
    }
}