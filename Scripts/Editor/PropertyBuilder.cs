using System;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace Z3.UIBuilder.Editor
{
    public static class PropertyBuilder
    {
        /// <summary>
        /// This method will create the property field and apply all draw attributes
        /// </summary>
        public static PropertyField BuildPropertyDrawer(SerializedProperty property, MemberInfo memberInfo)
        {
            PropertyField propertyField = property.ToPropertyField();
            EditorBuilder.ApplyAttributes(property, propertyField, memberInfo);
            return propertyField;
        }

        // Note: The Attribute drawers doesn't work properly
        public static VisualElement BuildProperty(SerializedObject serializedObject)
        {
            VisualElement root = new VisualElement();
            SerializedProperty iterator = serializedObject.GetIterator();

            // Required iteration
            iterator.Next(true);

            // Iterate through all properties
            while (iterator.NextVisible(false))
            {
                PropertyField propertyField = iterator.ToPropertyField();
                root.Add(propertyField);

                // TODO: Review it
                if (propertyField.name == "m_Script")
                {
                    propertyField.SetEnabled(false);
                }
            }

            return root;
        }

        /// <summary>
        /// Creates fields for the specified object using the provided root element and instance of object.
        /// </summary>
        public static T CreateInstance<T>(VisualElement root) where T : class
        {
            T targetInstance = Activator.CreateInstance<T>();
            DrawInstance(root, targetInstance, true);
            return targetInstance;
        }

        /// <summary>
        /// Note: Prefer to use this approach here <see cref="EditorBuilder.GetElement"/>
        /// </summary>
        public static VisualElement BuildVisualElement<T>(T target) where T : class
        {
            if (target is UnityEngine.Object obj)  // TODO: Review it
            {
                InspectorElement inspector = new InspectorElement();
                inspector.Bind(new SerializedObject(obj));
                return inspector;
            }

            VisualElement root = new VisualElement();
            DrawInstance(root, target, false);
            return root;
        }

        /// <summary>
        /// Creates fields for the specified object using the provided root element and target object.
        /// </summary>
        public static void DrawInstance<T>(VisualElement root, T target, bool generateElements) where T : class  // Review and delete?
        {
            // TODO: if is visual element, create or try to bind
            PropertyField propertyField = PropertyWrapper.CreateAsPropertyField(target, generateElements);
            root.Add(propertyField);

            // Trustuble
            root.schedule.Execute(OnPropertyCreated).StartingIn(0);
            //root.ExecuteWhenAttach(OnPropertyCreated);

            void OnPropertyCreated() // Should I create a DrawStack?
            {
                Foldout foldout = propertyField.Q<Foldout>();
                if (foldout == null)
                {
                    root.schedule.Execute(OnPropertyCreated).StartingIn(25);
                    return;
                }

                VisualElement container = foldout.contentContainer;
                container.style.marginLeft = 0f;
                container.style.display = DisplayStyle.Flex;

                root.Add(container); // TODO: Sometimes it get error. HOW: Open UI Builder preview, and use arrows to navigate many times
                root.Remove(propertyField);

                EditorBuilder.GenerateElementsAndAttributes(root, target);
            }
        }
    }
}