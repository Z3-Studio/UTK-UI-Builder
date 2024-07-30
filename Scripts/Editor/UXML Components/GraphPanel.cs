using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public abstract class GraphPanel : GraphView
    {
        //public new class UxmlFactory :  UxmlFactory<CustomGraph, UxmlTraits> { }

        //protected virtual string StyleSheetPath => "Assets/UI Toolkit/GraphStyleSS.uss";

        private const float MiniMapSize = 100f;

        public GraphPanel()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());  
            this.AddManipulator(new ContentDragger());  
            this.AddManipulator(new SelectionDragger());  
            this.AddManipulator(new RectangleSelector());

            MiniMap miniMap = new()
            {
                anchored = true,
            };

            miniMap.Q<Label>().style.color = Color.white;
            miniMap.style.backgroundColor = new Color(0, 0, 0, 0.4f);
            miniMap.style.SetBorderColor(new Color(0.5f, 0.5f, 0.5f));

            miniMap.style.maxHeight = MiniMapSize;
            miniMap.style.maxWidth = MiniMapSize;
            miniMap.style.minHeight = MiniMapSize;
            miniMap.style.minWidth = MiniMapSize;

            Add(miniMap);

            //StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(StyleSheetPath);
            //styleSheets.Add(style);

            // Subscribe to the GeometryChanged event
            RegisterCallback<GeometryChangedEvent>((evt) =>
            {
                // Calculate the
                Rect newPosition = new Rect(layout.xMax - MiniMapSize - 10f, layout.yMax - MiniMapSize - 35f, MiniMapSize, MiniMapSize);
                miniMap.SetPosition(newPosition);
            });

        }
    }
}