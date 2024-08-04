using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="SliderAttributeDrawer"/>
    /// </summary>
    public class SliderPreview
    {
        [Slider(0f, 10f)]
        public float slider;
    }
}