using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{

    /// <summary>
    /// Implementation: <see cref="BreadcrumbView"/>
    /// </summary>
    public class BreadcrumbPreview : VisualElement
    {
        private readonly BreadcrumbView breadcrumbView = new();
        private readonly Label currentValue = new();
        private int path;

        public BreadcrumbPreview()
        {
            Button addItem = new()
            {
                text = "Add Item",
                clickable = new Clickable(AddBreadcrumbItem)
            };

            // Create initial path
            for (int i = 0; i < 3; i++)
            {
                AddBreadcrumbItem();
            }

            // Add elements to Visual Element
            Add(breadcrumbView);
            Add(currentValue);
            Add(addItem);

        }

        private void AddBreadcrumbItem()
        {
            path++;
            int currentPath = path;
            breadcrumbView.AddAndSelectBreadcrumb($"Item {path}", () => OnSelectBreadcrumb(currentPath));

            OnSelectBreadcrumb(currentPath);
        }

        private void OnSelectBreadcrumb(int currentPath)
        {
            path = currentPath;
            currentValue.text = $"Current value is: {path}";
        }
    }
}