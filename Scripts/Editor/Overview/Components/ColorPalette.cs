using System;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class ColorPalette : VisualElement
    {
        private event Action<Color> OnColorPicked;

        private Color currentColor;
        private VisualElement swatchesContainer;

        private const float SwatchWidth = 28f;
        private const float SwatchHeight = 18f;

        public ColorPalette()
        {
            // Populate swatches
            RegisterCallbackOnce<AttachToPanelEvent>(e => InitializeUI());
        }

        private void InitializeUI()
        {
            // TODO: Fix it
            /*if (panel?.contextType == ContextType.Editor)
            {
                Label label = new("Color Palette: Not available in UI Builder Editor")
                {
                    style =
                    {
                        height = SwatchHeight,
                        fontSize = 12,
                        color = Color.gray,
                        backgroundColor = Color.gray2,
                        unityTextAlign = TextAnchor.MiddleCenter
                    }
                };

                Add(label);
                return;
            }*/

            // Main container
            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.FlexStart;

            // Add button
            Button addNewColor = new(OnAddColor)
            {
                text = "+"
            };

            addNewColor.style.width = SwatchWidth;
            addNewColor.style.height = SwatchHeight;

            addNewColor.style.SetPadding(0f);
            addNewColor.style.SetBorderWidth(1f);
            addNewColor.style.SetBorderColor(Color.black);
            Add(addNewColor);

            // Swatches container
            swatchesContainer = new();
            swatchesContainer.style.flexDirection = FlexDirection.Row;
            swatchesContainer.style.flexWrap = Wrap.Wrap;
            Add(swatchesContainer);

            RefreshSwatches();
        }

        private void RefreshSwatches()
        {
            swatchesContainer.Clear();

            foreach (Color color in ColorPaletteAsset.GetColors())
            {
                // Button container
                float alpha = color.a;
                VisualElement swatchContainer = new()
                {
                    style =
                    {
                        width = SwatchWidth,
                        height = SwatchHeight,
                        backgroundColor = color.SetAlpha(1f)
                    }
                };
                swatchContainer.style.SetBorderWidth(1f);
                swatchContainer.style.SetBorderColor(Color.black);

                swatchContainer.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (evt.button == 0)
                    {
                        OnSelectColor(color);
                    }
                    if (evt.button == 1)
                    {
                        OnRemoveColor(color);
                    }
                });

                // Alpha bar
                VisualElement alphaBar = new()
                {
                    pickingMode = PickingMode.Ignore,
                    style =
                    {
                        position = Position.Absolute,
                        left = 0,
                        bottom = 0,
                        height = 3,
                        width = Length.Percent(alpha * 100f),
                        backgroundColor = Color.white
                    }
                };

                swatchContainer.Add(alphaBar);
                swatchesContainer.Add(swatchContainer);
            }
        }

        public void BindField(BaseField<Color> field)
        {
            OnColorPicked = c => field.value = c;
            field.RegisterValueChangedCallback(evt => currentColor = evt.newValue);
            currentColor = field.value;
        }

        // User interactions
        private void OnSelectColor(Color color)
        {
            currentColor = color;
            OnColorPicked?.Invoke(color);
            RefreshSwatches();
        }

        private void OnAddColor()
        {
            ColorPaletteAsset.AddColor(currentColor);
            RefreshSwatches();
        }

        private void OnRemoveColor(Color color)
        {
            ColorPaletteAsset.RemoveColor(color);
            RefreshSwatches();
        }
    }
}
