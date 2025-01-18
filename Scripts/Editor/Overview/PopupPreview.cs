using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils;
using System.Linq;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="SelectionPopup{T}"/>
    /// </summary>
    public class PopupPreview
    {
        public string selectedValue;

        [Button]
        public void ShowPopup()
        {
            List<Type> derivedTypes = ReflectionUtils.GetDeriveredConcreteTypes<Z3VisualElementAttribute>().ToList();
            SelectionPopup<Type>.Open("Title", derivedTypes, AddItem, t => t.Name.ToNiceString());
        }

        void AddItem(Type type)
        {
            selectedValue = type?.Name;
        }
    }
}