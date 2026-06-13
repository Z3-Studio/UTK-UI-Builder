using UnityEngine;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    [UxmlElement]
    public partial class DragFloatField : BaseField<float>
    {
        [UxmlAttribute] public float LowValue { get; set; } = 0f;
        [UxmlAttribute] public float HighValue { get; set; } = 1f;
        [UxmlAttribute] public Color FillColor { get; set; } = new(0.8f, 0.2f, 0.2f);

        [UxmlAttribute]
        public bool CompactLabel
        {
            get => compactLabel;
            set
            {
                compactLabel = value;
                ApplyLabelStyle();
            }
        }

        private readonly FloatField floatField;
        private readonly VisualElement fillElement;
        private readonly VisualElement dragArea;

        private bool compactLabel;
        private bool isDragging;

        public DragFloatField() : this(string.Empty) { }

        public DragFloatField(string label) : base(label, new VisualElement())
        {
            style.marginBottom = 1;

            VisualElement input = this.Q<VisualElement>(className: inputUssClassName);

            dragArea = new()
            {
                style =
                {
                    flexGrow = 1,
                    backgroundColor = new Color(0.18f, 0.18f, 0.18f),
                    minHeight = 18,
                    overflow = Overflow.Hidden
                }
            };

            fillElement = new()
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    position = Position.Absolute,
                    left = 0,
                    top = 0,
                    bottom = 0
                }
            };

            floatField = new FloatField
            {
                style =
                {
                    width = 52,
                    flexShrink = 0,
                    marginLeft = 2
                }
            };
            floatField.labelElement.style.display = DisplayStyle.None;

            dragArea.Add(fillElement);

            input.style.flexDirection = FlexDirection.Row;
            input.style.flexGrow = 1;
            input.style.alignItems = Align.Center;
            input.Add(dragArea);
            input.Add(floatField);

            dragArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
            dragArea.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            dragArea.RegisterCallback<PointerUpEvent>(OnPointerUp);
            floatField.RegisterValueChangedCallback(OnFloatFieldChanged);

            ApplyLabelStyle();
            SetValueWithoutNotify(0f);
        }

        private void ApplyLabelStyle()
        {
            if (compactLabel)
            {
                labelElement.style.width = 18;
                labelElement.style.minWidth = 18;
                labelElement.style.maxWidth = 18;
                labelElement.style.fontSize = 10;
                labelElement.style.unityTextAlign = TextAnchor.MiddleCenter;
                labelElement.style.paddingLeft = 0;
                labelElement.style.paddingRight = 0;
                labelElement.style.marginRight = 2;
                return;
            }

            labelElement.style.width = StyleKeyword.Auto;
            labelElement.style.minWidth = StyleKeyword.Auto;
            labelElement.style.maxWidth = StyleKeyword.Auto;
            labelElement.style.fontSize = StyleKeyword.Null;
            labelElement.style.unityTextAlign = StyleKeyword.Null;
            labelElement.style.paddingLeft = StyleKeyword.Null;
            labelElement.style.paddingRight = StyleKeyword.Null;
            labelElement.style.marginRight = StyleKeyword.Null;
        }

        private void OnFloatFieldChanged(ChangeEvent<float> evt)
        {
            value = Mathf.Clamp(evt.newValue, LowValue, HighValue);
            RefreshVisuals();
        }

        public override void SetValueWithoutNotify(float newValue)
        {
            base.SetValueWithoutNotify(Mathf.Clamp(newValue, LowValue, HighValue));
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            float normalized = Mathf.InverseLerp(LowValue, HighValue, value);

            fillElement.style.backgroundColor = FillColor;
            fillElement.style.width = Length.Percent(normalized * 100f);
            floatField.SetValueWithoutNotify(value);
        }

        private void SetValueFromPointer(float localX)
        {
            float width = dragArea.contentRect.width;
            if (width <= Mathf.Epsilon)
            {
                return;
            }

            float normalized = Mathf.Clamp01(localX / width);
            value = Mathf.Lerp(LowValue, HighValue, normalized);
            RefreshVisuals();
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != (int)MouseButton.LeftMouse)
            {
                return;
            }

            isDragging = true;
            dragArea.CapturePointer(evt.pointerId);
            SetValueFromPointer(evt.localPosition.x);
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isDragging)
            {
                return;
            }

            SetValueFromPointer(evt.localPosition.x);
            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!isDragging)
            {
                return;
            }

            isDragging = false;
            dragArea.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        }
    }
}
