using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;

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
        public Action<VisualElement, int> onBind; //  IBindElement<T>
        public float fixedItemHeight = 22;

        // TODO: Unbind and destroy

        public Z3ListViewConfig() { }

        public Z3ListViewConfig(string title)
        {
            listName = title;
        }

        public static VisualElement OnMake()
        {
            return UIBuilderResources.ListElementVT.CloneTree();
        }

        public static Z3ListViewConfig SimpleTemplate<TView>() where TView : VisualElement => new Z3ListViewConfig()
        {
            showAddBtn = false,
            showRemoveButton = false,
            showReordable = false,
            showFoldout = false,
            onMakeItem = () => Activator.CreateInstance<TView>(),
        };
    }

    public class ListViewBuilder : ListViewBuilder<object, VisualElement> // Simplified
    {
        public ListViewBuilder(IList source, Z3ListViewConfig config) : base(source, config)
        {
        }
    }

    /// <summary>
    /// Copy of <see cref="Z3ListViewOld<>"/>
    /// </summary>
    public class ListViewBuilder<TItem, TView> : VisualElement where TView : VisualElement
    {
        [UIElement] private Button addButton;

        public event Action<TItem> OnSelectChange;
        public event Action<TItem> OnDelete;
        public event Action OnBuildList;

        public TItem Selection { get; private set; }

        private IList Source => listView.itemsSource; // IList<TItem>

        private ListView listView;
        private Z3ListViewConfig config;

        private Action createAction;

        public ListViewBuilder(IList<TItem> source) : this((IList)source, Z3ListViewConfig.SimpleTemplate<TView>())
        {

        }

        public ListViewBuilder(IList<TItem> source, Z3ListViewConfig config) : this((IList)source, config) 
        { 
        
        }

        protected ListViewBuilder(IList source, Z3ListViewConfig config)
        {
            this.config = config;

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
            OnSelectChange?.Invoke(item);
        }

        private void SetSelection(int index)
        {
            TItem item = (TItem)listView.itemsSource[index];
            listView.SetSelection(index);

            Selection = item;
            OnSelectChange?.Invoke(item);
        }

        public TView GetElement(int index) => (TView)listView.GetRootElementForIndex(index);

        private void BuildListView()
        {
            createAction();

            Action addEvent = config.addEvent != null ? config.addEvent : OnAddNewElement;
            Action<VisualElement, int> onBind = config.onBind != null ? config.onBind : OnBindItem;

            // Essencial
            listView.selectionChanged += OnSelectionChanged;
            listView.makeItem = config.onMakeItem;
            listView.bindItem = onBind;
            listView.itemIndexChanged += (o, n) => Rebuild(true); // Review: Bug when reording
            listView.reorderable = config.showReordable;
            listView.fixedItemHeight = config.fixedItemHeight;

            // List Style
            listView.selectionType = config.selectable ? SelectionType.Single : SelectionType.None;
            listView.reorderMode = ListViewReorderMode.Animated;
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;

            listView.showBorder = true;
            listView.showFoldoutHeader = config.showFoldout;
            listView.showBoundCollectionSize = false; // Only with Foldout
            listView.horizontalScrollingEnabled = false;
            listView.showAddRemoveFooter = false;

            // Bind addButton
            VisualElement listHeader = UIBuilderResources.ListHeaderVT.CloneTree();
            listHeader.BindUIElements(this);

            // Update name and Header position
            if (config.showFoldout)
            {
                listView.headerTitle = config.listName;
                Toggle target = listView.Q<Toggle>();
                target.hierarchy.Add(listHeader);

                target.tooltip = config.tooltip;
            }
            else
            {
                // REVIEW IT
                VisualElement row = new();
                row.style.flexDirection = FlexDirection.Row;
                row.style.justifyContent = Justify.SpaceBetween;

                Label listName = new Label(config.listName);
                row.Add(listName);
                row.Add(listHeader);

                listView.hierarchy.Insert(0, row);
            }

            addButton.clickable = new Clickable(addEvent);
            addButton.visible = config.showAddBtn;

            Add(listView);

            if (config.selectable && Source.Count > 0)
            {
                listView.SetSelection(0);
            }

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
            if (e is IBindElement<TItem> bindableElement)
            {
                bindableElement.Bind((TItem)Source[i], i);
            }

            // TODO: Try to create your own Label with custom text when serializedObject change any field / OnValidate
            // I need to receive a event from each list element
            Label label = e.Q<Label>("element-name");             

            if (label != null) // Only with base template
            {
                label.RegisterUpdate(UpdateLabel);

                void UpdateLabel() // TODO: Improve performance
                {
                    label.text = ToStringElement(Source[i]);
                }
            }

            Button removeButton = e.Q<Button>("remove-button");

            if (removeButton != null) // Only with base template
            {
                if (config.showRemoveButton)
                {
                    removeButton.clickable = new(() =>
                    {
                        TItem element = (TItem)Source[i];

                        Source.Remove(element);
                        OnDelete?.Invoke(element);

                        /*if (element.Equals(Selection))
                        {
                            if (Source.Count == 0)
                            {
                                SetSelection(-1);
                            }
                            else if (i >= Source.Count)
                            {
                                SetSelection(i - 1);
                            }
                            else
                            {
                                SetSelection(i);
                            }
                        }*/

                        Rebuild(true);
                    });
                }
                else
                {
                    removeButton.visible = false; 
                }
            }
        }

        private void OnAddNewElement()
        {
            Source.Add(default);
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

        private string ToStringElement(object element)
        {
            if (element == null)
                return "Null";

            string toString;

            if (string.IsNullOrEmpty(config.toStringExpression))
            {
                toString = element.ToString();
            }
            else
            {
                toString = ExecuteToStringExpression(element, config.toStringExpression);
            }

            if (!config.toStringWithPrefix)
                return toString;

            string prefix = element.GetTypeNiceString();
            return prefix + " " + toString.AddRichTextColor(EditorStyle.DarkLabel);
        }

        // TODO: Move to static
        private string ExecuteToStringExpression(object obj, string toStringExpression)
        {
            ParameterExpression param = Expression.Parameter(obj.GetType(), "x");
            LambdaExpression lambda = Expression.Lambda(Expression.PropertyOrField(param, toStringExpression), param);
            Delegate func = lambda.Compile();
            return func.DynamicInvoke(obj) as string;
        }
    }
}