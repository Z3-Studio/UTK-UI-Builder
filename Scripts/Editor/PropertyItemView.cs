using System;
using System.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    public class PropertyItemView : VisualElement
    {
        [UIElement] private VisualElement elementName;

        private PropertyListField propertyListField;
        private int index;

        public PropertyItemView()
        {
            UIBuilderResources.PropertyListElementVT.CloneTree(this);
            this.BindUIElements();
        }

        public void Setup(PropertyListField propertyListField, int index)
        {
            this.propertyListField = propertyListField;
            this.index = index;

            SerializedProperty property = propertyListField.property.GetArrayElementAtIndex(index);

            IBaseFieldReader baseField = EditorBuilder.GetElement(propertyListField.ElementType);
            baseField.SetLabel(property.displayName);
            baseField.SetValue(property.GetValue());

            BindableElement element = baseField.VisualElement;
            element.bindingPath = property.propertyPath;
            element.Bind(property.serializedObject);

            baseField.OnValueChange += propertyListField.ContentChange;

            elementName.Add(element);
        }

        [UIElement("remove-button")]
        private void OnRemove()
        {
            propertyListField.RemoveItem(index);
        }
    }

    public class PropertyListField : VisualElement
    {
        public SerializedProperty property { get; }
        public Type ElementType { get; }

        public event Action OnValueChanged;

        private IList source;
        private ListView listView;

        public PropertyListField(SerializedProperty property)
        {
            this.property = property;
            source = (IList)property.GetValue();

            ElementType = source.GetType().GetGenericArguments()[0];
            Build();
        }

        internal void ContentChange() => OnValueChanged?.Invoke();

        [UIElement("add-button")]
        public void OnAddNewElement()
        {
            source.Add(default);
            property.serializedObject.Update();
            Rebuild();
        }

        public void RemoveItem(int index)
        {
            source.RemoveAt(index);
            Rebuild();
        }

        public void Rebuild()
        {
            listView.Rebuild();
        }

        private void Build()
        {
            listView = new ListView(source);
            listView.style.marginTop = 8;

            // Essencial
            //listView.selectionChanged += OnSelectionChanged; // Add menu context to delete selection
            listView.makeItem = OnMake;
            listView.bindItem = OnBindItem;
            listView.itemIndexChanged += (o, n) => Rebuild(); // Review: Bug when reording
            listView.reorderable = true;

            // List Style
            listView.selectionType = SelectionType.Multiple;
            listView.reorderMode = ListViewReorderMode.Animated;
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;

            listView.showBorder = true;
            listView.showFoldoutHeader = false;
            listView.showBoundCollectionSize = false;
            listView.horizontalScrollingEnabled = false;
            listView.showAddRemoveFooter = false;

            VisualElement listHeader = UIBuilderResources.ListHeaderVT.CloneTree();
            listHeader.BindUIElements(this);

            if (listView.showFoldoutHeader)
            {
                listView.headerTitle = property.displayName;
                Toggle target = listView.Q<Toggle>();
                target.Add(listHeader);
            }
            else
            {
                // Note: Setting listView.headerTitle doesn't create a label
                VisualElement row = new();
                row.style.flexDirection = FlexDirection.Row;
                row.style.justifyContent = Justify.SpaceBetween;

                Label listName = new Label(property.displayName);
                row.Add(listName);
                row.Add(listHeader);

                listView.hierarchy.Insert(0, row);
            }

            Label label = listView.Q<Label>();
            label.style.marginTop = 3;

            Add(listView);
        }

        private VisualElement OnMake() => new PropertyItemView();

        private void OnBindItem(VisualElement e, int index)
        {
            PropertyItemView i = e as PropertyItemView;
            i.Setup(this, index);
        }
    }
}