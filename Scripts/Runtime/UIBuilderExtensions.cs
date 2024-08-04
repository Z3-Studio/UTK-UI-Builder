using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.ExtensionMethods
{
    public static class UIBuilderExtensions
    {
        /// <summary> Useful to use inside visual elements </summary>
        public static void BindUIElements(this VisualElement root, bool everything = false)
        {
            root.BindUIElements(root, everything);
        }

        /// <summary> Useful to use inside editor classes </summary>
        /// <remarks> Z3Editor classes do this automatically </remarks>
        public static void BindUIElements<T>(this VisualElement root, T target, bool everything = false) where T : class
        {
            UIElementBuilder.BindFields(target, root, everything);
        }

        /// <summary> Use always the same string </summary>
        public static VisualElement GetProperty(this VisualElement root, string propertyName)
        {
            return root.GetProperty<VisualElement>(propertyName);
        }

        /// <summary> Use always the same string </summary>
        public static T GetProperty<T>(this VisualElement root, string propertyName) where T : VisualElement
        {
            return root.Q<T>("PropertyField:" + propertyName);
        }

        public static void ExecuteWhenAttach(this VisualElement element, Action action)
        {
            //root.schedule.Execute(() =>
            //{
            //    BuildVisualElements(root, target);
            //}).StartingIn(0);

            if (element.panel != null)
            {
                action();
                return;
            }

            // Wait panel
            element.RegisterCallback<AttachToPanelEvent>(SetupMember);

            void SetupMember(AttachToPanelEvent _)
            {
                element.UnregisterCallback<AttachToPanelEvent>(SetupMember);
                action();
            }
        }

        public static void Bind(this VisualElement element, UnityEngine.Object obj)
        {
            SerializedObject serializedObject = new SerializedObject(obj);
            BindingExtensions.Bind(element, serializedObject);
        }
    }
}