using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils;

namespace Z3.UIBuilder.Editor
{
    public class Z3ListViewConfig
    {
        public string tooltip;
        public string listName;
        public bool showAddBtn = true;
        public bool showRemoveButton = true;
        public bool showReordable = true;
        public bool showFoldout = true;
        public bool selectable = true;

        public bool toStringWithPrefix = true;
        public string toStringExpression;

        public Func<VisualElement> onMakeItem = OnMake; //  () => .Activator.CreateInstance<T>()
        public Action addEvent;
        public float fixedItemHeight = 22;

        // TODO: Option to create Copy and Paste, and include callbacks.
        // TODO: Unbind and destroy

        public Z3ListViewConfig() { }

        public Z3ListViewConfig(string title)
        {
            listName = title;
        }

        public static VisualElement OnMake() => new LabelView();

        public static Z3ListViewConfig SimpleTemplate<TView>() where TView : VisualElement => new Z3ListViewConfig()
        {
            showAddBtn = false,
            showRemoveButton = false,
            showReordable = false,
            showFoldout = false,
            onMakeItem = () => Activator.CreateInstance<TView>(),
        };

        public static Z3ListViewConfig DefaultTemplate<TView>() where TView : VisualElement => new Z3ListViewConfig()
        {
            showAddBtn = true,
            showRemoveButton = true,
            showReordable = true,
            showFoldout = true,
            onMakeItem = () => Activator.CreateInstance<TView>(),
        };
    }

    public interface IListView
    {
        Z3ListViewConfig Config { get; }
        void DeleteElement(object element, int index);
    }

    public class ListViewBuilder<TItem> : ListViewBuilder<TItem, LabelView> // Simplified
    {
        public ListViewBuilder(IList<TItem> source, Z3ListViewConfig config) : base((IList)source, config)
        {
        }

        public ListViewBuilder(IList<TItem> source) : this(source, Z3ListViewConfig.DefaultTemplate<LabelView>())
        {
        }
    }

    /// <summary>
    /// Copy of <see cref="Z3ListViewOld<>"/>
    /// </summary>
    public class ListViewBuilder<TItem, TView> : VisualElement, IListView where TView : VisualElement
    {
        [UIElement] private Button addButton;

        public event Action<TItem> OnSelectChange;
        public event Action<TItem> OnDelete;
        public event Action OnBuildList; // TODO: Review it

        public Action<TView, TItem, int> onBind; // TODO: Remove it

        public TItem Selection { get; private set; }
        public int SelectionIndex { get; private set; }

        private IList Source => listView.itemsSource; // IList<TItem>

        public Z3ListViewConfig Config { get; }

        private ListView listView;

        private Action createAction;
        public VisualElement Header { get; private set; }

        public ListViewBuilder(IList<TItem> source) : this((IList)source, Z3ListViewConfig.SimpleTemplate<TView>())
        {

        }

        //public ListViewBuilder(IList<TItem> source, Z3ListViewConfig config) : this((IList)source, config) 
        //{ 
        
        //}

        public ListViewBuilder(IList source, Z3ListViewConfig config)
        {
            Config = config;

            if (config.listName == null)
            {
                config.listName = CollectionUtils.GetCollectionTypeName(source);
            }

            // Review: Bug when rebuilding the list. If can bind I could avoid this way
            createAction = () => listView = new ListView(source);

            // Default add element
            BuildListView();
        }

        // TODO: Find better solution and review TreeView component
        public List<TView> GetElements()
        {
            return listView.Query<TView>().ToList();

            List<TView> items = new(Source.Count);
            for (int i = 0; i < Source.Count; i++)
            {
                items.Add((TView)listView.GetRootElementForIndex(i));
            }

            return items;
        }

        public void SetSelection(TItem item)
        {
            int index = listView.itemsSource.IndexOf(item);
            listView.SetSelection(index);

            Selection = item; 
            SelectionIndex = index;
            OnSelectChange?.Invoke(item);
        }

