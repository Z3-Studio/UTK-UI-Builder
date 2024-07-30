using UnityEditor;
using UnityEngine.UIElements;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.Utils;

namespace Z3.UIBuilder.Editor
{
    public class EditorDebugWindow : EditorWindow
    {
        private Label visualElementCounter;
        private Label baseFieldCounter;

        [MenuItem(Z3Path.UiBuilderMenuPath + "Debug")]
        public static void OpenWindow()
        {
            GetWindow<EditorDebugWindow>("Debug");
        }

        private void CreateGUI()
        {
            visualElementCounter = new Label();
            baseFieldCounter = new Label();
            rootVisualElement.Add(visualElementCounter);
            rootVisualElement.Add(baseFieldCounter);
        }

        private void Update()
        {
            visualElementCounter.text = $"Visual Element: {UIBuilderEditorExtensions.VisualElementCounter}";
            baseFieldCounter.text = $"Base Fields: {UIBuilderEditorExtensions.BaseFieldCounter}";
        }
    }
}