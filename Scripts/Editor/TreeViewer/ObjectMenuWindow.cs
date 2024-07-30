using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;
using Z3.UIBuilder.TreeViewer;

namespace Z3.UIBuilder.Editor.TreeViewer
{
    public abstract class ObjectMenuWindow : ObjectMenuWindow<object> { }

    public abstract class ObjectMenuWindow<TContent> : EditorWindow, IHasCustomMenu where TContent : class
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;

        [UIElement] private TreeView treeView;
        [UIElement] private VisualElement inspectorContainer;
        [UIElement] private ToolbarSearchField searchField;

        protected TContent SelectedObject => treeViewBuilder.SelectedContent.TargetObject;
        protected virtual VisualTreeAsset VisualTreeAsset => UIBuilderResources.ObjectMenuWindow;

        private TreeViewController<TContent, Label> treeViewBuilder;

        private void CreateGUI()
        {
            // Close style and bind
            VisualTreeAsset.CloneTree(rootVisualElement);
            rootVisualElement.BindUIElements(this);

            // Build Menu Tree and registe event
            TreeMenu<TContent> treeMenu = new TreeMenu<TContent>();
            BuildMenuTree(treeMenu);
            IncludeIcon(treeMenu.Root);

            treeViewBuilder = new TreeViewController<TContent, Label>(treeMenu, treeView, searchField, OnCreateNewElement);
            treeViewBuilder.OnChangeSelection += OnChangeSelection;
            treeViewBuilder.OnBindItem += OnBindItem;

            // Draw default selection
            OnChangeSelection(treeViewBuilder.SelectedContent);
        }

        private void IncludeIcon(List<TreeViewContent<TContent>> items)
        {
            foreach (TreeViewContent<TContent> item in items)
            {
                if (item.TargetObject != null)
                {
                    EditorIcon editorIcon = item.TargetObject.GetType().GetCustomAttribute<EditorIcon>();

                    if (editorIcon != null)
                    {
                        Texture2D icon = EditorIcons.GetTexture2D(editorIcon.Icon);
                        item.SetIcon(icon);
                    }
                }

                IncludeIcon(item.Children);
            }
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Force Rebuild Tree"), false, ForceRebuild);
        }

        protected void ForceRebuild()
        {
            rootVisualElement.Clear();
            CreateGUI();
        }

        /// <summary> Used to create the TreeMenu hierarchy </summary>
        protected abstract void BuildMenuTree(TreeMenu<TContent> tree);

        /// <summary> Called when instantiating a VisualElement </summary>
        protected virtual Label OnCreateNewElement()
        {
            Label label = new Label();
            label.style.paddingTop = 4f;
            return label;
        }

        /// <summary> Called when Bind is required </summary>
        protected virtual void OnBindItem(Label label, TreeViewContent<TContent> content) => label.text = content.Title;

        /// <summary> Draw right side as Inspector and call child </summary>
        private void OnChangeSelection(TreeViewContent<TContent> selectedObject)
        {
            inspectorContainer.Clear();

            if (selectedObject.TargetObject != null)
            {
                VisualElement property = PropertyBuilder.BuildVisualElement(selectedObject.TargetObject);
                inspectorContainer.Add(property);
            }

            OnChangeSelection(selectedObject.TargetObject);
        }

        protected virtual void OnChangeSelection(TContent selectedObject) { }
    }
}