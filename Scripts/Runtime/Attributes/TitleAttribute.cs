namespace Z3.UIBuilder.Core
{
    public sealed class TitleAttribute : Z3VisualElementAttribute
    {
        public string Text { get; set; }
        public TitleAttribute(string header)
        {
            Text = header;
        }
    }
}