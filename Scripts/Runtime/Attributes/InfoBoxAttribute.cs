using UnityEngine.UIElements;

namespace Z3.UIBuilder.Core
{
    /// <summary>
    /// Display message with icon using <see cref="HelpBox"/> and <see cref="HelpBoxMessageType"/>
    /// </summary>
    public sealed class InfoBoxAttribute : Z3InspectorMemberAttribute 
    {
        public string Message { get; }
        public HelpBoxMessageType MessageType { get; }

        public InfoBoxAttribute(string message, HelpBoxMessageType helpBoxMessageType = HelpBoxMessageType.Info)
        {
            Message = message;
            MessageType = helpBoxMessageType;
        }
    } 
}