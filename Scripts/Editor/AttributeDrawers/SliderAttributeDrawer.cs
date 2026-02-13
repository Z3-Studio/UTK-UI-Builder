using System.Reflection;
using UnityEngine;
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
            return MemberInfo.IsAssignableFromAny(typeof(int), typeof(float), typeof(Vector2), typeof(Vector2Int), typeof(Vector3), typeof(Vector3Int), typeof(Vector4));
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

        //private void ReplaceFieldVector2Int()
        //{
        //    VisualElement container = new VisualElement();
        //    container.AddToClassList("unity-base-field__aligned");
        //    container.style.flexDirection = FlexDirection.Column;

        //    Label title = new Label(SerializedProperty.displayName);
        //    container.Add(title);

        //    VisualElement row = new VisualElement();
        //    row.style.flexDirection = FlexDirection.Row;

        //    SliderInt xSlider = new SliderInt("X", (int)Attribute.Min, (int)Attribute.Max);
        //    xSlider.bindingPath = SerializedProperty.propertyPath + ".x";
        //    xSlider.style.flexGrow = 1;

        //    SliderInt ySlider = new SliderInt("Y", (int)Attribute.Min, (int)Attribute.Max);
        //    ySlider.bindingPath = SerializedProperty.propertyPath + ".y";
        //    ySlider.style.flexGrow = 1;

        //    row.Add(xSlider);
        //    row.Add(ySlider);

        //    container.Add(row);

        //    if (Attribute.ShowValue)
        //    {
        //        IntegerField xField = new IntegerField();
        //        xField.style.width = 50;
        //        xField.bindingPath = SerializedProperty.propertyPath + ".x";

        //        IntegerField yField = new IntegerField();
        //        yField.style.width = 50;
        //        yField.bindingPath = SerializedProperty.propertyPath + ".y";

        //        row.Add(xField);
        //        row.Add(yField);
        //    }

        //    container.Bind(SerializedProperty.serializedObject);

        //    VisualElement.Add(container);
        //}
    }
}