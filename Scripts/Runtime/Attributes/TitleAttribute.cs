using System;
using UnityEngine;

namespace Z3.UIBuilder.Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class TitleAttribute : Z3VisualElementAttribute
    {
        public string Text { get; set; }
        public TextAnchor TextAnchor { get; set; }

        /// <summary> Apply <see cref="Utils.ExtensionMethods.StringExtensions.ToNiceString"/> to the text </summary>
        public bool Format { get; set; }

        public TitleAttribute(string header, TextAnchor textAnchor = TextAnchor.LowerLeft, bool format = false)
        {
            Text = header;
            TextAnchor = textAnchor;
            Format = format;
        }
    }
}
