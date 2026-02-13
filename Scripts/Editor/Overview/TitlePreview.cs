using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="TitleAttributeDrawer"/>
    /// </summary>
    public class TitlePreview
    {
        [Title("Title Example")]
        public GameObject field;

        //[Title("Title Example")] // TODO: Support to Properties
        [field: SerializeField] public GameObject Property { get; set; }
    }
}
