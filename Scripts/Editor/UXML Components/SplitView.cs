using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    public class SplitView : TwoPaneSplitView
    {
        /// <summary>
        /// Used To Display Component
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
    }
}