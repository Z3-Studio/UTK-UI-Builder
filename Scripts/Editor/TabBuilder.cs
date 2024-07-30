using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils;

namespace Z3.UIBuilder.Editor
{
    public static class TabBuilder
    {
        internal static void UpdateMembers(object target, VisualElement root)
        {
            List<(FieldInfo, TabAttribute)> query = ReflectionUtils.GetAllFieldsWithAttribute<TabAttribute>(target, "UnityEngine");
            TabView tabView = new TabView();
            root.Add(tabView);
            // TODO: Find Original and update hierarchy using TabView container as parent
        }
    }
}