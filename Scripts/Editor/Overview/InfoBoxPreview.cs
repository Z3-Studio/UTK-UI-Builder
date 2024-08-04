using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="InfoBoxAttributeProcessor"/>
    /// </summary>
    public class InfoBoxPreview
    {
        [InfoBox("Box Message with HelpBoxMessageType.None argument", HelpBoxMessageType.None)]
        public string none;

        [InfoBox("Box Message with HelpBoxMessageType.Info argument", HelpBoxMessageType.Info)]
        public string info;

        [InfoBox("Box Message with HelpBoxMessageType.Warning argument", HelpBoxMessageType.Warning)]
        public string warning;

        [InfoBox("Box Message with HelpBoxMessageType.Error argument", HelpBoxMessageType.Error)]
        public string error;
    }
}