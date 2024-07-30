namespace Z3.UIBuilder.Core
{
    public sealed class MinMaxSliderAttribute : Z3VisualElementAttribute
    {
        public float Min { get; }
        public float Max { get; }
        public bool ShowValue { get; } = true;

        public MinMaxSliderAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}