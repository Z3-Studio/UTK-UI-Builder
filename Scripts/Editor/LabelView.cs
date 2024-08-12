using System;
using System.Linq.Expressions;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class LabelView : VisualElement
    {
        [UIElement] private Label elementName;
        [UIElement] private Button removeButton;

        public int Index { get; private set; } = -1;
        public object Element { get; private set; }

        private Z3ListViewConfig Config => listView.Config;
        private IListView listView;

        public LabelView()
        {
            UIBuilderResources.ListElementVT.CloneTree(this);
            this.BindUIElements();
        }

        [UIElement("remove-button")]
        public void OnDelete()
        {
            listView.DeleteElement(Element);
        }

        public void Bind(IListView listView, object element, int i)
        {
            this.listView = listView;
            Element = element;
            Index = i;

            this.RegisterUpdate(UpdateLabel);

            void UpdateLabel() // TODO: Improve performance
            {
                elementName.text = ToStringElement(element);
            }

            if (!Config.showRemoveButton)
            {
                removeButton.visible = false;
            }
        }

        private string ToStringElement(object element) // TODO: Improve it
        {
            if (element == null)
                return "Null".ToItalic();

            string toString;

            if (string.IsNullOrEmpty(Config.toStringExpression))
            {
                toString = element.ToString();
            }
            else
            {
                toString = ExecuteToStringExpression(element, Config.toStringExpression);
            }

            if (!Config.toStringWithPrefix)
                return toString;

            string prefix = element.GetTypeNiceString();
            return prefix + " " + toString.AddRichTextColor(EditorStyle.DarkLabel);
        }

        // TODO: Move to static
        private string ExecuteToStringExpression(object obj, string toStringExpression)
        {
            ParameterExpression param = Expression.Parameter(obj.GetType(), "x");
            LambdaExpression lambda = Expression.Lambda(Expression.PropertyOrField(param, toStringExpression), param);
            Delegate func = lambda.Compile();
            return func.DynamicInvoke(obj) as string;
        }
    }
}