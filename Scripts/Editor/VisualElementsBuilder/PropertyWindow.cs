using UnityEngine;

namespace Z3.UIBuilder.Editor
{
    public class PropertyWindow : Z3EditorWindow
    {
        private object property;

        public static PropertyWindow OpenWindow(string title, object property)
        {
            PropertyWindow window = CreateInstance<PropertyWindow>();
            window.titleContent = new GUIContent(title);
            window.property = property;
            window.Show();

            return window;
        }

        protected override void CreateGUI()
        {
            if (property == null)
            {
                Close();
                return;
            }

            PropertyBuilder.DrawInstance(rootVisualElement, property);
        }
    }
}
