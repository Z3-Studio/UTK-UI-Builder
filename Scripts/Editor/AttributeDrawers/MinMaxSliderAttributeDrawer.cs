using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class MinMaxSliderAttributeDrawer : Z3AttributeDrawer<MinMaxSliderAttribute>
    {
        protected override bool CanDraw()
        {
            // Check if is Vector2 or Vector2Int
            return MemberInfo.IsAssignableFromAny(typeof(Vector2), typeof(Vector2Int));
        }

        protected override void Draw()
        {
            ClearVisualElement();

            if (MemberInfo.IsAssignableFrom(typeof(Vector2)))
            {
                ReplaceField();
                return;
            }
            
            if (MemberInfo.IsAssignableFrom(typeof(Vector2Int)))
            {
                ReplaceFieldVector2Int();
            }
        }

        private void ReplaceField()
        {
            MinMaxSlider slider = new MinMaxSlider(label: SerializedProperty.displayName, minLimit: Attribute.Min, maxLimit: Attribute.Max);
            slider.AddToClassList("unity-base-field__aligned"); // Set right space
            slider.BindProperty(SerializedProperty);
            VisualElement.Add(slider);

            AddFields(slider);
        }

        private void ReplaceFieldVector2Int()
        {
            Vector2Int currentValue = SerializedProperty.vector2IntValue;

            MinMaxSlider slider = new MinMaxSlider(
                SerializedProperty.displayName,
                Attribute.Min,
                Attribute.Max
            );

            slider.AddToClassList("unity-base-field__aligned");
            slider.value = new Vector2(currentValue.x, currentValue.y);

            // TODO: Bind
            slider.RegisterValueChangedCallback(evt =>
            {
                Vector2 previousValue = evt.previousValue;
                Vector2 newValue = evt.newValue;

                bool minChanged = !Mathf.Approximately(newValue.x, previousValue.x);
                bool maxChanged = !Mathf.Approximately(newValue.y, previousValue.y);

                int minValue;
                int maxValue;

                if (minChanged)
                {
                    minValue = newValue.x < previousValue.x
                        ? Mathf.FloorToInt(newValue.x)
                        : Mathf.CeilToInt(newValue.x);

                    maxValue = Mathf.RoundToInt(newValue.y);
                }
                else
                {
                    minValue = Mathf.RoundToInt(newValue.x);

                    maxValue = newValue.y < previousValue.y
                        ? Mathf.FloorToInt(newValue.y)
                        : Mathf.CeilToInt(newValue.y);
                }

                Vector2Int result = new(minValue, maxValue);

                // TODO: Review it
                SerializedProperty.serializedObject.Update();
                SerializedProperty.vector2IntValue = result;
                SerializedProperty.serializedObject.ApplyModifiedProperties();

                if (result != newValue)
                    slider.value = newValue;
            });

            VisualElement.Add(slider);
            AddFields(slider);
        }

        private void AddFields(MinMaxSlider slider)
        {
            if (!Attribute.ShowValue)
                return;

            // TODO: Improve style
            FloatField minText = new FloatField();
            minText.value = Attribute.Min;
            minText.style.width = 50; // unity-base-slider__text-field

            FloatField maxText = new FloatField();
            maxText.value = Attribute.Max;
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