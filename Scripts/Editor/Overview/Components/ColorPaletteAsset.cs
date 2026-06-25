using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Z3.Utils.Editor;

namespace Z3.UIBuilder.Editor
{
    public class ColorPaletteAsset : ScriptableObject
    {
        [SerializeField] private List<Color> colors = new();

        public List<Color> Colors => colors;
        private static ColorPaletteAsset Instance { get; set; }

        [InitializeOnLoadMethod]
        public static void Initializer()
        {
            string path = $"Assets/Editor/Z3/{nameof(ColorPaletteAsset)}.asset";
            ColorPaletteAsset runtimeInstance = EditorUtils.LoadOrCreateAsset<ColorPaletteAsset>(path);
            runtimeInstance.Init();
        }

        public void Init()
        {
            if (Instance != null)
                return;

            Instance = this;
        }

        public static List<Color> GetColors()
        {
            return Instance.Colors;
        }

        public static void AddColor(Color color)
        {
            // Check if already exists
            bool alreadyExists = Instance.colors.Any(c => c == color);
            if (alreadyExists)
                return;

            Instance.colors.Insert(0, color);
            SetAsDirty();
        }

        public static void RemoveColor(Color color)
        {
            Instance.colors.Remove(color);
            SetAsDirty();
        }

        public static void SetAsFirstElement(Color color)
        {
            Instance.colors.Remove(color);
            Instance.colors.Insert(0, color);
            SetAsDirty();
        }

        private static void SetAsDirty()
        {
            EditorUtility.SetDirty(Instance);
        }
    }
}
