using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using Object = UnityEngine.Object;
using Z3.UIBuilder.TreeViewer;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public static class TreeMenuExtensions
    {
        public static void AddGameData<T>(this TreeMenu<T> menuTree, string drawPath, T mainData) where T : Object
        {
            menuTree.Add(drawPath, mainData);

            FieldInfo[] fields = mainData.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                if (field.GetValue(mainData) is T asset)
                {
                    string segment = field.Name.GetNiceString();
                    menuTree.Add(drawPath + "/" + segment, asset);
                }
            }
        }

        public static void AddAllAssetsAtPath<T>(this TreeMenu<T> menuTree, string drawPath, string projectPath, Type searchType, bool checkSubFolders = false, IconType iconType = IconType.None) where T : Object
        {
            Texture2D texture = EditorIcons.GetTexture2D(iconType);
            menuTree.AddAllAssetsAtPath(drawPath, projectPath, searchType, texture, checkSubFolders);
        }
        
        public static void AddAllAssetsAtPath<T>(this TreeMenu<T> menuTree, string drawPath, string projectPath, Type searchType, Texture2D iconTexture = null, bool checkSubFolders = false) where T : Object
        {
            // Use Unity's AssetDatabase class to search for all assets of the specified type in the specified project path
            string[] guids = AssetDatabase.FindAssets("t:" + searchType.Name, new[] { projectPath });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // Check if is not a subfolder item
                if (!assetPath.Remove(0, projectPath.Length + 1).Contains("/"))
                {
                    // Add the asset to the menu tree, using the specified drawing path as the tree path
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    string segment = asset.name.GetNiceString();
                    menuTree.Add(drawPath + "/" + segment, asset);
                }
            }

            if (checkSubFolders)
            {
                // Get the child folders of the specified project path
                string[] subFolders = AssetDatabase.GetSubFolders(projectPath);
                foreach (string subFolder in subFolders)
                {
                    menuTree.AddAllAssetsAtPath(drawPath + "/" + Path.GetFileName(subFolder), subFolder, searchType, true, IconType.None);
                }
            }

            TreeViewContent<T> content = menuTree.FindContentAtPath(drawPath);
            if (content != null)
            {
                content.SetIcon(iconTexture);
            }
        }

        private static TreeViewContent<T> FindContentAtPath<T>(this TreeMenu<T> menuTree, string path, List<TreeViewContent<T>> nodes = null) where T : Object
        {
            nodes ??= menuTree.Root;

            string[] pathSegments = path.Split('/');
            if (pathSegments.Length == 0)
            {
                return null;
            }

            TreeViewContent<T> node = nodes.FirstOrDefault(n => n.Title == pathSegments[0]);
            if (node == null)
            {
                return null;
            }

            if (pathSegments.Length == 1)
            {
                return node;
            }
            else
            {
                return menuTree.FindContentAtPath(string.Join("/", pathSegments, 1, pathSegments.Length - 1), node.Children);
            }
        }
    }
}