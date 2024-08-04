using UnityEngine.UIElements;
using System.Reflection;
using Z3.UIBuilder.Core;
using UnityEngine;

namespace Z3.UIBuilder.Editor
{
    public static class TitleBuilder
    {
        public static void UpdateMember(VisualElement visualElement, MemberInfo memberInfo)
        {
            TitleAttribute attribute = memberInfo.GetCustomAttribute<TitleAttribute>();
            if (attribute == null)
                return;

            SetupTitle(attribute, visualElement);
        }

        public static void AddTitle(VisualElement root, string labelText)
        {
            Label label = GetTitle(labelText);
            root.Add(label);
        }

        public static Label GetTitle(string labelText = null)
        {
            Label label = new Label(labelText);
            label.style.borderBottomColor = new StyleColor(new Color(.4f, .4f, .4f));
            label.style.borderBottomWidth = new StyleFloat(1);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            return label;
        }

        public static void SetupTitle(TitleAttribute attribute, VisualElement element)
        {
            Label label = GetTitle(attribute.Text);
            label.style.marginLeft = 3;
            label.style.marginTop = 3;
            label.style.marginLeft = 3;

            int index = element.parent.IndexOf(element);
            element.parent.Insert(index, label);
        }

        static void Notes()
        {
            //label.AddToClassList("unity-header-drawer__label");
            //VisualElement container = new VisualElement();
            //container.AddToClassList("unity-decorator-drawers-container");
            //container.Add(label);
            //element.parent.Insert(index, container);
        }
    }
}