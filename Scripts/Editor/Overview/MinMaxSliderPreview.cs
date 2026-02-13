using UnityEngine;
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
        public Vector2 defaultVector2;

        [MinMaxSlider(0, 100)]
        public Vector2 vector2WithFloats;

        [MinMaxSlider(0, 100, false)]
        public Vector2 vector2WithoutFloats;

        [MinMaxSlider(0, 100)]
        public Vector2Int vector2IntWithFloats;

        [MinMaxSlider(0, 100, false)]
        public Vector2Int vector2IntWithoutFloats;

        [BuildUIElement]
        public MinMaxSlider minMaxSliderElement = new()
        {
            label = nameof(minMaxSliderElement).ToNiceString(),
            lowLimit = 0,
            highLimit = 10
        };

        [Button]
        public void Test()
        {
            Debug.Log($"Name: {nameof(vector2WithFloats)}, value: {vector2WithFloats}");
            Debug.Log($"Name: {nameof(vector2WithoutFloats)}, value: {vector2WithoutFloats}");
            Debug.Log($"Name: {nameof(vector2IntWithFloats)}, value: {vector2IntWithFloats}");
            Debug.Log($"Name: {nameof(vector2IntWithoutFloats)}, value: {vector2IntWithoutFloats}");
            Debug.Log($"Name: {nameof(defaultVector2)}, value: {defaultVector2}");
            Debug.Log($"Name: {nameof(minMaxSliderElement)}, value: {minMaxSliderElement.value}");
        }
    }
}