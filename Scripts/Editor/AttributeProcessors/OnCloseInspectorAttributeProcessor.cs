using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class OnCloseInspectorAttributeProcessor : Z3InspectorMemberAttributeProcessor<OnCloseInspectorAttribute>
    {
        public override void Process(object target, MemberInfo member, OnCloseInspectorAttribute attribute, VisualElement root)
        {
            MethodInfo method = (MethodInfo)member;

            root.RegisterCallback<DetachFromPanelEvent>(_ => 
            {
                try
                {
                    method.Invoke(target, null);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });
        }
    }
}
