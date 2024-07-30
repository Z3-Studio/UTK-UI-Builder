using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class OnInitInspectorPreview
    {
        [OnInitInspector]
        public void OnInitInspector()
        {
            Debug.Log("Inspector was built");
        }
    }
}