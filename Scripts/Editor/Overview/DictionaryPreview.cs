using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class DictionaryPreview
    {
        public SerializableDictionary<string, string> dictionary = new SerializableDictionary<string, string>()
        {
            { "Key 1", "Value 1" },
            { "Key 2", "Value 2" },
            { "Key 3", "Value 3" },
        };
    }
}