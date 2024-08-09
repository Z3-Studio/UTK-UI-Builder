using UnityEditor;
using Z3.UIBuilder.Core;
using UnityEngine.UIElements;
using System.Collections;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Used to display <see cref="SerializableDictionary{TKey, TValue}"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(IDictionary), true)]
    public class DictionaryPropertyDrawer : Z3PropertyDrawer<IDictionary>
    {
        protected override VisualElement CreateVisualElement()
        {
            VisualElement root = new();

            HelpBox helpBox = new HelpBox();
            helpBox.text = "EXPERIMENTAL FEATURE";
            helpBox.messageType = HelpBoxMessageType.Warning;
            root.Add(helpBox);

            VisualElement listHeader = UIBuilderResources.ListHeaderVT.CloneTree();
            root.Add(listHeader);

            foreach (DictionaryEntry item in ResolvedValue)
            {
                VisualElement element = OnMake();

                // Bind
                Label label = new Label($"Key: {item.Key}, Value: {item.Value}");
                element.Add(label);

                listHeader.Add(element);
            }

            return root;
        }

        public static VisualElement OnMake()
        {
            return UIBuilderResources.ListElementVT.CloneTree();
        }

        private void OnBind(VisualElement e, int i)
        {

        }
    }
}