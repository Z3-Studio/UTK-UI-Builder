using System;

namespace Z3.UIBuilder.Core
{
    /// <summary>
    /// Called when the inspector is detached from the panel
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnCloseInspectorAttribute : Z3InspectorMemberAttribute { }
}
