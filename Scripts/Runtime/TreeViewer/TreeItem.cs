using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Core
{
    public interface IBindElement<T>
    {
        void Bind(T item, int index);
    }

    public class TreeViewConfig<TContent, TViewElement> where TViewElement : VisualElement
    {
        public TreeContainer<TContent> Tree { get; }
        public int FixedItemHeight { get; set; } = 22; // BaseVerticalCollectionView.s_DefaultItemHeight

        public Func<TViewElement> onMake;
        public Action<TViewElement, TreeItem<TContent>, int> onBind;
        public INotifyValueChanged<string> searchField;

        public TreeViewConfig()
        {
            onMake = MakeItem;
            onBind = BindItem; 
        }

        private TViewElement MakeItem()
        {
            TViewElement item = Activator.CreateInstance<TViewElement>();
            item.name = "Z3 Make";
            return item;
        }

        private void BindItem(TViewElement viewElement, TreeItem<TContent> element, int index)
        {
            if (viewElement is IBindElement<TreeItem<TContent>> bindableElement)
            {
                bindableElement.Bind(element, index);
            }
        }
    }

    public class TreeViewContainer<TContent, TViewElement> where TViewElement : VisualElement
    {
        // Events
        public event Action<TViewElement, TreeItem<TContent>, int> OnBindItem;

        public event Action<TreeItem<TContent>> OnChangeSelection;

        public event Action<TViewElement> OnMakeItem;

        // State
        public TreeItem<TContent> SelectedContent { get; private set; }

        // Depedencies
        public TreeContainer<TContent> Tree { get; }
        public TreeViewConfig<TContent, TViewElement> TreeConfig { get; } = new();

        private TreeView TreeView { get; } = new();

        public TreeViewContainer(IEnumerable<TContent> source, Func<TContent, IEnumerable<TContent>> getChilds)
        {
            Tree = new TreeContainer<TContent>(source, getChilds);
            TreeConfig = new TreeViewConfig<TContent, TViewElement>();

            GenerateTreeView();
        }

        public TreeViewContainer(IEnumerable<TContent> source, Func<TContent, IEnumerable<TContent>> getChilds, TreeViewConfig<TContent, TViewElement> config)
        {
            Tree = new TreeContainer<TContent>(source, getChilds);
            TreeConfig = config;

            GenerateTreeView();
        }

        public TreeViewContainer(TreeContainer<TContent> tree, TreeViewConfig<TContent, TViewElement> config)
        {
            Tree = tree;
            TreeConfig = config;

            GenerateTreeView();
        }

        // TODO: Find better solution and review ListView component
        public List<VisualElement> GetRootElements()
        {
            int count = TreeView.viewController.GetItemsCount();
            List<VisualElement> items = new(count);
            for (int i = 0; i < count; i++)
            {
                VisualElement e = TreeView.GetRootElementForIndex(i);
                items.Add(e);
            }

            return items;
        }

        public List<TViewElement> GetElements()
        {
            int count = TreeView.viewController.GetItemsCount();
            List<TViewElement> items = new(count);
            for (int i = 0; i < count; i++)
            {
                items.Add(GetElement(i));
            }

            return items;
        }

        public TViewElement GetElement(int index) => TreeView.GetRootElementForIndex(index).Q<TViewElement>();

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
            TreeConfig.searchField?.RegisterValueChangedCallback(OnSearch);

            TreeView.SetRootItems(Tree.CreateTreeViewItemData());
            TreeView.makeItem = OnMake;
            TreeView.bindItem = BindItem;
            TreeView.selectionType = SelectionType.Single;
            TreeView.fixedItemHeight = TreeConfig.FixedItemHeight;
            TreeView.Rebuild();

            TreeView.selectionChanged += Select;

            // Callback invoked when the user double clicks an item
            //treeView.itemsChosen += Select;
        }

        private TViewElement OnMake()
        {
            TViewElement newElement = TreeConfig.onMake();
            OnMakeItem?.Invoke(newElement);
            return newElement;
        }

        /// <summary>
        /// The "makeItem" function will be called as needed
        /// when the TreeView needs more items to render
        /// </summary>
        private void BindItem(VisualElement e, int i)
        {
            TreeItem<TContent> item = TreeView.GetItemDataForIndex<TreeItem<TContent>>(i);
            TViewElement tViewElement = e as TViewElement;

            TreeConfig.onBind(tViewElement, item, i);
            OnBindItem?.Invoke(tViewElement, item, i);
        }

        private void Select(IEnumerable<object> obj)
        {
            TreeItem<TContent> content = obj.FirstOrDefault() as TreeItem<TContent>;
            SelectedContent = content;
            OnChangeSelection?.Invoke(content);
        }

        public static implicit operator TreeView(TreeViewContainer<TContent, TViewElement> treeContainerView) => treeContainerView.TreeView;
    }

    public class TreeContainer<T>
    {
        private readonly IEnumerable<T> source;
        private readonly Func<T, IEnumerable<T>> getChilds;

        public List<TreeItem<T>> Root { get; private set; } = new();

        public TreeContainer(IEnumerable<T> source, Func<T, IEnumerable<T>> getChilds)
        {
            this.source = source;
            this.getChilds = getChilds;

            foreach (T item in source)
            {
                TreeItem<T> treeView = AddRecursive(item, default, getChilds);
                Root.Add(treeView);
            }
        }

        private static TreeItem<T> AddRecursive(T content, TreeItem<T> parent, Func<T, IEnumerable<T>> getChilds)
        {
            TreeItem<T> treeView = new TreeItem<T>(content, parent);

            foreach (T contentChild in getChilds(content))
            {
                TreeItem<T> child = AddRecursive(contentChild, treeView, getChilds);
                treeView.Children.Add(child);
            }

            return treeView;
        }

        public List<TreeViewItemData<TreeItem<T>>> CreateTreeViewItemData()
        {
            int id = 0;
            List<TreeViewItemData<TreeItem<T>>> root = new List<TreeViewItemData<TreeItem<T>>>();
            AddRecursive(Root, root, ref id);
            return root;
        }

        private void AddRecursive(List<TreeItem<T>> content, List<TreeViewItemData<TreeItem<T>>> items, ref int id)
        {
            for (int i = 0; i < content.Count; i++)
            {
                List<TreeViewItemData<TreeItem<T>>> treeViewSubItemsData = new List<TreeViewItemData<TreeItem<T>>>();
                AddRecursive(content[i].Children, treeViewSubItemsData, ref id);

                TreeViewItemData<TreeItem<T>> treeViewItemData = new TreeViewItemData<TreeItem<T>>(id, content[i], treeViewSubItemsData);
                items.Add(treeViewItemData);
                id++;
            }
        }
    }

    public class TreeItem<T>
    {
        public T Content { get; }
        public TreeItem<T> Parent { get; }
        public List<TreeItem<T>> Children { get; } = new List<TreeItem<T>>();
        public bool Root => Parent != null;

        public TreeItem(T content, TreeItem<T> parent)
        {
            Content = content;
            Parent = parent;
        }

        public TreeItem<T> GetRoot()
        {
            TreeItem<T> item = this;
            while (item.Parent != null)
            {
                item = item.Parent;
            }

            return item;
        }

        public void Rebuild(Func<T, IEnumerable<T>> getChilds)
        {
            Children.Clear();

            IEnumerable<T> newChildren = getChilds(Content);
            Dictionary<T, TreeItem<T>> currentChildren = Children.ToDictionary(c => c.Content, c => c);

            // Remove children not in the new list
            foreach (T child in currentChildren.Keys.Except(newChildren))
            {
                Children.Remove(currentChildren[child]);
            }

            // Add new children that were not in the current list
            foreach (T child in newChildren.Except(currentChildren.Keys))
            {
                TreeItem<T> treeItem = new TreeItem<T>(child, this);
                Children.Add(treeItem);
            }

            Children.ForEach(c => c.Rebuild(getChilds));
        }

        // TODO: Create extension method
        public static TreeItem<T> FindMatchingItem(List<TreeItem<T>> list, T content)
        {
            foreach (TreeItem<T> treeItem in list)
            {
                TreeItem<T> result = treeItem.FindMatchingItem(content);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public TreeItem<T> FindMatchingItem(T content)
        {
            if (EqualityComparer<T>.Default.Equals(Content, content))
                return this;

            foreach (TreeItem<T> children in Children)
            {
                TreeItem<T> result = children.FindMatchingItem(content);

                if (result != null)
                    return result;
            }

            return null;
        }
    }
}