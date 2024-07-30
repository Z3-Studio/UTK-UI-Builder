using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Z3.UIBuilder
{
    public class BreadcrumbView : VisualElement
    {
        private readonly List<Button> breadcrumbs = new();

        private readonly ScrollView navigationScrollView;

        public override VisualElement contentContainer => navigationScrollView;
        public new class UxmlFactory : UxmlFactory<BreadcrumbView, UxmlTraits> { }

        public BreadcrumbView()
        {
            style.minHeight = new Length(24, LengthUnit.Pixel);
            style.paddingBottom = 2;

            // Crie um ScrollView
            navigationScrollView = new ScrollView()
            {
                mode = ScrollViewMode.Horizontal,
                verticalScrollerVisibility = ScrollerVisibility.Hidden,
                scrollDecelerationRate = 0,
                elasticity = 0
            };

            navigationScrollView.style.alignItems = Align.Stretch;

            // Adicione o ScrollView ao BreadcrumbView
            hierarchy.Add(navigationScrollView);
            //Add(navigationScrollView);
        }

        public void AddAndSelectBreadcrumb(string displayName, Action callback)
        {
            AddBreadcrumb(displayName, callback);
            callback();
        }

        /// <summary>
        /// Adds a breadcrumb button to the view.
        /// </summary>
        /// <param name="displayName"> Display name of the button. </param>
        /// <param name="callback"> Callback to invoke when the button is clicked. </param>
        public void AddBreadcrumb(string displayName, Action callback)
        {
            Button newBreadcrumb = new()
            {
                text = displayName
            };

            newBreadcrumb.style.maxWidth = 120;
            newBreadcrumb.style.minWidth = 80;
            newBreadcrumb.style.textOverflow = TextOverflow.Ellipsis;
            newBreadcrumb.style.unityTextAlign = TextAnchor.MiddleLeft;

            AddBreadcrumb(newBreadcrumb, callback);
        }

        /// <summary>
        /// Adds a breadcrumb button to the view.
        /// </summary>
        /// <remarks>
        /// This method adds a new breadcrumb button to the view with the specified display name and click callback.
        /// The button will be appended to the list of existing breadcrumbs.
        /// </remarks>
        /// <param name="newBreadcrumb"> The Button element representing the new breadcrumb. </param>
        /// <param name="callback"> Callback to invoke when the button is clicked. </param>
        public void AddBreadcrumb(Button newBreadcrumb, Action callback)
        {
            newBreadcrumb.SetEnabled(false);

            int breadcrumbIndex = breadcrumbs.Count;

            newBreadcrumb.clickable.clicked += () =>
            {
                SelectButton(breadcrumbIndex);
                callback?.Invoke();
            };

            breadcrumbs.Add(newBreadcrumb);
            Add(newBreadcrumb);

            // Enable previous breadcrumb
            if (breadcrumbIndex > 0)
            {
                breadcrumbs[breadcrumbIndex - 1].SetEnabled(true);
            }
        }

        /// <summary>
        /// Clears all breadcrumbs from the view.
        /// </summary>
        public void ClearBreadcrumbs()
        {
            navigationScrollView.Clear();
            breadcrumbs.Clear();
        }

        private void SelectButton(int index)
        {
            Button selection = breadcrumbs[index];

            // Remove all buttons after this index
            for (int i = breadcrumbs.Count - 1; i > index; i--)
            {
                breadcrumbs[i].RemoveFromHierarchy();
                breadcrumbs.RemoveAt(i);
            }

            // Disable selected button
            selection.SetEnabled(false);
        }
    }
}