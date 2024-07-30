using System;

namespace Z3.UIBuilder.Core
{
    public sealed class PropertySettingsAttribute : Z3VisualElementAttribute // Check item, remove from root, move chields to parent
    {
        public enum Definition
        {
            Box,
            BoxFoldout,
            Inline,
            InlineWithoutLabel
        }

        public bool Label { get; }
        public bool Foldout { get; }
        public bool Box { get; }

        public PropertySettingsAttribute(bool label = true, bool foldout = false, bool box = true)
        {
            Box = label;
            Foldout = foldout;
            Label = box;
        }

        public PropertySettingsAttribute(Definition definition)
        {
            switch (definition)
            {
                case Definition.Box:
                    Label = true;
                    Box = true;
                    Foldout = false;
                    break;
                case Definition.BoxFoldout:
                    Label = true;
                    Box = true;
                    Foldout = true;
                    break;
                case Definition.Inline:
                    Label = true;
                    Box = false;
                    Foldout = false;
                    break;
                case Definition.InlineWithoutLabel:
                    Label = false;
                    Box = false;
                    Foldout = false;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    } 

    // SceneObject vs Asset Only
}