using System.Reflection;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class InfoBoxAttributeProcessor : Z3InspectorMemberAttributeProcessor<InfoBoxAttribute>
    {
        public override void Process(object target, MemberInfo member, InfoBoxAttribute attribute, VisualElement root)
        {
            HelpBox csharpHelpBox = new HelpBox(attribute.Message, attribute.MessageType);
            csharpHelpBox.AddToClassList("some-styled-help-box");

            int index = root.parent.IndexOf(root);
            root.parent.Insert(index, csharpHelpBox);
        }
    }
}