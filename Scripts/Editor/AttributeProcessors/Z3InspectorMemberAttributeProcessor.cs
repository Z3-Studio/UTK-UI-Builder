using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public abstract class Z3InspectorMemberAttributeProcessor
    {
        internal abstract void ProcessMembers(object target, IEnumerable<MemberInfo> members, VisualElement root);
    }

    public abstract class Z3InspectorMemberAttributeProcessor<TAttribute> : Z3InspectorMemberAttributeProcessor where TAttribute : Z3InspectorMemberAttribute
    {
        internal sealed override void ProcessMembers(object target, IEnumerable<MemberInfo> members, VisualElement root)
        {
            // Process multiple
            foreach (MemberInfo memberInfo in members)
            {
                IEnumerable<TAttribute> attributes = memberInfo.GetCustomAttributes<TAttribute>();

                foreach (TAttribute attribute in attributes)
                {
                    Process(target, memberInfo, attribute,  root);
                }
            }
        }

        public virtual void Process(object target, MemberInfo member, TAttribute attribute, VisualElement root) { }
    }
}