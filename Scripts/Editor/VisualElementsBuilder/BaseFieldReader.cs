using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public interface IBaseFieldReader : IDisposable
    {
        event Action OnValueChangedAfterBlur;
        event Action OnValueChange;

        public bool TwoWay { get; }
        object Value { get; }
        BindableElement VisualElement { get; }

        void SetValue(object value);
        void CreateGetSet(Func<object> get, Action<object> set);
        void Bind(object target, PropertyInfo memberInfo);
        void Bind(object target, FieldInfo memberInfo);

        void SetLabel(string label);
    }

    public class BaseFieldReader<T> : IBaseFieldReader
    {
        public object Value => field.value;
        public BindableElement VisualElement { get; }
        public bool TwoWay => field != null;
        public event Action OnValueChangedAfterBlur;
        public event Action OnValueChange;

        private readonly BaseField<T> field;
        private readonly EventCallback<ChangeEvent<T>> eventCallback;
        private readonly EventCallback<BlurEvent> blur;

        private bool dirty;

        public BaseFieldReader(BindableElement visualElement)
        {
            VisualElement = visualElement;
        }

        public BaseFieldReader(SerializedProperty property)
        {
            field.BindProperty(property);
        }

        public BaseFieldReader(BaseField<T> baseField)
        {
            VisualElement = baseField;
            field = baseField;

            eventCallback = _ =>
            {
                OnValueChange?.Invoke();
                dirty = true;
            };

            blur = _ =>
            {
                // Maybe check if the original value is different before call
                if (!dirty)
                    return;

                dirty = false;
                OnValueChangedAfterBlur?.Invoke();
            };
            
            field.RegisterValueChangedCallback(eventCallback);
            field.RegisterCallback(blur);
        }

        public void SetValue(object value)
        {
            field.value = (T)value;
        }

        public void SetLabel(string label)
        {
            field.label = label;
        }

        public void Bind(object target, PropertyInfo propertyInfo)
        {
            if (!TwoWay)
                return;

            field.name = propertyInfo.Name;
            field.label = propertyInfo.Name.GetNiceString();

            field.value = (T)propertyInfo.GetValue(target);
            field.RegisterChanges<BaseField<T>, T>(propertyInfo, target);
        }
        
        public void Bind(object target, FieldInfo fieldInfo)
        {
            if (!TwoWay)
                return;

            field.name = fieldInfo.Name;
            field.label = fieldInfo.Name.GetNiceString();

            field.value = (T)fieldInfo.GetValue(target);
            field.RegisterChanges<BaseField<T>, T>(fieldInfo, target);
        }

        public void CreateGetSet(Func<object> get, Action<object> set)
        {
            field.value = (T)get();

            field.RegisterValueChangedCallback(e =>
            {
                set(field.value);
            });

            field.RegisterUpdate(() =>
            {
                T value = (T)get();
                if (!EqualityComparer<T>.Default.Equals(value, field.value))
                {
                    field.value = value;
                }
            });
        }

        public void Dispose()
        {
            if (eventCallback == null)
                return;

            field.UnregisterCallback(eventCallback);
            field.UnregisterCallback(blur);
        }
    }
}