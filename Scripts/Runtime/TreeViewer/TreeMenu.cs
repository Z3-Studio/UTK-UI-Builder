using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.TreeViewer
{
    /// <summary> Data structure based on paths </summary>
    [Obsolete("TODO: Rename it to Tree Segment or something like")]
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

    [Obsolete]
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

            ButtonIcon = type.GetMethod(attribute.Method, ReflectionUtils.InstanceAccess).Invoke(asset, null) as Texture2D;
        }

        public void SetIcon(Texture2D icon) // Editor
        {
            ButtonIcon = icon;
        }
    }

    [Obsolete]
    public class TreeViewController<TContent> : TreeViewController<TContent, Label>
    {
        public TreeViewController(TreeMenu<TContent> tree, TreeView treeView, INotifyValueChanged<string> searchField) : base(tree, treeView, searchField, OnMake) { }
        public static Label OnMake()
        {
            Label label = new();
            label.style.paddingTop = 4f;
            return label;
        }
    }

    [Obsolete]
    public class TreeViewController<TContent, TViewElement> where TViewElement : VisualElement
    {
        /// <summary> This will be called whenever drawing is needed. That is, in construction and how much to expand. </summary>
        public event Action<TViewElement, TreeViewContent<TContent>> OnBindItem;

        public event Action<TreeViewContent<TContent>> OnChangeSelection;

        public TreeViewContent<TContent> SelectedContent { get; private set; }

        private TreeMenu<TContent> Tree { get; }
        private Func<TViewElement> makeItem;
        private TreeView treeView;


        public TreeViewController(TreeMenu<TContent> tree, TreeView treeView, INotifyValueChanged<string> searchField, Func<TViewElement> onCreate) : this(tree, treeView, onCreate)
        {
            searchField.RegisterValueChangedCallback(OnSearch);
        }

        public TreeViewController(TreeMenu<TContent> tree, TreeView treeView, Func<TViewElement> onCreate) // TODO: create a component that inherit from TreeView and implement a search field
        {
            this.treeView = treeView;

            Tree = tree;

            makeItem = onCreate;

            // Populate the data structure
            GenerateTreeView();

            if (Tree.Root.Count > 0)
            {
                SelectedContent = Tree.Root[0];
            }
        }

        // TODO: Find better solution and review ListView component
        public List<VisualElement> GetRootElements()
        {
            int count = treeView.viewController.GetItemsCount();
            List<VisualElement> items = new(count);
            for (int i = 0; i < count; i++)
            {
                VisualElement e = treeView.GetRootElementForIndex(i);
                items.Add(e);
            }

            return items;
        }

        public List<TViewElement> GetElements()
        {
            int count = treeView.viewController.GetItemsCount();
            List<TViewElement> items = new(count);
            for (int i = 0; i < count; i++)
            {
                items.Add(GetElement(i));
            }

            return items;
        }

        public TViewElement GetElement(int index) => treeView.GetRootElementForIndex(index).Q<TViewElement>();

        private void OnSearch(ChangeEvent<string> evt)
        {
            // TODO: Hide TreeView and display a list to avoid toogle bug
            foreach (VisualElement element in GetRootElements())
            {
                bool visible = element.name.SearchMatch(evt.newValue);
                element.style.SetDisplay(visible);
            }
        }

        private void GenerateTreeView()
        {
            List<TreeViewItemData<TreeViewContent<TContent>>> items = new List<TreeViewItemData<TreeViewContent<TContent>>>();
            int id = 0;
            AddRecursive(Tree.Root, items, ref id);

            treeView.SetRootItems(items);
            treeView.makeItem = OnMake;
            treeView.bindItem = BindItem;
            treeView.selectionType = SelectionType.Single;
            treeView.Rebuild();

            treeView.selectionChanged += Select;

            // Callback invoked when the user double clicks an item
            //treeView.itemsChosen += Select;
        }

        private VisualElement OnMake()
        {
            VisualElement container = new VisualElement();
            container.name = "Z3 Container";
            container.style.flexDirection = FlexDirection.Row;

            VisualElement icon = new VisualElement();
            icon.name = "Z3 Icon";
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            container.Add(icon);

            TViewElement item = makeItem();
            item.name = "Z3 Make";
            container.Add(item);

            return container;
        }

        /// <summary>
        /// The "makeItem" function will be called as needed
        /// when the TreeView needs more items to render
        /// </summary>
        private void BindItem(VisualElement e, int i)
        {
            TreeViewContent<TContent> item = treeView.GetItemDataForIndex<TreeViewContent<TContent>>(i);

            VisualElement element = treeView.GetRootElementForIndex(i);
            element.name = item.Title;

            VisualElement icon = element.Q("Z3 Icon");
            if (item.ButtonIcon != null)
            {
                icon.style.backgroundImage = new StyleBackground(item.ButtonIcon);
                icon.style.SetPadding(10);
                icon.style.marginRight = 4;
            }
            else
            {
                icon.style.backgroundImage = null;
                icon.style.SetPadding(0);
                icon.style.marginRight = 0;
            }

            TViewElement tViewElement = e.Q<TViewElement>("Z3 Make");
            OnBindItem(tViewElement, item);
        }

        private void AddRecursive(List<TreeViewContent<TContent>> content, List<TreeViewItemData<TreeViewContent<TContent>>> items, ref int id)
        {
            for (int i = 0; i < content.Count; i++)
            {
                List<TreeViewItemData<TreeViewContent<TContent>>> treeViewSubItemsData = new List<TreeViewItemData<TreeViewContent<TContent>>>();
                AddRecursive(content[i].Children, treeViewSubItemsData, ref id);

                TreeViewItemData<TreeViewContent<TContent>> treeViewItemData = new TreeViewItemData<TreeViewContent<TContent>>(id, content[i], treeViewSubItemsData);
                items.Add(treeViewItemData);
                id++;
            }
        }

        private void Select(IEnumerable<object> obj)
        {
            TreeViewContent<TContent> content = obj.ToList()[0] as TreeViewContent<TContent>;
            SelectedContent = content;
            OnChangeSelection?.Invoke(content);
        }
    }
}