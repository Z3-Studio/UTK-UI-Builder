using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Z3.Utils.ExtensionMethods;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// Async problems
    /// schedule: https://forum.unity.com/threads/pro...nly-be-queried-after-initial-ui-build.979092/
    /// https://forum.unity.com/threads/propertydrawer-with-uielements-changes-in-array-dont-refresh-inspector.747467/
    /// https://forum.unity.com/threads/uielements-listview-with-serializedproperty-of-an-array.719570/#post-4827677
    public class PropertySettingsAttributeDrawer : Z3AttributeDrawer<PropertySettingsAttribute>
    {
        protected override void Draw()
        {
            SetupProperty(Attribute, VisualElement);
        }

        internal static void UpdateMember(VisualElement propertyField, MemberInfo memberInfo)
        {
            PropertySettingsAttribute attribute = memberInfo.GetCustomAttribute<PropertySettingsAttribute>();
            if (attribute == null)
                return;

            SetupProperty(attribute, propertyField);
        }

        private static void SetupProperty(PropertySettingsAttribute attribute, VisualElement visualElement)
        {
            if (visualElement is not PropertyField propertyDrawer)
                return;

            if (attribute.Box)
            {
                Color boxColor = new(.3f, .3f, .3f);

                // Add box border and margin out the border
                propertyDrawer.parent.style.SetBorderColor(boxColor);
                propertyDrawer.parent.style.SetBorderWidth(2f);
                propertyDrawer.parent.style.SetMargin(5f);

                // Remove left margin and add spacing inside the box
                Foldout foldout = propertyDrawer.Q<Foldout>();
                foldout.contentContainer.style.marginLeft = 0f;
                foldout.contentContainer.style.paddingTop = 5f;
                foldout.contentContainer.style.paddingBottom = 5f;

                // Compress the toggle and add color
                Toggle toggle = foldout.Q<Toggle>();
                toggle.style.SetMargin(0f);
                toggle.style.backgroundColor = boxColor;

                if (!attribute.Foldout)
                {
                    // TODO: New container, add property and label, set label background
                    //VisualElement visualElement = new VisualElement();
                    //foldout.Remove(toggle);
                }
            }
            else if (!attribute.Foldout)
            {
                Foldout foldout = propertyDrawer.Q<Foldout>();
                foldout.contentContainer.style.marginLeft = 0f;

                foldout.parent.Add(foldout.contentContainer);
                propertyDrawer.Add(foldout.contentContainer.Q<PropertyField>());
                propertyDrawer.Remove(foldout);
            }
        }
    }

}