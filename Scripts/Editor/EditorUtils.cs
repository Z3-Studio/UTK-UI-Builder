using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Z3.UIBuilder.Editor
{
    public static class EditorUtils
    {
        public static T LoadOrCreateAsset<T>(string path) where T : ScriptableObject
        {
            path = $"{path}.asset";
            T obj = AssetDatabase.LoadAssetAtPath<T>(path);

            if (obj == null)
            {
                obj = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(obj, path);
            }

            return obj;
        }

        public static ScriptableObject CreateAssetInProject(Type type)
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Asset of type " + type.ToString(), type.Name + ".asset", "asset", "");

            if (string.IsNullOrEmpty(path))
                return null;

            ScriptableObject data = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            return data;
        }

        public static void DestroyAsset(Object obj)
        {
            if (AssetDatabase.IsSubAsset(obj))
            {
                AssetDatabase.RemoveObjectFromAsset(obj);
                return;
            }

            string path = AssetDatabase.GetAssetPath(obj);
            AssetDatabase.DeleteAsset(path);
        }

        public static IEnumerable<T> GetAllSubAssets<T>(Object target) where T : Object
        {
            // Get path and sub items
            string path = AssetDatabase.GetAssetPath(target);
            List<Object> list = AssetDatabase.LoadAllAssetsAtPath(path).ToList();

            // Remove target
            list.Remove(target);

            // Return children
            return list.Select(o => o as T);
        }
    }
}