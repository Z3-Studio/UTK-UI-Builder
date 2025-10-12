using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class MonoScriptView : ObjectField
    {
        public MonoScriptView(Object obj, SerializedObject serializedObject) : this(obj)
        {
            bindingPath = "m_Script";
            this.Bind(serializedObject);
        }

        public MonoScriptView(Object obj) : base()
        {
            MonoScript monoScript;
            if (obj is ScriptableObject so)
            {
                monoScript = MonoScript.FromScriptableObject(so);
            }
            else
            {
                monoScript = MonoScript.FromMonoBehaviour((MonoBehaviour)obj);
            }

            value = monoScript;
            label = "Script";

            SetEnabled(false);
            this.AddAlignedStyle();
        }
    }
}
