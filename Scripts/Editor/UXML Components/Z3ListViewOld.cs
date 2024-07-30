using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    using Object = UnityEngine.Object;

    public class Z3ListViewOld<T> : VisualElement
    {
        [UIElement] private Button addButton;

        public event Action<T> OnSelectChange;
        public event Action<T> OnDelete;
        private IList<T> Source => listView.itemsSource as IList<T>;

        private Action createAction;

        private ListView listView;
        private string fieldInfo;

        public Z3ListViewOld(Object obj, IList<T> source, string fieldName = null)
        {
            SerializedObject serializedObject = new SerializedObject(obj);
            fieldInfo = fieldName;

            //Debug.Log(serializedObject.FindProperty(fieldName).propertyPath);
            //Debug.Log(GetBindPath(0));

            createAction = () =>
            {
                listView = new ListView();
                listView.itemsSource = source as IList;
            };

            BuildListView();
        }

        public Z3ListViewOld(IList<T> source, string fieldName = null)
        {
            if (fieldName == null)
            {
                fieldInfo = CollectionUtils.GetCollectionTypeName(source);
            }
            else
            {
                fieldInfo = fieldName.GetNiceString();
            }

            createAction = () =>
            {
                listView = new ListView();
                listView.itemsSource = source as IList;
            };

            BuildListView();
        }

        private void BuildListView()
        {
            // TODO: Send event when remove multiple objects, or change the list size
            createAction();

            // Essencial
            listView.selectionChanged += OnSelectionChanged;
            listView.makeItem = OnMakeItem;
            listView.bindItem = OnBindItem;

            // List Style
            listView.selectionType = SelectionType.Multiple;
            listView.reorderMode = ListViewReorderMode.Animated;
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;

            listView.reorderable = true;
            listView.showBorder = true;
            listView.showFoldoutHeader = true;
            listView.showBoundCollectionSize = true; // Only with Foldout
            listView.horizontalScrollingEnabled = false;
            listView.showAddRemoveFooter = false;

            // Bind addButton
            VisualElement listHeader = UIBuilderResources.ListHeaderVT.CloneTree();
            listHeader.BindUIElements(this);

            // Update name and Header position
            if (listView.showFoldoutHeader)
            {
                Toggle target = listView.Q<Toggle>();
                target.text = fieldInfo;

                TextField boundCollectionSize = listView.Q<TextField>("unity-list-view__size-field");
                boundCollectionSize.style.marginRight = 30;
                boundCollectionSize.style.marginLeft = -30;

                target.hierarchy.Add(listHeader);
            }
            else
            {
                listView.hierarchy.Insert(0, listHeader);
            }

            // Override add new element
            SetAddEvent(OnAddNewElement);

            Add(listView);
        }

        private void OnSelectionChanged(IEnumerable<object> a)
        {
            T obj = (T)a.FirstOrDefault();

            if (obj != null)
            {
                OnSelectChange?.Invoke(obj);
            }
        }

        private void OnBindItem(VisualElement e, int i)
        {
            Label label = e.Q<Label>();

            string name = Source[i] == null ? "Null" : Source[i].ToString();
            label.text = $"{i + 1} - {name}";

            e.Q<Button>().clickable = new(() =>
            {
                T element = Source.ElementAt(i);
                Source.Remove(element);
                OnDelete?.Invoke(element);

                Rebuild(true);
            });
        }

        private VisualElement OnMakeItem()
        {
            return UIBuilderResources.ListElementVT.CloneTree();
        }

        private void OnAddNewElement()
        {
            Source.Add(default);
            Rebuild(true);
            //listView.viewController.AddItems(1);
        }

        public void SetAddEvent(Action newAddEvent)
        {
            addButton.clickable = new Clickable(newAddEvent);
        }

        public void Rebuild(bool force = false)
        {
            if (!force)
            {
                listView.Rebuild();
                return;
            }

            //listView.viewController.RemoveItem(0);
            Remove(listView);
            BuildListView();
        }

        /*
        [UIElement] private Button addButton;

        public event Action<T> OnSelectChange;
        public event Action<T> OnDelete;

        private IList<T> Source => listView.itemsSource as IList<T>;
        private static Color DarkColor => new Color(.6f, .6f, .6f);

        private ListView listView;
        private string fieldInfo;


        private Clickable addEvent;

        private Action createAction;

        private bool showAddBtn;
        private bool showRemoveButton;
        private bool showReordable;

        private static UIBuilderResources Resources => UIBuilderResources.Instance;

        SerializedObject serializedObject;
        //public TaskListView(Object obj, IList<T> source, string fieldName = null) { }
        

        public Z3ListView(IList<T> source, string fieldName = null, bool showAddBtn = true, bool editable = true)
        {
            if (fieldName == null)
            {
                fieldInfo = CollectionUtils.GetCollectionTypeName(source);
            }
            else
            {
                fieldInfo = fieldName.GetNiceString();
            }

            this.showAddBtn = showAddBtn;
            this.showRemoveButton = editable;
            this.showReordable = editable;

            //this.source = source;

            createAction = () =>
            {
                listView = new ListView(); // Node bind!!
                listView.itemsSource = source as IList;
            };

            // Default add element
            addEvent = new Clickable(OnAddNewElement);

            BuildListView();
        }

        private void BuildListView()
        {
            // TODO: Send event when remove multiple objects, or change the list size
            createAction();

            // Essencial
            listView.selectionChanged += OnSelectionChanged;
            listView.makeItem = OnMakeItem;
            listView.bindItem = OnBindItem;

            // List Style
            listView.selectionType = SelectionType.Multiple;
            listView.reorderMode = ListViewReorderMode.Animated;
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;

            listView.reorderable = showReordable;
            listView.showBorder = true;
            listView.showFoldoutHeader = true;
            listView.showBoundCollectionSize = false; // Only with Foldout
            listView.horizontalScrollingEnabled = false;
            listView.showAddRemoveFooter = false;

            // Bind addButton
            VisualElement listHeader = Resources.ListHeaderVT.CloneTree();
            listHeader.BindUIElements(this);

            // Update name and Header position
            if (listView.showFoldoutHeader)
            {
                Toggle target = listView.Q<Toggle>();
                target.text = fieldInfo;

                target.hierarchy.Add(listHeader);
            }
            else
            {
                listView.hierarchy.Insert(0, listHeader);
            }

            addButton.clickable = addEvent;
            addButton.visible = showAddBtn;

            Add(listView);

            listView.SetSelection(0);
        }

        private void OnSelectionChanged(IEnumerable<object> a)
        {
            T obj = (T)a.FirstOrDefault();

            if (obj != null)
            {
                OnSelectChange?.Invoke(obj);
            }
        }

        private void OnBindItem(VisualElement e, int i)
        {
            Label label = e.Q<Label>(); // TODO: Try to create your own Label with custom text when serializedObject change any field / OnValidate

            //SerializedObject serializedObject = new SerializedObject(source[i]);
            

            if (label != null)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.richText = true;

                IMGUIContainer imguiContainer = new IMGUIContainer();
                imguiContainer.onGUIHandler += () =>
                {
                    string name = ToStringElement(Source[i]);
                    EditorGUILayout.LabelField(name, style);
                };

                int d = label.parent.IndexOf(label);
                label.parent.Insert(d, imguiContainer);
                label.RemoveFromHierarchy();
            }

            // I need to receive a event from each list element

            //string name = ToStringElement(source[i]);
            //string name = ToStringElement(listView.viewController.GetItemForIndex(i));

            //label.text = $"{i + 1} - {name}";
            Button removeButton = e.Q<Button>();

            if (showRemoveButton)
            {
                removeButton.clickable = new(() =>
                {
                    T element = Source.ElementAt(i);
                    Source.Remove(element);
                    OnDelete?.Invoke(element);

                    Rebuild(true);
                });
            }
            else
            {
                removeButton.visible = false;
            }
        }

        private VisualElement OnMakeItem()
        {
            return Resources.ListElementVT.CloneTree();
        }

        private void OnAddNewElement()
        {
            Source.Add(default);
            Rebuild(true);
            //listView.viewController.AddItems(1);
        }

        // Override add new element
        public void SetAddEvent(Action newAddEvent)
        {
            addEvent = new Clickable(newAddEvent);
            addButton.clickable = addEvent;
        }

        public void Rebuild(bool force = false)
        {
            if (!force)
            {
                listView.Rebuild();
                return;
            }

            //listView.viewController.RemoveItem(0);
            Remove(listView);
            BuildListView();
        }

        // Scriptable Objects with Guid. Very especific 

        public string GetBindPath(int index) => $"{fieldInfo}.Array.data[{index}]";

        private string ToStringElement(object element)
        {
            if (element == null)
                return "Null";

            // I Could use a Interface to do this
            (string, string) a = GetValueWithBracket(element.ToString());

            string name = element.GetType().Name.GetNiceString();

            // NodeName Info
            string info = string.Empty;
            try
            {
                return name + " " + ExecuteToStringExpression(element, "Info").AddRichTextColor(DarkColor);

            }
            catch (Exception)
            {

            }
            return element.ToString();

            string name2 = a.Item1.GetNiceString();
            string guid = a.Item2.AddRichTextColor(DarkColor);
            // NodeName [GUID]
            return name + " " + guid;
        }


        public static (string, string) GetValueWithBracket(string input)
        {
            string inputSplitted = input.Split("/").Last();
            int startIndex = inputSplitted.IndexOf("[");


            if (startIndex > 0)
            {
                string beforeBracket = inputSplitted.Substring(0, startIndex);
                string bracket = inputSplitted.Substring(startIndex);

                return (beforeBracket, bracket);
            }

            // No Guid Item
            return (inputSplitted, string.Empty);
        }

        // TODO: Move to static
        private string ExecuteToStringExpression(object obj, string toStringExpression)
        {
            ParameterExpression param = Expression.Parameter(obj.GetType(), "x");
            LambdaExpression lambda = Expression.Lambda(Expression.PropertyOrField(param, toStringExpression), param);
            Delegate func = lambda.Compile();
            return func.DynamicInvoke(obj) as string;
        }

        //static object ExecuteToStringExpression(object obj, string toStringExpression)
        //{
        //    var param = Expression.Parameter(obj.GetType(), "x");
        //    var expression = new DynamicExpresso.ExpressionParser().Parse(toStringExpression, param);
        //    var lambda = Expression.Lambda(expression, param);
        //    var compiledLambda = lambda.Compile();
        //    return compiledLambda.DynamicInvoke(obj);
        //}
         * */
    }
}