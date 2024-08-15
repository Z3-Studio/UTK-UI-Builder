using UnityEngine;

namespace Z3.UIBuilder.Core
{
    public sealed class TitleAttribute : Z3VisualElementAttribute
    {
        public string Text { get; set; }
        public TextAnchor TextAnchor { get; set; } 

        public TitleAttribute(string header, TextAnchor textAnchor = TextAnchor.LowerLeft)
        {
            Text = header;
            TextAnchor = textAnchor;
        }
    }
}