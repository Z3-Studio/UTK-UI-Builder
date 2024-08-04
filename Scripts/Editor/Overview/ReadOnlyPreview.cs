using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="ReadOnlyAttributeDrawer"/>
    /// </summary>
    public class ReadOnlyPreview
    {
        [ReadOnly]
        public GameObject gameObject;
    }
}