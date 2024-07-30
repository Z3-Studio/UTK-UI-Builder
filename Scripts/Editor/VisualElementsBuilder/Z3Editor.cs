using UnityEngine;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    using Editor = UnityEditor.Editor;

    public abstract class Z3Editor<T> : Editor where T : Object // TODO: Improve
    {
        /// <summary> Object To Draw </summary>
        protected T Target => target as T;

        public override VisualElement CreateInspectorGUI()
        {
            return EditorBuilder.BuildEditor(this);
        }
    }
}