using UnityEngine;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    using Editor = UnityEditor.Editor;

    public abstract class Z3Editor<T> : Editor where T : Object // TODO: Improve
    {
        /// <summary> Object To Draw </summary>
        protected T Target => target as T;

        private VisualElement root;

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            root.Add(CreateVisualElement());
            return root;
        }

        protected virtual VisualElement CreateVisualElement() // TODO: Review redraw method used in DamageTable. Before that the method BuildEditor was used directly in CreateInspectorGUI
        {
            return EditorBuilder.BuildEditor(this);
        }

        protected void Redraw() // TODO: Review redraw method used in DamageTable 
        {
            root.Clear();
            root.Add(CreateVisualElement());
        }

        public void AddMonoScript(VisualElement element) => element.Add(new MonoScriptView(target, serializedObject));
    }
}
