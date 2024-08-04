using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor.TreeViewer;
using Z3.UIBuilder.TreeViewer;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Get all drawers and processors
    /// </summary>
    /// 
    /// Core members
    /// <seealso cref="EditorBuilder"/>
    /// <seealso cref="UIBuilderCache"/>
    /// <seealso cref="PropertyBuilder"/>
    public class AttributesOverviewWindow : ObjectMenuWindow
    {
        private const string AttributeOverview = "Attributes Overview";

        [MenuItem(Z3Path.UiBuilderMenuPath + AttributeOverview)]
        public static void OpenWindow()
        {
            GetWindow<AttributesOverviewWindow>(AttributeOverview).Show();
        }

        protected override void BuildMenuTree(TreeMenu<object> tree)
        {
            const string AttributeDrawers = "Attribute Drawers";
            const string AttributeProcessors = "Attribute Processors";

            // Attribute Drawers
            AddRoot(tree, AttributeDrawers, $"Attribute: {nameof(Z3VisualElementAttribute).ToBold()} Drawer: {nameof(Z3AttributeDrawer<Z3VisualElementAttribute>).ToBold()}");
            Add<TitlePreview>(tree, AttributeDrawers);
            Add<MinMaxSliderPreview>(tree, AttributeDrawers);
            Add<SliderPreview>(tree, AttributeDrawers);
            Add<ReadOnlyPreview>(tree, AttributeDrawers);
            Add<PropertySettingsPreview>(tree, AttributeDrawers);

            // Attribute Processor
            AddRoot(tree, AttributeProcessors, $"Attribute: {nameof(Z3InspectorMemberAttribute).ToBold()} Drawer: {nameof(Z3InspectorMemberAttributeProcessor<Z3InspectorMemberAttribute>).ToBold()}");
            Add<OnInitInspectorPreview>(tree, AttributeProcessors);
            Add<ButtonPreview>(tree, AttributeProcessors);
            Add<InfoBoxPreview>(tree, AttributeProcessors);

            const string Experimental = "Experimental";
            AddRoot(tree, Experimental, "Experiements to build new ideas");
            tree.Add(Experimental + "/Table List", TableListDrawer.DrawTable());
            tree.Add(Experimental + "/Progress Bar", new ProgressBar() { value = 60 });
            tree.Add(Experimental + "/Pie Chart", new PieChart() { value = 60 });
            tree.Add(Experimental + "/Radial Progress", new RadialProgress() { progress = 60 });

            // Should I display Attribute PropertyDrawer?

            // TODO: Use reflection to find all classes as: https://www.foundations.unity.com/components            
        }

        private void AddRoot(TreeMenu<object> tree, string path, string description)
        {
            tree.Add(path, new Label(description));
        }

        private void Add<T>(TreeMenu<object> tree, string path = "")
        {
            string className = typeof(T).Name;
            className = className.Replace("Preview", string.Empty);
            className = className.ToNiceString();

            path = $"{path}/{className}";

            T instance = Activator.CreateInstance<T>();
            tree.Add(path, instance);
        }

        // -------------------------- EXPERIMENTS ------------------------
        private class TableListDrawer
        {
            public static VisualElement DrawTable()
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

                Columns columns = new Columns();
                columnArray.ForEach(column => columns.Add(column));

                MultiColumnListView table = new MultiColumnListView(columns);
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

                return table;
            }
        }

        /// <summary>
        /// PieChart https://docs.unity3d.com/Manual/UIE-pie-chart.html
        /// </summary>
        private class PieChart : VisualElement
        {
            float m_Radius = 100.0f;
            float m_Value = 40.0f;

            public float radius
            {
                get => m_Radius;
                set
                {
                    m_Radius = value;
                }
            }

            public float diameter => m_Radius * 2.0f;

            public float value
            {
                get { return m_Value; }
                set { m_Value = value; MarkDirtyRepaint(); }
            }

            public PieChart()
            {
                generateVisualContent += DrawCanvas;
            }

            void DrawCanvas(MeshGenerationContext ctx)
            {
                var painter = ctx.painter2D;
                painter.strokeColor = Color.white;
                painter.fillColor = Color.white;

                var percentage = m_Value;

                var percentages = new float[] { percentage, 100 - percentage };

                var colors = new Color32[] 
                {
                    Color.cyan,
                    Color.gray,
                };

                float angle = 0.0f;
                float anglePct = 0.0f;
                int k = 0;
                foreach (var pct in percentages)
                {
                    anglePct += 360.0f * (pct / 100);

                    painter.fillColor = colors[k++];
                    painter.BeginPath();
                    painter.MoveTo(new Vector2(m_Radius, m_Radius));
                    painter.Arc(new Vector2(m_Radius, m_Radius), m_Radius, angle, anglePct);
                    painter.Fill();

                    angle = anglePct;
                }
            }
        }

        /// <summary>
        /// https://docs.unity3d.com/Manual/UIE-radial-progress.html
        /// https://docs.unity3d.com/Manual/UIE-radial-progress-use-vector-api.html
        /// </summary>
        private class RadialProgress : VisualElement
        {
            public new class UxmlTraits : VisualElement.UxmlTraits
            {
                // The progress property is exposed to UXML.
                UxmlFloatAttributeDescription m_ProgressAttribute = new UxmlFloatAttributeDescription()
                {
                    name = "progress"
                };

                // Use the Init method to assign the value of the progress UXML attribute to the C# progress property.
                public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
                {
                    base.Init(ve, bag, cc);

                    (ve as RadialProgress).progress = m_ProgressAttribute.GetValueFromBag(bag, cc);
                }
            }

            // Define a factory class to expose this control to UXML.
            public new class UxmlFactory : UxmlFactory<RadialProgress, UxmlTraits> { }

            // These are USS class names for the control overall and the label.
            public static readonly string ussClassName = "radial-progress";
            public static readonly string ussLabelClassName = "radial-progress__label";

            // These objects allow C# code to access custom USS properties.
            static CustomStyleProperty<Color> s_TrackColor = new CustomStyleProperty<Color>("--track-color");
            static CustomStyleProperty<Color> s_ProgressColor = new CustomStyleProperty<Color>("--progress-color");

            Color m_TrackColor = Color.gray;
            Color m_ProgressColor = Color.red;

            // This is the label that displays the percentage.
            Label m_Label;

            // This is the number that the Label displays as a percentage.
            float m_Progress;

            // A value between 0 and 100
            public float progress
            {
                // The progress property is exposed in C#.
                get => m_Progress;
                set
                {
                    // Whenever the progress property changes, MarkDirtyRepaint() is named. This causes a call to the
                    // generateVisualContents callback.
                    m_Progress = value;
                    m_Label.text = Mathf.Clamp(Mathf.Round(value), 0, 100) + "%";
                    MarkDirtyRepaint();
                }
            }

            // This default constructor is RadialProgress's only constructor.
            public RadialProgress()
            {
                // Create a Label, add a USS class name, and add it to this visual tree.
                m_Label = new Label();
                m_Label.AddToClassList(ussLabelClassName);
                Add(m_Label);

                // Add the USS class name for the overall control.
                AddToClassList(ussClassName);

                // Register a callback after custom style resolution.
                RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

                // Register a callback to generate the visual content of the control.
                generateVisualContent += GenerateVisualContent;

                style.left = 20;
                style.top = 20;
                style.width = 200;
                style.height = 200;
            }

            static void CustomStylesResolved(CustomStyleResolvedEvent evt)
            {
                RadialProgress element = (RadialProgress)evt.currentTarget;
                element.UpdateCustomStyles();
            }

            // After the custom colors are resolved, this method uses them to color the meshes and (if necessary) repaint
            // the control.
            void UpdateCustomStyles()
            {
                bool repaint = false;
                if (customStyle.TryGetValue(s_ProgressColor, out m_ProgressColor))
                    repaint = true;

                if (customStyle.TryGetValue(s_TrackColor, out m_TrackColor))
                    repaint = true;

                if (repaint)
                    MarkDirtyRepaint();
            }

            void GenerateVisualContent(MeshGenerationContext context)
            {
                float width = contentRect.width;
                float height = contentRect.height;

                var painter = context.painter2D;
                painter.lineWidth = 10.0f;
                painter.lineCap = LineCap.Butt;

                // Draw the track
                painter.strokeColor = m_TrackColor;
                painter.BeginPath();
                painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, 0.0f, 360.0f);
                painter.Stroke();

                // Draw the progress
                painter.strokeColor = m_ProgressColor;
                painter.BeginPath();
                painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, -90.0f, 360.0f * (progress / 100.0f) - 90.0f);
                painter.Stroke();
            }
        }
    }
}