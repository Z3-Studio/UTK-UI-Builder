using UnityEditor.EditorTools;
using UnityEngine;

namespace Z3.UIBuilder.Editor
{
    public abstract class Z3EditorTool<T> : EditorTool where T : Component
    {
        protected T Target => target as T;

    }
}