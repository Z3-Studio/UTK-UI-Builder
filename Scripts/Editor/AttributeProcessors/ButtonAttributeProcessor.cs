using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class ButtonAttributeProcessor : Z3InspectorMemberAttributeProcessor<ButtonAttribute>
    {
        public override void Process(object target, MemberInfo member, ButtonAttribute attribute, VisualElement root)
        {
            MethodInfo method = (MethodInfo)member;
            Button button = ButtonBuilder.CreateButtonFromMethod(target, method, attribute);

            // Nested objects
            if (root is PropertyField prop)
            {
                prop.Q<Foldout>().Add(button);
            }
            else
            {
                root.Add(button);
            }
        }
    }
}