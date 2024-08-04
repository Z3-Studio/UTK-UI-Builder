using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="OnInitInspectorAttributeProcessor"/>
    /// </summary>
    public class OnInitInspectorPreview
    {
        [OnInitInspector]
        public void OnInitInspector()
        {
            Debug.Log("Inspector was built");
        }
    }
}