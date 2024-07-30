using System;
using UnityEditor;
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
            const string AttributeDrawers = "Attribute Drawers/";
            const string AttributeProcessors = "Attribute Processors/";

            // Attribute Drawers
            Add<TitlePreview>(tree, AttributeDrawers);
            Add<MinMaxSliderPreview>(tree, AttributeDrawers);
            Add<SliderPreview>(tree, AttributeDrawers);
            Add<ReadOnlyPreview>(tree, AttributeDrawers);
            Add<PropertySettingsPreview>(tree, AttributeDrawers);

            // Attribute Processor
            Add<OnInitInspectorPreview>(tree, AttributeProcessors);
            Add<ButtonPreview>(tree, AttributeProcessors);

            // Should I display Attribute PropertyDrawer?
        }

        private void Add<T>(TreeMenu<object> tree, string path = "")
        {
            string className = typeof(T).Name;
            className = className.Replace("Preview", string.Empty);
            className = className.ToNiceString();

            path += className;

            T instance = Activator.CreateInstance<T>();
            tree.Add(path, instance);
        }
    }
}