using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class TitleAttributeDrawer : Z3AttributeDrawer<TitleAttribute>
    {
        protected override void Draw()
        {
            TitleBuilder.SetupTitle(Attribute, VisualElement);
        }
    }
}