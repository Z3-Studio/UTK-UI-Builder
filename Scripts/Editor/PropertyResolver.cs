using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class PropertyResolver // UnityReferenceResolver 
    {
        public bool IsArray => serializedProperty.isArray;
        public SerializedProperty serializedProperty { get; }

        public PropertyResolver(SerializedProperty serializedProperty)
        {
            this.serializedProperty = serializedProperty;
        }

        public static MemberInfo GetMemberInfoFromPropertyPath(Type rootType, string propertyPath) // OLD IMPLEMENTATION -> EditorBuilder.ProcessAttributes
        {
            string[] parts = propertyPath.Split('.');
            Type currentType = rootType;
            MemberInfo lastMember = null;

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];

                // Ignore array indices (Array.data[x])
                if (part.StartsWith("Array.data[", StringComparison.Ordinal))
                {
                    continue;
                }

                FieldInfo fieldInfo = currentType.GetField(part, ReflectionUtils.AllDeclared);
                if (fieldInfo != null)
                {
                    lastMember = fieldInfo;
                    currentType = fieldInfo.FieldType;
                    continue;
                }

                PropertyInfo propertyInfo = currentType.GetProperty(part, ReflectionUtils.AllDeclared);
                if (propertyInfo != null)
                {
                    lastMember = propertyInfo;
                    currentType = propertyInfo.PropertyType;
                    continue;
                }

                // Parou em um segmento que não existe
                return null;
            }

            return lastMember;
        }

        // path1.path2.path3.path4 - 4 is nullable but 3 is not null. find the penultimate 
        public void GetMemberInfoWithParent(out MemberInfo lastMember, out object parentObject)
        {
            string path = serializedProperty.propertyPath.Replace(".Array.data[", "[");
            object obj = serializedProperty.serializedObject.targetObject;
            string[] elements = path.Split('.');

            Type currentType = obj.GetType();
            lastMember = null;
            parentObject = null;

            foreach (string element in elements)
            {
                string memberName = element;

                if (element.EndsWith("]")) // array
                {
                    int arrayStart = element.IndexOf("[");
                    memberName = element.Substring(0, arrayStart);
                }

                if (memberName.EndsWith(">k__BackingField"))
                {
                    lastMember = obj.GetType().GetBackingField(memberName);
                }
                else
                {
                    lastMember = obj.GetType().GetField(memberName);
                }

                // Guarda o objeto pai antes de atualizar `obj`
                parentObject = obj;

                int indexStart = element.IndexOf("[");
                if (indexStart != -1)
                {
                    string elementName = element.Substring(0, indexStart);
                    int elementLength = element.IndexOf("]") - indexStart - 1;
                    string indexString = element.Substring(indexStart + 1, elementLength);
                    int index = int.Parse(indexString);

                    obj = GetElementAtIndex(obj, elementName, index);
                }
                else
                {
                    obj = GetMemberValue(obj, element);
                }
            }
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
            BindingFlags publicAndPrivate = ReflectionUtils.InstanceAccess;
            if (source == null)
                return null;

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo f = type.GetField(name, publicAndPrivate);
                if (f != null)
                    return f.GetValue(source);

                // Review it
                PropertyInfo p = type.GetProperty(name, publicAndPrivate);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        // ------------- GENERATED CODE BY AI --------------------
        public static object GetTargetObjectOfProperty(SerializedProperty property)
        {
            object rootObject = property.serializedObject.targetObject;
            return GetObjectByPath(rootObject, property.propertyPath);
        }
        public static object GetParentObjectOfProperty(SerializedProperty property)
        {
            string path = property.propertyPath;
            int lastDotIndex = path.LastIndexOf('.');

            if (lastDotIndex < 0)
                return property.serializedObject.targetObject;

            string parentPath = path.Substring(0, lastDotIndex);
            object rootObject = property.serializedObject.targetObject;

            return GetObjectByPath(rootObject, parentPath);
        }

        private static object GetObjectByPath(object root, string path)
        {
            if (root == null || string.IsNullOrEmpty(path))
            {
                return root;
            }

            string normalizedPath = path.Replace(".Array.data[", "[");
            string[] elements = normalizedPath.Split('.');

            object currentObject = root;

            for (int i = 0; i < elements.Length; i++)
            {
                if (currentObject == null)
                {
                    return null;
                }

                string element = elements[i];

                if (element.Contains("["))
                {
                    int bracketIndex = element.IndexOf('[');
                    string fieldName = element.Substring(0, bracketIndex);
                    int index = int.Parse(element.Substring(bracketIndex + 1, element.Length - bracketIndex - 2));

                    FieldInfo fieldInfo = GetField(currentObject.GetType(), fieldName);
                    if (fieldInfo == null)
                    {
                        return null;
                    }

                    IList list = fieldInfo.GetValue(currentObject) as IList;
                    if (list == null || index < 0 || index >= list.Count)
                    {
                        return null;
                    }

                    currentObject = list[index];
                }
                else
                {
                    FieldInfo fieldInfo = GetField(currentObject.GetType(), element);
                    if (fieldInfo != null)
                    {
                        currentObject = fieldInfo.GetValue(currentObject);
                        continue;
                    }

                    PropertyInfo propertyInfo = GetProperty(currentObject.GetType(), element);
                    if (propertyInfo != null)
                    {
                        currentObject = propertyInfo.GetValue(currentObject);
                        continue;
                    }

                    return null;
                }
            }

            return currentObject;
        }
        private static FieldInfo GetField(Type type, string name)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(
                    name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

                if (field != null)
                {
                    return field;
                }

                type = type.BaseType;
            }

            return null;
        }

        private static PropertyInfo GetProperty(Type type, string name)
        {
            while (type != null)
            {
                PropertyInfo property = type.GetProperty(
                    name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

                if (property != null)
                {
                    return property;
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}