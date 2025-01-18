using UnityEditor.EditorTools;
using UnityEngine;

namespace Z3.UIBuilder.Editor
{
    public abstract class Z3EditorTool<T> : EditorTool where T : MonoBehaviour
    {
        protected T Target => target as T;

    }
}