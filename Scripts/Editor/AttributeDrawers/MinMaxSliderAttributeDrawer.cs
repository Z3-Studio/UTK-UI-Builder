using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class MinMaxSliderAttributeDrawer : Z3AttributeDrawer<MinMaxSliderAttribute>
    {
        protected override bool CanDraw()
        {
            // Check if is Vector2 or Vector2Int
            return MemberInfo.IsAssignableFrom(typeof(Vector2));
        }

        protected override void Draw()
        {
            // TODO: The first visualization is not showing
            Vector2Field vector2Field = VisualElement.Q<Vector2Field>();

            if (vector2Field != null)
            {
                Vector2 value = GetResolvedValue<Vector2>();
                ReplaceField(vector2Field, value.x, value.y);
                return;
            }
            
            Vector2IntField vector2IntField = VisualElement.Q<Vector2IntField>();
            if (vector2IntField != null)
            {
                // TODO: It doesn't serialize
                //ReplaceField(vector2IntField, vector2IntField.value.x, vector2IntField.value.y);
            }
        }

        private void ReplaceField<TValueType>(BaseField<TValueType> visualElement, float min, float max)
        {
            MinMaxSlider slider = new MinMaxSlider(visualElement.label, min, max, Attribute.Min, Attribute.Max);
            slider.AddToClassList("unity-base-field__aligned"); // Set right space
            slider.TransferBinding(visualElement, SerializedProperty);

            visualElement.parent.Add(slider);
            visualElement.RemoveFromHierarchy();

            if (!Attribute.ShowValue)
                return;

            // TODO: Improve style
            FloatField minText = new FloatField();            
            minText.value = min;
            minText.style.width = 50; // unity-base-slider__text-field

            FloatField maxText = new FloatField();
            maxText.value = max;
            maxText.style.width = 50;

            slider.Insert(1, minText);
            slider.Insert(3, maxText);

            minText.RegisterValueChangedCallback(c =>
            {
                float newMin = c.newValue;
                float newMax = slider.value.y;
                if (newMin > newMax)
                {
                    newMax = newMin;
                }

                slider.value = new Vector2(newMin, newMax);
            });

            maxText.RegisterValueChangedCallback(c =>
            {
                float newMin = slider.value.x;
                float newMax = c.newValue;
                if (newMax < newMin)
                {
                    newMin = newMax;
                }

                slider.value = new Vector2(newMin, newMax);
            });

            slider.RegisterValueChangedCallback(v =>
            {
                minText.SetValueWithoutNotify(v.newValue.x);
                maxText.SetValueWithoutNotify(v.newValue.y);
            });
        }
    }
}