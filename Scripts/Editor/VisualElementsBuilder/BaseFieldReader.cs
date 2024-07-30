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
        event Action OnChangeValue;
        public bool TwoWay { get; }
        object Value { get; }
        BindableElement VisualElement { get; }

        void CreateGetSet(Func<object> get, Action<object> set);
        void Bind(object target, PropertyInfo memberInfo);
        void Bind(object target, FieldInfo memberInfo);
    }

    public class BaseFieldReader<T> : IBaseFieldReader
    {
        public object Value => field.value;
        public BindableElement VisualElement { get; }
        public bool TwoWay => field != null;
        public event Action OnChangeValue;

        private readonly BaseField<T> field;
        private readonly EventCallback<ChangeEvent<T>> eventCallback;

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

            eventCallback = _ => OnChangeValue?.Invoke();
            field.RegisterValueChangedCallback(eventCallback);

            // Maybe should use BlurEvent
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
        }
    }
}