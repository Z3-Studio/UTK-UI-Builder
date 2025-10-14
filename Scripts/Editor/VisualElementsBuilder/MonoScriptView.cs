using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.UIBuilder.Editor
{
    public class MonoScriptView : ObjectField
    {
        public MonoScriptView(Object obj, SerializedObject serializedObject) : this(obj)
        {
            bindingPath = "m_Script";
            this.Bind(serializedObject);
        }

        public MonoScriptView(Object obj) : base("Script")
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

            SetEnabled(false);
            this.AddAlignedStyle();
        }

        public MonoScriptView(object item) : base("Script")
        {
            Type type = item.GetType();
            objectType = typeof(MonoScript);
            SetEnabled(false);
            this.AddAlignedStyle();

            // Quickly search
            string[] guids = AssetDatabase.FindAssets($"{type.Name} t:MonoScript");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script.GetClass() == type)
                {
                    value = script;
                    return;
                }
            }

            value = new TextAsset()
            {
                name = "⚠️ File no available"
            };
            tooltip = "Move to a different file with same name of the class";

            //// Deep search: SUPER SLOW, and not reliable
            //Regex declarationRegex = new Regex($@"\b(public|private|internal|protected)?\s*(class|struct)\s+{Regex.Escape(type.Name)}\b", RegexOptions.Compiled);

            //guids = AssetDatabase.FindAssets("t:MonoScript");
            //foreach (string guid in guids)
            //{
            //    string path = AssetDatabase.GUIDToAssetPath(guid);
            //    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

            //    if (declarationRegex.IsMatch(script.text))
            //    {
            //        value = script;
            //        return;
            //    }
            //}
        }
    }
}
