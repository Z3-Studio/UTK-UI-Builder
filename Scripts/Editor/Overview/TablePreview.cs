using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    // https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-uxml-element-MultiColumnListView.html

    // TODO: Create TableAttribute for lists, with option to be auto
    // Create ColumnAttribute for fields, with option to combine two fields at same Column
    public class TablePreview : VisualElement
    {
        public TablePreview()
        {
            /* // Tree View
            MultiColumnHeaderState.Column[] multiColumns = new[]
            {
            new MultiColumnHeaderState.Column { contextMenuText = "Column 1", width = 100 },
            new MultiColumnHeaderState.Column { contextMenuText = "Column 2", width = 100 },
            new MultiColumnHeaderState.Column { contextMenuText = "Column 3", width = 100 }
            };
            MultiColumnHeaderState headerState = new MultiColumnHeaderState(multiColumns);
            MultiColumnHeader header = new MultiColumnHeader(headerState);
            */

            const int Colums = 4;

            List<string[]> matrixTable = new List<string[]>
            {
                new string[Colums] { "1", "2", "3", "4" },
                new string[Colums] { "5", "6", "7", "8" },
                new string[Colums] { "9", "10", "11", "12" },
                new string[Colums] { "13", "14", "15", "16" }
            };

            List<Column> columnArray = new()  // Check fields here -> UnityEngine.UIElements.ColumnDataType
            {
                new Column()
                {
                    name = "column-a",
                    title = "Column A",
                    width = 100,
                    makeCell = () => new Label(),
                    bindCell = (e, i) =>
                    {
                        (e as Label).text = matrixTable[0][i];
                    }
                },
                new Column()
                {
                    name = "column-b",
                    title = "Column B",
                    width = 100,
                    makeCell = () => new Label(),
                    bindCell = (e, i) =>
                    {
                        (e as Label).text = matrixTable[1][i];
                    }
                },
                new Column()
                {
                    name = "column-c",
                    title = "Column C",
                    width = 100,
                    makeCell = () => new Label(),
                    bindCell = (e, i) =>
                    {
                        (e as Label).text = matrixTable[2][i];
                    }
                },
                new Column()
                {
                    name = "column-d",
                    title = "Column D",
                    width = 100,
                    makeCell = () => new Label(),
                    bindCell = (e, i) =>
                    {
                        (e as Label).text = matrixTable[3][i];
                    }
                },
            };

            MultiColumnListView table = new MultiColumnListView();
            columnArray.ForEach(column => table.columns.Add(column));
            table.itemsSource = matrixTable;
            table.sortingEnabled = true;

            table.columnSortingChanged += () =>
            {
                foreach (SortColumnDescription column in table.sortedColumns)
                {
                    if (column.columnName == "variable-name")
                    {
                        table.Sort((x, b) =>
                        {
                            Debug.Log(x + " ;; " + b);
                            return 0;
                        });
                    }

                    Debug.Log($"ColumnName: {column.columnName} Direction: {column.direction} ColumnIndex: {column.columnIndex}");
                }
            };

            table.sortColumnDescriptions.Add(new SortColumnDescription(0, SortDirection.Ascending));
            table.Sort((x, b) =>
            {
                Debug.Log(x + " // " + b);
                return 0;
            });

            Add(table);
        }
    }
}