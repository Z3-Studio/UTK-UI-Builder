using System.Reflection;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class OnInitInspectorAttributeProcessor : Z3InspectorMemberAttributeProcessor<OnInitInspectorAttribute>
    {
        public override void Process(object target, MemberInfo member, OnInitInspectorAttribute attribute, VisualElement root)
        {
            MethodInfo method = (MethodInfo)member;
            method.Invoke(target, null);
        }
    }
}