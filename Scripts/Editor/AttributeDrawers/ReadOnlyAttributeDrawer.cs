using UnityEngine.UIElements;
using System.Reflection;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class ReadOnlyAttributeDrawer : Z3AttributeDrawer<ReadOnlyAttribute>
    {
        protected override void Draw()
        {
            VisualElement.SetEnabled(false);
        }

        public static void UpdateMember(VisualElement visualElement, MemberInfo memberInfo)
        {
            ReadOnlyAttribute attribute = memberInfo.GetCustomAttribute<ReadOnlyAttribute>();
            if (attribute == null)
                return;

            visualElement.SetEnabled(false);
        }
    }
}