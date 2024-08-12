using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class TitleAttributeDrawer : Z3AttributeDrawer<TitleAttribute>
    {
        protected override void Draw()
        {
            TitleView title = new TitleView(Attribute);

            int index = VisualElement.parent.IndexOf(VisualElement);
            VisualElement.parent.Insert(index, title);
        }
    }
}