using System;

namespace Z3.UIBuilder.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public sealed class ReadOnlyAttribute : Z3VisualElementAttribute
    {
        // TODO: Implement classes and Search Mode
        public enum SearchClassMode
        {
            OnlyOwner,
            AllHierarchy,
            OwnerAndParents
        }

        public ReadOnlyAttribute(SearchClassMode searchMode = SearchClassMode.OwnerAndParents)
        {

        }
    }
}