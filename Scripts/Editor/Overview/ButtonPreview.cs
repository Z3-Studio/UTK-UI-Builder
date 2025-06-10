using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="ButtonAttributeProcessor"/>
    /// </summary>
    public class ButtonPreview
    {
        public string test; // TODO: Remove this. For some reason add a basic field is required to appear buttons
        [BuildUIElement] public Button declarationExample;
        [BuildUIElement] public Button declarationExample2 = new() { text = "Hi" };

        [SerializeField] private Button nonVisible;

        [OnInitInspector]
        public void OnCreate()
        {
            declarationExample.text = nameof(declarationExample).ToNiceString();
            declarationExample.clicked += () =>
            {
                UnityEditor.EditorUtility.DisplayDialog(nameof(ButtonPreview), $"You pressed {nameof(declarationExample)}", "Ok");
            }; 
            
            declarationExample.clicked += () =>
            {
                UnityEditor.EditorUtility.DisplayDialog(nameof(ButtonPreview), $"You pressed {nameof(declarationExample2)}", "Ok");
            };
        }

        [Button]
        public void MethodExample()
        {
            UnityEditor.EditorUtility.DisplayDialog(nameof(ButtonPreview), $"You pressed {nameof(MethodExample)}", "Ok");
        }
    }
}