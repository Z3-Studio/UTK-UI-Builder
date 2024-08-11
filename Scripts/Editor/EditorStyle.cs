using UnityEngine;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    public static class EditorStyle
    {
        public static readonly Color DarkLabel = new Color(.6f, .6f, .6f);

        public static void SetSmallEditorButton(IStyle style)
        {
            style.minWidth = new Length(18, LengthUnit.Pixel);
            style.borderTopWidth = 1f;
            style.borderBottomWidth = 1f;
            style.marginLeft = 10;
        }
    }
}