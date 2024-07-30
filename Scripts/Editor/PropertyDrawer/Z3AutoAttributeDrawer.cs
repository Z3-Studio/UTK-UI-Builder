using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// If the field there is no <see cref="Z3PropertyDrawer{TProperty}"/>, this class will be called to draw all <see cref="Z3AttributeDrawer{TAttribute}"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(Z3VisualElementAttribute), true)]
    public sealed class Z3AutoAttributeDrawer : Z3Drawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Note: This check will create VisualElement just once, avoiding be called when have multiple Z3PropertyAttribute

            List<Z3VisualElementAttribute> attributes = fieldInfo.GetCustomAttributes<Z3VisualElementAttribute>(false).ToList();
            if (!attribute.Match(attributes[0]))
                return null;

            if (EditorBuilder.DrawByPropertyDrawer(property, fieldInfo, out VisualElement element))
                return element;

            return base.CreatePropertyGUI(property);
        }
    }
}