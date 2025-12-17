using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="OnCloseInspectorAttributeProcessor"/>
    /// </summary>
    public class OnCloseInspectorPreview
    {
        [OnCloseInspector]
        public void OnCloseInspector()
        {
            Debug.Log("Inspector was removed");
        }
    }
}
