using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class HideAttributeDrawer : Z3AttributeDrawer<HideAttribute>
    {
        protected override void Draw()
        {
            VisualElement.style.display = DisplayStyle.None;
        }
    }
}