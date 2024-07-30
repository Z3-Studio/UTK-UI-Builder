using System.Reflection;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class SliderAttributeDrawer : Z3AttributeDrawer<SliderAttribute>
    {
        protected override bool CanDraw()
        {
            // Check if is numeric
            return MemberInfo.IsAssignableFrom(typeof(float));
        }

        protected override void Draw()
        {
            // TODO: The first visualization is not showing
            IntegerField intField = VisualElement.Q<IntegerField>();

            if (intField != null)
            {
                ReplaceField(intField);
                return;
            }

            FloatField floatField = VisualElement.Q<FloatField>();
            if (floatField != null)
            {
                ReplaceField(floatField);
            }
        }

        private void ReplaceField<TValueType>(BaseField<TValueType> visualElement)
        {
            Slider slider = new Slider(visualElement.label, Attribute.Min, Attribute.Max);
            slider.AddToClassList("unity-base-field__aligned"); // Set right space
            slider.TransferBinding(visualElement, SerializedProperty);

            slider.showInputField = Attribute.ShowValue;

            visualElement.parent.Add(slider);
            visualElement.RemoveFromHierarchy();
        }
    }
}