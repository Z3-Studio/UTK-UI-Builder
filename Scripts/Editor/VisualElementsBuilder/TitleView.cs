using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class TitleView : Label
    {
        public TitleView(string labelText = null) : base(labelText)
        {
            style.marginTop = 8f;
            style.marginBottom = 3f;
            style.marginLeft = 3f;
            style.borderBottomColor = new Color(.4f, .4f, .4f);
            style.borderBottomWidth = 1f;
            style.unityFontStyleAndWeight = FontStyle.Bold;

            //label.AddToClassList("unity-header-drawer__label");
            //VisualElement container = new VisualElement();
            //container.AddToClassList("unity-decorator-drawers-container");
            //container.Add(label);
            //element.parent.Insert(index, container);
        }

        public TitleView(TitleAttribute attribute) : this(attribute.Format ? attribute.Text.ToNiceString() : attribute.Text)
        {
            style.unityTextAlign = attribute.TextAnchor;
        }

        public static TitleView AddTitle(VisualElement root, string labelText)
        {
            TitleView label = new TitleView(labelText);
            root.Add(label);
            return label;
        }
    }
}
