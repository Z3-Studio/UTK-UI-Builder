using System.Reflection;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public static class ButtonBuilder
    {
        /// <param name="field"> Field type of <seealso cref="UnityEngine.UIElements.Button"/> </param>
        internal static Button CreateButtonFromVisualElementField(FieldInfo field, object target)
        {
            // Check if is null
            Button button = field.GetValue(target) as Button;
            if (button is null)
            {
                button = new Button();
                SetButton(button, field.Name, field.Name.GetNiceString());
            }

            // Bind
            field.SetValue(target, button);

            return button;
        }

        internal static Button CreateButtonFromMethod(object target, MethodInfo method, ButtonAttribute attribute)
        {
            Button button = new Button();

            string label = attribute.Name != null ? attribute.Name : method.Name.GetNiceString();
            SetButton(button, method.Name, label);

            button.clicked += () => method.Invoke(target, null);
            return button;
        }

        internal static void SetButton(Button button, string memberName, string label)
        {
            button.name = memberName;
            button.text = label;
        }
    }
}