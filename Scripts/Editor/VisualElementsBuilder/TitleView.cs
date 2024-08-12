using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using UnityEngine;

namespace Z3.UIBuilder.Editor
{
    public class TitleView : Label
    {
        public TitleView(string labelText = null) : base(labelText)
        {
            style.borderBottomColor = new StyleColor(new Color(.4f, .4f, .4f));
            style.borderBottomWidth = new StyleFloat(1);
            style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        public TitleView(TitleAttribute attribute) : base(attribute.Text)
        {
            style.marginLeft = 3;
            style.marginTop = 3;
            style.marginLeft = 3;

            //label.AddToClassList("unity-header-drawer__label");
            //VisualElement container = new VisualElement();
            //container.AddToClassList("unity-decorator-drawers-container");
            //container.Add(label);
            //element.parent.Insert(index, container);
        }

        public static void AddTitle(VisualElement root, string labelText)
        {
            TitleView label = new TitleView(labelText);
            root.Add(label);
        }
    }
}