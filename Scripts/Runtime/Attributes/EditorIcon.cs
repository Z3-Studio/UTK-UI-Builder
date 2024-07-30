using System;

namespace Z3.UIBuilder
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class EditorIcon : Attribute
    {
        public IconType Icon { get; }

        public EditorIcon(IconType iconType)
        {
            Icon = iconType;
        }
    }
}