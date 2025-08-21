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

        private VisualElement root;

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            root.Add(CreateVisualElement());
            return root;
        }

        protected virtual VisualElement CreateVisualElement() // TODO: Review redraw method used in DamageTable. Before that the method BuildEditor was used directly in CreateInspectorGUI
        {
            return EditorBuilder.BuildEditor(this);
        }

        protected void Redraw() // TODO: Review redraw method used in DamageTable 
        {
            root.Clear();
            root.Add(CreateVisualElement());
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