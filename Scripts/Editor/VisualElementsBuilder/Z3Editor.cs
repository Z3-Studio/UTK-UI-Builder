using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    using Editor = UnityEditor.Editor;

    public abstract class Z3Editor<T> : Editor where T : Object // TODO: Improve
    {
        /// <summary> Object To Draw </summary>
        protected T Target => target as T;

        public override VisualElement CreateInspectorGUI()
        {
            return EditorBuilder.BuildEditor(this);
        }

        public ObjectField GetMonoScript()
        {
            MonoScript monoScript;
            if (target is ScriptableObject so)
            {
                monoScript = MonoScript.FromScriptableObject(so);
            }
            else
            {
                monoScript = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
            }

            ObjectField objectField = new ObjectField()
            {
                label = "Script",
                value = monoScript
            };

            objectField.SetEnabled(false);
            objectField.bindingPath = "m_Script";
            objectField.Bind(serializedObject);
            objectField.AddAlignedStyle();
            return objectField;
        }
    }
}