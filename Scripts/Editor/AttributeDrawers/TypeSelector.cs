using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class TypeSelectorDrawer : Z3AttributeDrawer<TypeSelectorAttribute>
    {
        protected override void Draw()
        {
            VisualElement.Clear();
            TypeSelector typeSelector = new(SerializedProperty);
            VisualElement.Add(typeSelector);
        }
    }

    public class TypeSelector : VisualElement
    {
        public event Action OnChange;

        private struct Null { } // Useful class

        PropertyResolver member;

        private Action<object> set;
        private Func<object> get;
        private bool saveChangesBtn; /// TEMP

        private object Value
        {
            get => get();
            set => set(value);
        }

        public TypeSelector(SerializedProperty property) : this(property.GetValue<IList>(), property.displayName, true)
        {
            OnChange += () =>
            {
                property.serializedObject.ApplyModifiedProperties(); // Maybe not necessary
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            };

            return;
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

        public TypeSelector(MemberInfo memberInfo, object target)
        {
            VisualElement v = DrawAsProperty(memberInfo, target);
            Add(v);
        }

        public TypeSelector(IList list, string fieldName = null, bool saveChangesBtn = false)
        {
            this.saveChangesBtn = saveChangesBtn;
            VisualElement v = DrawAsArray(list, fieldName);
            Add(v);
        }

        private VisualElement DrawAsArray(IList list, string fieldName = null)
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

            if (saveChangesBtn) // TEMP
            {
                Button saveChangesBtn = new(() =>
                {
                    OnChange?.Invoke();
                });
                saveChangesBtn.text = "Save Changes";
                root.Add(saveChangesBtn);
            }

            return root;

            void AddItem(Type type)
            {
                object newInstance = Activator.CreateInstance(type);
                list.Add(newInstance);
                listView.Rebuild(true);
                OnChange?.Invoke();
            }

            void DrawSelection(object item)
            {
                inspectElement.Clear();
                VisualElement itemView = PropertyBuilder.BuildVisualElement(item);
                inspectElement.Add(itemView);

                // Maybe use property field instead of bindable
                //foreach (VisualElement element in itemView.Query<VisualElement>().Where(v => v is IBindable).ToList())
                //{
                //    if (element is PropertyField propertyField)
                //    {
                //        propertyField.RegisterCallback<AttachToPanelEvent>(e =>
                //        {
                //            propertyField.schedule.Execute(() =>
                //            {
                //                List<BindableElement> innerBindables = propertyField.Query<BindableElement>().ToList();

                //                foreach (BindableElement bindableElement in innerBindables)
                //                {
                //                    bindableElement.RegisterCallback<BlurEvent>(changeEvent =>
                //                    {
                //                        OnChange?.Invoke();
                //                    });
                //                }
                //            }

                //            ).StartingIn(100);

                //        });
                //    }
                //}

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

                    OnChange?.Invoke();
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