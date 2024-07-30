using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class ButtonPreview
    {
        [SerializeField] private Button declarationExample;
        public Button declarationExample2;

        [OnInitInspector]
        public void OnCreate()
        {
            declarationExample.clicked += () =>
            {
                Debug.Log("You pressed declaration example");
            };
        }

        [Button]
        public void MethodExample()
        {
            Debug.Log("You pressed method example");
        }
    }
}