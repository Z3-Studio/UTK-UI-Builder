using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="SliderAttributeDrawer"/>
    /// </summary>
    public class SliderPreview
    {
        [Slider(0f, 10f)]
        public float slider;

        [BuildUIElement] 
        public Slider sliderElement = new() 
        { 
            label = nameof(sliderElement).ToNiceString(), 
            lowValue = 0,
            highValue = 10,
            showInputField = true
        };
    }
}