using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.UIBuilder.Editor
{
    public class TypeSelectorDrawer : Z3AttributeDrawer<TypeSelectorAttribute>
    {
        protected override void Draw()
        {
            ClearVisualElement();
            object value = SerializedProperty.GetValue();

            TypeSelector typeSelector;
            if (value is IList list)
            {
                typeSelector = new(SerializedProperty.serializedObject.targetObject, list, SerializedProperty.displayName, true);
            }
            else
            { 
                // TODO: First time when you open inspector is opening a array, but the Member Info is correct. Review EditorBuilder
                if (SerializedProperty.propertyPath.EndsWith("]"))
                    return;

                // NOTE: object value is null, is this correct?
                object declaringInstance = PropertyResolver.GetParentObjectOfProperty(SerializedProperty); 
                typeSelector = new(SerializedProperty.serializedObject.targetObject, MemberInfo, declaringInstance, SerializedProperty.displayName);
            }

            VisualElement.Add(typeSelector);
        }
    }

    public class TypeSelector : VisualElement
    {
        public event Action OnChange;

        private struct Null { } // Useful class

        private Action<object> set;
        private Func<object> get;
        private bool saveChangesBtn; /// TEMP

        private object Value
        {
            get => get();
            set => set(value);
        }

        public TypeSelector(MemberInfo memberInfo, object targetClass, string label = null)
        {
             DrawAsProperty(memberInfo, targetClass, label);
        }

        public TypeSelector(Object target, MemberInfo memberInfo, object targetClass, string label = null) : this(memberInfo, targetClass, label)
        {
            OnChange += () =>
            {
                EditorUtility.SetDirty(target);
            };
        }

        public TypeSelector(Object target, IList list, string fieldName = null, bool saveChangesBtn = false) : this(list, fieldName, saveChangesBtn)
        {
            OnChange += () =>
            {
                EditorUtility.SetDirty(target);
            };
        }

        public TypeSelector(IList list, string fieldName = null, bool saveChangesBtn = false)
        {
            this.saveChangesBtn = saveChangesBtn;
            DrawAsArray(list, fieldName);
        }

        private void DrawAsArray(IList list, string fieldName = null)
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
            inspectElement.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            inspectElement.style.SetPadding(4);

            listView = new(list, config);
            listView.OnSelectChange += DrawSelection;
            listView.OnDelete += e => OnChange?.Invoke();

            if (list.Count > 0)
            {
                DrawSelection(list[0]);
            }

            VisualElement root = new();
            root.Add(listView);
            root.Add(inspectElement); // TODO: Add to Foldout. ListView.Foldout.Add

            if (saveChangesBtn) // TEMP
            {
                Button saveChangesBtn = new(() =>
                {
                    OnChange?.Invoke();
                });
                saveChangesBtn.text = "Save Changes";
                root.Add(saveChangesBtn);
            }

            Add(root);

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

                // TODO: Handle when is null
                if (item == null)
                    return;

                MonoScriptView objectField = new(item);

                // NOTE: If use PropertyAttribute, is possible to track changes by using UIBuilderEditorExtensions.RegisterChanges / propertyField.RegisterCallback<SerializedPropertyChangeEvent>
                //IBaseFieldReader itemView = EditorBuilder.GetElement(declaringObject);
                //itemView.OnValueChangedAfterBlur += OnChange;
                //inspectElement.Add(itemView.VisualElement);

                VisualElement itemView = PropertyBuilder.BuildVisualElement(item);
                itemView.schedule.Execute(() =>
                {
                    itemView.RegisterCallback((SerializedPropertyChangeEvent evt) =>
                    {
                        OnChange?.Invoke();
                    });

                }).StartingIn(1000);

                // TODO: Send events of dirty when is 
                inspectElement.Add(objectField);
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

        private void DrawAsProperty(MemberInfo memberInfo, object target, string label)
        {
            Type propertyType = memberInfo.DeclaringType; // null?
            if (memberInfo is PropertyInfo propertyInfo)
            {
                if (propertyInfo.TryGetBackingField(out FieldInfo backingField))
                {
                    set = newValue => backingField.SetValue(target, newValue);
                    get = () => backingField.GetValue(target);
                }
                else
                {
                    set = newValue => propertyInfo.SetValue(target, newValue);
                    get = () => propertyInfo.GetValue(target);
                }
            }
            else if (memberInfo is FieldInfo fieldInfo)
            {
                propertyType = fieldInfo.FieldType;
                set = newValue => fieldInfo.SetValue(target, newValue);
                get = () => fieldInfo.GetValue(target);
            }

            // Get all derived concrete types
            List<Type> derivedTypes = ReflectionUtils.GetDeriveredConcreteTypes(propertyType).ToList();
            derivedTypes.Insert(0, typeof(Null));

            int index = Value == null ? 0 : derivedTypes.IndexOf(Value.GetType());

            label = !string.IsNullOrEmpty(label) ? label : memberInfo.Name.ToNiceString();

            VisualElement itemView = new();
            PopupField<Type> dropdownField = new(label, derivedTypes, index, t => t?.Name, t => t?.Name);

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


            Add(root);
        }
    }
}