        private void SetSelection(int? index)
        {
            if (!index.HasValue)
            {
                Selection = default;
                SelectionIndex = -1;
                listView.ClearSelection();
                OnSelectChange?.Invoke(default);
                return;
            }

            int i = index.Value;
            TItem item = (TItem)listView.itemsSource[i];
            listView.SetSelection(i);

            Selection = item;
            SelectionIndex = i;
            OnSelectChange?.Invoke(item);
        }

        public TView GetElement(int index) => (TView)listView.GetRootElementForIndex(index);

        private void BuildListView()
        {
            createAction();

            Action addEvent = Config.addEvent != null ? Config.addEvent : OnAddNewElement;

            // Essencial
            listView.style.marginTop = 8;
            listView.selectionChanged += OnSelectionChanged;
            listView.makeItem = Config.onMakeItem;
            listView.bindItem = OnBindItem;
            listView.itemIndexChanged += (o, n) => Rebuild(true); // Review: Bug when reording
            listView.reorderable = Config.showReordable;
            listView.fixedItemHeight = Config.fixedItemHeight;

            // List Style
            listView.selectionType = Config.selectable ? SelectionType.Single : SelectionType.None;
            listView.reorderMode = ListViewReorderMode.Animated;
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;

            listView.showBorder = true;
            listView.showFoldoutHeader = Config.showFoldout;
            listView.showBoundCollectionSize = false; // Only with Foldout
            listView.horizontalScrollingEnabled = false;
            listView.showAddRemoveFooter = false;

            // Bind addButton
            VisualElement listHeader = UIBuilderResources.ListHeaderVT.CloneTree();
            listHeader.BindUIElements(this);

            // Update name and Header position
            if (Config.showFoldout)
            {
                listView.headerTitle = Config.listName;
                Toggle target = listView.Q<Toggle>();
                target.hierarchy.Add(listHeader);

                target.tooltip = Config.tooltip;

                Header = target;
            }
            else
            {
                // Note: Setting listView.headerTitle doesn't create a label
                VisualElement row = new();
                row.style.flexDirection = FlexDirection.Row;
                row.style.justifyContent = Justify.SpaceBetween;

                Label listName = new Label(Config.listName);
                row.Add(listName);
                row.Add(listHeader);

                listView.hierarchy.Insert(0, row);
                Header = row;
            }

            addButton.clickable = new Clickable(addEvent);
            addButton.visible = Config.showAddBtn;

            Add(listView);

            if (Config.selectable && Source.Count > 0)
            {
                listView.SetSelection(0);
            }
            Label label = listView.Q<Label>();
            label.style.marginTop = 3;

            // TODO: Find a better solution
            listView.schedule.Execute((e) => OnBuildList?.Invoke()).StartingIn(100);
        }

        private void OnSelectionChanged(IEnumerable<object> a)
        {
            TItem obj = (TItem)a.FirstOrDefault();

            if (obj != null)
            {
                SetSelection(obj);
            }
        }

        private void OnBindItem(VisualElement e, int i)
        {
            TItem element = (TItem)Source[i];

            if (e is IBindElement<TItem> bindableElement)
            {
                bindableElement.Bind(element, i);
            }
            else if (e is LabelView listElement)
            {
                listElement.Bind(this, element, i);
            }

            TView view = (TView)e;
            onBind?.Invoke(view, element, i);
        }


        void IListView.DeleteElement(object element, int index) => DeleteElement((TItem)element, index);

        public void DeleteElement(TItem element, int index) // Maybe option to remove using index?
        {
            Source.RemoveAt(index);
            OnDelete?.Invoke(element);

            if (SelectionIndex == index)
            {
                if (Source.Count == 0)
                {
                    SetSelection(null);
                }
                else if (index >= Source.Count)
                {
                    SetSelection(index - 1);
                }
                else
                {
                    SetSelection(index);
                }
            }

            Rebuild(true);
        }

        private void OnAddNewElement()
        {
            TItem value;
            try
            {
                // TODO: Review it
                value = Activator.CreateInstance<TItem>();
            }
            catch (Exception)
            {
                value = default;
            }

            Source.Add(value);
            Rebuild(true);
            //listView.viewController.AddItems(1);
        }

        public void Rebuild(bool force = false)
        {
            if (!force)
            {
                listView.Rebuild();
                return;
            }

            Clear();
            BuildListView();
        }
    }
}