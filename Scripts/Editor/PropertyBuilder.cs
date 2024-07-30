using System;
using System.Collections;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Z3.Utils;

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
        /// Get
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <returns></returns>
        public static object ResolveProperty(SerializedProperty serializedProperty)
        {
            // Note: This method is important due to prop.propertyPath.
            // It happen with collections and items inside of a Serialized object (ex: structs)

            string path = serializedProperty.propertyPath.Replace(".Array.data[", "[");
            object obj = serializedProperty.serializedObject.targetObject;
            string[] elements = path.Split('.');
            foreach (string element in elements)
            {
                // Checks if the element contains an index in square brackets
                int indexStart = element.IndexOf("[");
                if (indexStart != -1)
                {
                    // Gets the name of the element
                    string elementName = element.Substring(0, indexStart);

                    // Gets the index of the element in square brackets
                    int elementLength = element.IndexOf("]") - indexStart - 1;
                    string indexString = element.Substring(indexStart + 1, elementLength);
                    int index = int.Parse(indexString);

                    obj = GetElementAtIndex(obj, elementName, index);
                }
                else
                {
                    // If there are no square brackets, get the element value directly
                    obj = GetMemberValue(obj, element);
                }
            }

            return obj;
        }

        private static object GetElementAtIndex(object source, string name, int index)
        {
            IEnumerable enumerable = GetMemberValue(source, name) as IEnumerable;
            if (enumerable == null) 
                return null;

            IEnumerator enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext()) 
                    return null;
            }

            return enumerator.Current;
        }

        private static object GetMemberValue(object source, string name)
        {
            if (source == null)
                return null;

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo f = type.GetField(name, ReflectionUtils.PublicAndPrivate);
                if (f != null)
                    return f.GetValue(source);

                // Review it
                PropertyInfo p = type.GetProperty(name, ReflectionUtils.PublicAndPrivate | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Creates fields for the specified object using the provided root element and instance of object.
        /// </summary>
        public static T CreateInstance<T>(VisualElement root) where T : class
        {
            T targetInstance = Activator.CreateInstance<T>();
            DrawInstance(root, targetInstance);
            return targetInstance;
        }

        public static VisualElement BuildVisualElement<T>(T target) where T : class
        {
            if (target is UnityEngine.Object obj)  // TODO: Review it
            {
                InspectorElement inspector = new InspectorElement();
                inspector.Bind(new SerializedObject(obj));
                return inspector;
            }

            VisualElement root = new VisualElement();
            DrawInstance(root, target);
            return root;
        }

        /// <summary>
        /// Creates fields for the specified object using the provided root element and target object.
        /// </summary>
        public static void DrawInstance<T>(VisualElement root, T target) where T : class  // Review and delete?
        {
            // TODO: if is visual element, create or try to bind
            PropertyField propertyField = PropertyWrapper.CreateAsPropertyField(target);
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

                root.Add(container);
                root.Remove(propertyField);

                EditorBuilder.GenerateElementsAndAttributes(root, target);
            }
        }
    }
}