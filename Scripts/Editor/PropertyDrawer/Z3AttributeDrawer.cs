using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public interface IZ3AttributeDrawer
    {
        void Init(SerializedProperty serializedProperty, VisualElement visualElement, MemberInfo memberInfo, Z3VisualElementAttribute attribute);
        bool CanDraw();
        void Draw();
    }

    public abstract class Z3AttributeDrawer<TAttribute> : IZ3AttributeDrawer where TAttribute : Z3VisualElementAttribute
    {
        protected SerializedProperty SerializedProperty { get; private set; }
        protected VisualElement VisualElement { get; private set; }
        protected MemberInfo MemberInfo { get; private set; }
        protected TAttribute Attribute { get; private set; }

        public TValue GetResolvedValue<TValue>() => SerializedProperty.GetValue<TValue>();

        bool IZ3AttributeDrawer.CanDraw() => CanDraw();

        void IZ3AttributeDrawer.Init(SerializedProperty serializedProperty, VisualElement visualElement, MemberInfo memberInfo, Z3VisualElementAttribute attribute)
        {
            SerializedProperty = serializedProperty;
            VisualElement = visualElement;
            MemberInfo = memberInfo;
            Attribute = (TAttribute)attribute;
        }

        void IZ3AttributeDrawer.Draw()
        {
            if (!CanDraw())
                return;

            Draw();
        }

        protected virtual bool CanDraw() => true;

        // Note: Maybe create AttachDraw
        protected virtual void Draw() { }

    }
}