using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Data;
using Z3.UIBuilder.Core;
using Z3.Utils;

namespace Z3.UIBuilder
{
    public static class UIElementBuilder
    {
        /// <summary>
        /// Binds the fields of the specified target object to VisualElements in the provided root element.
        /// </summary>
        public static void BindFields<T>(T target, VisualElement root, bool everything = false) where T : class
        {
            IEnumerable<MemberInfo> members = ReflectionUtils.GetAllMembers(target);

            if (!everything)
            {
                members = members.Where(f => f.GetCustomAttribute(typeof(UIElementAttribute)) != null);
            }

            foreach (MemberInfo member in members)
            {
                UIElementAttribute attribute = member.GetCustomAttribute<UIElementAttribute>();
                VisualElement element;

                bool optional = false;
                bool hasName = false;
                if (attribute != null)
                {
                    optional = attribute.Optional;
                    hasName = !string.IsNullOrEmpty(attribute.ElementName);
                }

                if (hasName)
                {
                    // Try to find using Attribute
                    element = root.Q(attribute.ElementName);

                    if (element == null)
                    {
                        if (!optional)
                        {
                            Debug.LogError($"Couldn't find a VisualElement with name '{attribute.ElementName}'");
                        }
                        continue;
                    }
                }
                else
                {
                    // Try to find using declaration and formatting (variable-pattern)
                    string name = StringFormater.FormatToLowerCaseAndHyphenateWords(member.Name);
                    element = root.Q(name);

                    // Try to find using declaration (original)
                    element ??= root.Q(member.Name);

                    // Try to find using default
                    element ??= root.Q("PropertyField:" + member.Name);

                    if (element == null)
                    {
                        if (!optional)
                        {
                            Debug.LogError($"Couldn't find a VisualElement with name '{member.Name}'");
                        }
                        continue;
                    }
                }

                // Bind UI Element
                switch (member)
                {
                    case FieldInfo fieldInfo:
                        if (fieldInfo.FieldType.IsAssignableFrom(element.GetType()))
                        {
                            fieldInfo.SetValue(target, element);
                        }
                        else
                        {
                            Debug.LogError($"Couldn't bind a VisualElement name of '{member.Name}'.\nField Type {fieldInfo.FieldType}, expected type {element.GetType()}");
                        }
                        break;
                    case MethodInfo methodInfo:
                        if (element is Button button)
                        {
                            button.clicked += () =>
                            {
                                try
                                {
                                    methodInfo.Invoke(target, null);
                                }
                                catch (System.Exception e)
                                {
                                    Debug.LogError(e);
                                }
                            };
                        }
                        break;
                }
            }
        }
    }
}
