using System.Numerics;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="MinMaxSliderAttributeDrawer"/>
    /// </summary>
    public class MinMaxSliderPreview
    {
        public string a;

        [MinMaxSlider(0, 100)]
        public Vector2 minMaxSlider;

        public Vector2 minMaxSlider2;

        [BuildUIElement]
        public MinMaxSlider minMaxSliderElement = new()
        {
            label = nameof(minMaxSliderElement).ToNiceString(),
            lowLimit = 0,
            highLimit = 10
        };
    }
}