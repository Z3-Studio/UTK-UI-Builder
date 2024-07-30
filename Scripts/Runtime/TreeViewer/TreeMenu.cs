using System.Collections.Generic;
using System.Linq;

namespace Z3.UIBuilder.TreeViewer
{
    /// <summary> Data structure based on paths </summary>
    public class TreeMenu<T>
    {
        public List<TreeViewContent<T>> Root { get; } = new();

        public void Add(string drawPath, T objectToDraw)
        {
            string[] pathSegments = drawPath.Split('/');
            AddRecursive(Root, pathSegments, 0, objectToDraw);
        }

        protected void AddRecursive(List<TreeViewContent<T>> nodes, string[] pathSegments, int segmentIndex, T objectToDraw)
        {
            if (segmentIndex >= pathSegments.Length)
                return;

            string segment = pathSegments[segmentIndex];
            TreeViewContent<T> node = nodes.FirstOrDefault(n => n.Title == segment);
            if (node == null)
            {
                node = new TreeViewContent<T>(segment);
                nodes.Add(node);
            }

            if (segmentIndex == pathSegments.Length - 1)
            {
                node.SetAsset(objectToDraw);
            }
            else
            {
                AddRecursive(node.Children, pathSegments, segmentIndex + 1, objectToDraw);
            }
        }
    }
}