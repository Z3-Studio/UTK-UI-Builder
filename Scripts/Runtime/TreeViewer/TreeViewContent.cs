using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Z3.UIBuilder.Core;
using Z3.Utils;

namespace Z3.UIBuilder.TreeViewer
{
    public class TreeViewContent<T>
    {
        public string Title { get; }
        public T TargetObject { get; private set; }
        public Texture2D ButtonIcon { get; private set; }
        public List<TreeViewContent<T>> Children { get; } = new List<TreeViewContent<T>>();

        public TreeViewContent(string title) => Title = title;

        internal void SetAsset(T asset)
        {
            TargetObject = asset;

            Type type = asset.GetType();
            CustomIcon attribute = type.GetCustomAttribute<CustomIcon>();

            if (attribute == null)
                return;

            ButtonIcon = type.GetMethod(attribute.Method, ReflectionUtils.PublicAndPrivate).Invoke(asset, null) as Texture2D;
        }

        public void SetIcon(Texture2D icon) // Editor
        {
            ButtonIcon = icon;
        }
    }
}