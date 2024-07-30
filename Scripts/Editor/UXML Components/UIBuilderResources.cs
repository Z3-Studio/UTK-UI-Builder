using UnityEngine;
using UnityEngine.UIElements;
using Z3.Utils;

namespace Z3.UIBuilder.Editor
{
    public class UIBuilderResources : ScriptableObject
    {
        [SerializeField] private VisualTreeAsset listHeaderVT;
        [SerializeField] private VisualTreeAsset listElementVT;
        [SerializeField] private VisualTreeAsset objectMenuWindowVT;

        public static VisualTreeAsset ListHeaderVT => Instance.listHeaderVT;
        public static VisualTreeAsset ListElementVT => Instance.listElementVT;
        public static VisualTreeAsset ObjectMenuWindow => Instance.objectMenuWindowVT;

        private static UIBuilderResources _Instance { get; set; }
        private static UIBuilderResources Instance
        {
            get
            {
                if (!_Instance)
                {
                    string path = $"Packages/{Z3Path.PackageCompanyName}.ui-builder/{nameof(UIBuilderResources)}.asset";
                    _Instance = UnityEditor.AssetDatabase.LoadAssetAtPath<UIBuilderResources>(path);
                    //_Instance = Z3Path.LoadAssetPath<UIBuilderResources>();
                }

                return _Instance;

            }
        }
    }
}