using UnityEditor;
using UnityEngine;

namespace Z3.UIBuilder.Editor
{
    public class UIPopup : PopupWindowContent
    {
        protected Vector2 Size { get; set; } = new Vector2(400, 400);

        protected void OpenGenericPopup()
        {
            OpenGenericPopup(Event.current.mousePosition);
        }

        protected void OpenGenericPopup(Vector2 windowPosition)
        {
            Rect rect = new(windowPosition.x, windowPosition.y, 0f, 0f);
            PopupWindow.Show(rect, this);
        }

        public override Vector2 GetWindowSize() => Size;

        public override void OnGUI(Rect rect) { } // Intentionally left empty
    }
}