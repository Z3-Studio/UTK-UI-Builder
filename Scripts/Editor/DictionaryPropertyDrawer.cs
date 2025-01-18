using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Used to display <see cref="SerializableDictionary{TKey, TValue}"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(IDictionary), true)]
    public class DictionaryPropertyDrawer : Z3PropertyDrawer<IDictionary>
    {
        [UIElement] private Label header;
        [UIElement] private Button addButton;
        [UIElement] private VisualElement newItemContainer;
        [UIElement] private VisualElement content;

        protected override VisualElement CreateVisualElement()
        {
            VisualElement listHeader = UIBuilderResources.DictionaryHeaderVT.CloneTree();
            listHeader.BindUIElements(this);

            // Title
            header.text = SerializedProperty.displayName + " [EXPERIMENTAL FEATURE]";

            // Key add 
            Type keyType = ResolvedValue.GetType().GetGenericArguments()[0];
            IBaseFieldReader keyField = EditorBuilder.GetElement(keyType);
            keyField.SetLabel("New Key");
            newItemContainer.Add(keyField.VisualElement);

            // Value add
            Type valueType = ResolvedValue.GetType().GetGenericArguments()[1];
            IBaseFieldReader valueField = EditorBuilder.GetElement(keyType);
            valueField.SetLabel("New Value");
            newItemContainer.Add(valueField.VisualElement);

            // Add btn
            addButton.clickable = new Clickable(Add);

            void Add()
            {
                ResolvedValue.Add(keyField.Value, valueField.Value);
                DrawContent();
            }

            DrawContent();

            return listHeader;
        }

        private void DrawContent()
        {
            content.Clear();

            SerializedProperty pairs = SerializedProperty.FindPropertyRelative("pairs");

            List<(SerializedProperty key, SerializedProperty value)> depedencies = new();

            for (int i = 0; i < pairs.arraySize; i++)
            {
                SerializedProperty pair = pairs.GetArrayElementAtIndex(i);
                SerializedProperty keyP = pair.FindPropertyRelative("key");
                SerializedProperty valueP = pair.FindPropertyRelative("value");

                depedencies.Add((keyP, valueP));
            }

            List<Column> variableColumns = new()
            {
                new Column()
                {
                    name = "key",
                    title = "Key",
                    width = 300,
                    makeCell = () => new PropertyField(),
                    bindCell = (e, i) => (e as PropertyField).BindProperty(depedencies[i].key)
                },
                new Column()
                {
                    name = "value",
                    title = "Value",
                    width = 300,
                    makeCell = () => new PropertyField(),
                    bindCell = (e, i) => (e as PropertyField).BindProperty(depedencies[i].value)
                },
                new Column()
                {
                    name = "actions",
                    title = "Action",
                    width = 40,
                    makeCell = () => new Button() { text = "X" },
                    bindCell = (e, i) =>
                    {
                        Button button = e as Button;
                        button.clickable = new Clickable(Remove);

                        void Remove()
                        {
                            object keyResolved = depedencies[i].key.GetValue();
                            ResolvedValue.Remove(keyResolved);

                            DrawContent();
                        }
                    }
                }
            };

            MultiColumnListView variableTable = new MultiColumnListView();
            variableColumns.ForEach(c => variableTable.columns.Add(c));
            variableTable.itemsSource = depedencies;

            content.Add(variableTable);
        }

        private void DictionaryBind()
        {
            /*
            content.Clear();

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty pairs = serializedObject.FindProperty("pairs");

            MultiColumnListView variableTable = new MultiColumnListView
            {
                bindingPath = "pairs",
                showBoundCollectionSize = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };

            // Adiciona colunas diretamente vinculadas às propriedades
            variableTable.columns.Add(new Column { bindingPath = "key", title = "Key", width = 300 });
            variableTable.columns.Add(new Column { bindingPath = "value", title = "Value", width = 300 });

            variableTable.columns.Add(new Column
            {
                title = "Action",
                width = 40,
                makeCell = () => new Button { text = "X" },
                bindCell = (e, i) =>
                {
                    Button button = e as Button;
                    button.clickable = new Clickable(() =>
                    {
                        pairs.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                    });
                }
            });

            // Faz o binding ao SerializedObject
            variableTable.Bind(serializedObject);


            content.Add(variableTable);*/
        }
    }
}