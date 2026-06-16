using UnityEngine;
using UnityEngine.UIElements;
using Z3.Utils.Editor;

namespace Z3.UIBuilder.Editor
{
    [UxmlElement]
    public partial class ColorPickerField : BaseField<Color>
    {
        private static Texture2D checkerTexture;

        // Sliders
        private readonly DragFloatField redField;
        private readonly DragFloatField greenField;
        private readonly DragFloatField blueField;
        private readonly DragFloatField alphaField;

        // Color picker background
        private readonly VisualElement colorPreview;
        private readonly VisualElement colorSwatch;
        private readonly ColorPalette colorPalette;

        public ColorPickerField() : this("Color Picker") { }

        public ColorPickerField(string label) : base(label, new VisualElement())
        {
            AddToClassList(alignedFieldUssClassName);

            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.FlexStart;

            VisualElement input = this.Q<VisualElement>(className: inputUssClassName);

            VisualElement channelsContainer = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    marginLeft = 2,
                    marginRight = 4
                }
            };

            redField = new DragFloatField("R") { FillColor = Color.red, CompactLabel = true };
            greenField = new DragFloatField("G") { FillColor = Color.green, CompactLabel = true };
            blueField = new DragFloatField("B") { FillColor = Color.blue, CompactLabel = true };
            alphaField = new DragFloatField("A") { FillColor = new Color(0.55f, 0.55f, 0.55f), CompactLabel = true };

            channelsContainer.Add(redField);
            channelsContainer.Add(greenField);
            channelsContainer.Add(blueField);
            channelsContainer.Add(alphaField);

            colorPreview = new()
            {
                style =
                {
                    width = 72,
                    height = 72,
                    flexShrink = 0,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    backgroundImage = new StyleBackground(GetCheckerTexture()),
                    backgroundSize = new BackgroundSize(8, 8)
                }
            };

            colorSwatch = new()
            {
                style =
                {
                    width = Length.Percent(88),
                    height = Length.Percent(88),
                    borderTopLeftRadius = Length.Percent(50),
                    borderTopRightRadius = Length.Percent(50),
                    borderBottomLeftRadius = Length.Percent(50),
                    borderBottomRightRadius = Length.Percent(50)
                }
            };

            colorPreview.Add(colorSwatch);
            colorPreview.RegisterCallback<PointerDownEvent>(OnPreviewClicked);
            colorSwatch.RegisterCallback<PointerDownEvent>(OnPreviewClicked);

            VisualElement topRow = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1
                }
            };
            topRow.Add(channelsContainer);
            topRow.Add(colorPreview);

            colorPalette = new ColorPalette();
            colorPalette.BindField(this);

            input.style.flexDirection = FlexDirection.Column;
            input.style.flexGrow = 1;
            input.Add(topRow);
            input.Add(colorPalette);

            // Register Callbacks
            redField.RegisterValueChangedCallback(evt => value = new(evt.newValue, value.g, value.b, value.a));
            greenField.RegisterValueChangedCallback(evt => value = new(value.r, evt.newValue, value.b, value.a));
            blueField.RegisterValueChangedCallback(evt => value = new(value.r, value.g, evt.newValue, value.a));
            alphaField.RegisterValueChangedCallback(evt => value = new(value.r, value.g, value.b, evt.newValue));
        }

        public override void SetValueWithoutNotify(Color newValue)
        {
            base.SetValueWithoutNotify(newValue);

            redField.SetValueWithoutNotify(newValue.r);
            greenField.SetValueWithoutNotify(newValue.g);
            blueField.SetValueWithoutNotify(newValue.b);
            alphaField.SetValueWithoutNotify(newValue.a);

            colorSwatch.style.backgroundColor = newValue;
        }

        private void OnPreviewClicked(PointerDownEvent evt)
        {
            if (evt.button != (int)MouseButton.LeftMouse)
                return;

            EditorUtils.ShowColorPicker(c => value = c, value);

            evt.StopPropagation();
        }

        private static Texture2D GetCheckerTexture()
        {
            if (checkerTexture)
                return checkerTexture;

            checkerTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat,
                hideFlags = HideFlags.HideAndDontSave
            };

            Color light = new(0.82f, 0.82f, 0.82f);
            Color dark = new(0.58f, 0.58f, 0.58f);
            checkerTexture.SetPixels(new[] { light, dark, dark, light });
            checkerTexture.Apply();

            return checkerTexture;
        }
    }
}
