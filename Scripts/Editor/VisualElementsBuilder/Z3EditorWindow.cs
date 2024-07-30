using UnityEditor;

namespace Z3.UIBuilder.Editor
{
    public abstract class Z3EditorWindow : EditorWindow
    {
        protected virtual void CreateGUI()
        {
            EditorBuilder.BuildEditorWindow(this);
        }
    }
}
