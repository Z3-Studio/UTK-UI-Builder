using System;
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
    public class OverviewWindow : ObjectMenuWindow
    {
        private const string WindowName = "Overview";

        [MenuItem(Z3Path.UiBuilderMenuPath + WindowName)]
        public static void OpenWindow()
        {
            GetWindow<OverviewWindow>(WindowName).Show();
        }

        protected override void BuildMenuTree(TreeMenu<object> tree)
        {
            const string AttributeDrawers = "Attribute Drawers";
            const string AttributeProcessors = "Attribute Processors";
            const string PropertyDrawers = "Property Drawers";

            // Attribute Drawers
            AddRoot(tree, AttributeDrawers, $"Attribute: {nameof(Z3VisualElementAttribute).ToBold()} Drawer: {nameof(Z3AttributeDrawer<Z3VisualElementAttribute>).ToBold()}");
            Add<TitlePreview>(tree, AttributeDrawers);
            Add<MinMaxSliderPreview>(tree, AttributeDrawers);
            Add<TypeSelectorPreview>(tree, AttributeDrawers);
            Add<SliderPreview>(tree, AttributeDrawers);
            Add<ReadOnlyPreview>(tree, AttributeDrawers);
            Add<PropertySettingsPreview>(tree, AttributeDrawers);
            Add<SceneReferencePreview>(tree, AttributeDrawers);

            // Attribute Processor
            AddRoot(tree, AttributeProcessors, $"Attribute: {nameof(Z3InspectorMemberAttribute).ToBold()} Drawer: {nameof(Z3InspectorMemberAttributeProcessor<Z3InspectorMemberAttribute>).ToBold()}");
            Add<OnInitInspectorPreview>(tree, AttributeProcessors);
            Add<OnCloseInspectorPreview>(tree, AttributeProcessors);
            Add<ButtonPreview>(tree, AttributeProcessors);
            Add<InfoBoxPreview>(tree, AttributeProcessors);

            // Property Drawers
            AddRoot(tree, PropertyDrawers, $"Custom drawers for specific Properties. Drawer: {nameof(Z3PropertyDrawer<object>).ToBold()}");
            Add<DictionaryPreview>(tree, PropertyDrawers);
            Add<InterfacePreview>(tree, PropertyDrawers);
                
            const string Experimental = "Experimental";
            AddRoot(tree, Experimental, "Experiements to build new ideas");
            Add<PopupPreview>(tree, Experimental);
            Add<BreadcrumbPreview>(tree, Experimental);
            tree.Add(Experimental + "/Joystick", new JoystickPreview() { });

            tree.Add(Experimental + "/Table List", new TablePreview());
            tree.Add(Experimental + "/Progress Bar", new ProgressBar() { value = 60 });
            tree.Add(Experimental + "/Pie Chart", new PieChart() { value = 60 });
            tree.Add(Experimental + "/Radial Progress", new RadialProgress() { progress = 60 });
            tree.Add(Experimental + "/Drag Float Field", new DragFloatField("Drag Float Field") { value = 0.5f });
            tree.Add(Experimental + "/Color Picker Field", new ColorPickerField() { value = new(.2f, .4f, .6f, .8f)});

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
    }


    /// <summary>
    /// https://docs.unity3d.com/Manual/UIE-radial-progress.html
    /// https://docs.unity3d.com/Manual/UIE-radial-progress-use-vector-api.html
    /// </summary>
    [UxmlElement]
    public partial class RadialProgress : VisualElement
    {
        [UxmlAttribute]
        public float Progress { get; set; }

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