using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor.ExtensionMethods
{
    public static class UIBuilderEditorExtensions
    {
        public static int VisualElementCounter { get; private set; }
        public static int BaseFieldCounter { get; private set; }

        private const double TimeToUpdate = 0.2d;

        public static void RegisterUpdate(this VisualElement element, EditorApplication.CallbackFunction onUpdate)
        {
            double lastUpdate = 0d;

            if (element.panel != null)
            {
                onUpdate();
                EditorApplication.update += OnUpdate;
                VisualElementCounter++;
            }

            element.RegisterCallback<AttachToPanelEvent>(Attach);
            element.RegisterCallback<DetachFromPanelEvent>(Detach);

            void Attach(AttachToPanelEvent e)
            {
                EditorApplication.update += OnUpdate;
                VisualElementCounter++;
            }

            void Detach(DetachFromPanelEvent e)
            {
                EditorApplication.update -= OnUpdate;
                VisualElementCounter--;
            }

            // Update the field when the instance changes the value 
            void OnUpdate()
            {
                if (EditorApplication.timeSinceStartup < lastUpdate + TimeToUpdate)
                    return;

                lastUpdate = EditorApplication.timeSinceStartup;
                onUpdate();
            }
        }

        public static void RegisterChanges<TField, TValue>(this TField visualField, FieldInfo field, object target) where TField : BaseField<TValue>
        {
            TValue Get() => (TValue)field.GetValue(target);
            void Set(TValue newValue) => field.SetValue(target, newValue);

            RegisterChanges(visualField, Get, Set);
        }

        public static void RegisterChanges<TField, TValue>(this TField visualField, PropertyInfo property, object target) where TField : BaseField<TValue>
        {
            TValue Get() => (TValue)property.GetValue(target);
            void Set(TValue newValue) => property.SetValue(target, newValue);

            RegisterChanges(visualField, Get, Set);
        }

        public static void RegisterChanges<TField, TValue>(this TField fieldVisual, Func<TValue> getValue, Action<TValue> setValue) where TField : BaseField<TValue>
        {
            double lastUpdate = 0d;

            fieldVisual.RegisterCallback<AttachToPanelEvent>(Attach);
            fieldVisual.RegisterCallback<DetachFromPanelEvent>(Detach);

            void Attach(AttachToPanelEvent e)
            {
                fieldVisual.RegisterValueChangedCallback(OnChangeValue);
                EditorApplication.update += OnUpdate;
                BaseFieldCounter++;
            }

            void Detach(DetachFromPanelEvent e)
            {
                fieldVisual.UnregisterValueChangedCallback(OnChangeValue);
                EditorApplication.update -= OnUpdate;
                BaseFieldCounter--;
            }

            // Update the instance when the user changes the value 
            void OnChangeValue(ChangeEvent<TValue> e) => setValue(e.newValue);
            
            // Update the field when the instance changes the value 
            void OnUpdate()
            {
                if (EditorApplication.timeSinceStartup < lastUpdate + TimeToUpdate)
                    return;

                lastUpdate = EditorApplication.timeSinceStartup;
                fieldVisual.value = getValue();
            }
        }

        // TODO: Try to remove SerializedProperty argument (if is possible)
        public static void TransferBinding(this BindableElement newElement, BindableElement oldElement, SerializedProperty serializedProperty)
        {
            newElement.BindProperty(serializedProperty);
            newElement.bindingPath = oldElement.bindingPath;
        }

        public static void Bind(this VisualElement element, UnityEngine.Object obj)
        {
            SerializedObject serializedObject = new SerializedObject(obj);
            BindingExtensions.Bind(element, serializedObject);
        }
    }
}