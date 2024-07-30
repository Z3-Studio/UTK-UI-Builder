using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public static class SliderIntBuilder
    {
        internal static SliderInt CreateSliderIntFromVisualElement(FieldInfo field, object target)
        {
            RangeAttribute range = field.GetCustomAttribute<RangeAttribute>();

            if (field.GetValue(target) is not SliderInt sliderInt)
            {
                sliderInt = new SliderInt();
                SetSliderInt(sliderInt, field.Name);

                range ??= new RangeAttribute(sliderInt.lowValue, sliderInt.highValue);
            }

            if (range != null)
            {
                sliderInt.lowValue = (int)range.min;
                sliderInt.highValue = (int)range.max;
            }

            // Bind
            field.SetValue(target, sliderInt);

            return sliderInt;
        }

        internal static SliderInt CreateSliderIntFromInt(FieldInfo field, RangeAttribute range, object target)
        {
            // <<<<<<<<<<<<<<<<, TODO: VERY IMPORTANT!! >>>>>>>>>>>>>>>>.
            // TODO: Bind variable
            //SerializedProperty property = new SerializedObject(target).FindProperty(propertyPath);
            //PropertyField intProperty = new PropertyField(property);

            SliderInt sliderInt = new SliderInt();
            SetSliderInt(sliderInt, field.Name, (int)field.GetValue(target));

            sliderInt.lowValue = (int)range.min;
            sliderInt.highValue = (int)range.max;

            if (sliderInt.value < sliderInt.lowValue)
            {
                field.SetValue(target, sliderInt.lowValue);
            }
            else if (sliderInt.value > sliderInt.highValue)
            {
                field.SetValue(target, sliderInt.highValue);
            }


            sliderInt.RegisterValueChangedCallback(e =>
            {
                field.SetValue(target, e.newValue);
            });

            return sliderInt;
        }

        internal static void SetSliderInt(SliderInt sliderInt, string fieldName, int value = 0)
        {
            sliderInt.name = fieldName;
            sliderInt.label = fieldName.GetNiceString();
            sliderInt.showInputField = true;
            sliderInt.value = sliderInt.value > sliderInt.highValue ? sliderInt.highValue : value;
        }
    }
}