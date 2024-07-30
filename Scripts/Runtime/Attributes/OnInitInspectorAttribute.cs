using System;

namespace Z3.UIBuilder.Core
{
    /// <summary> Called after finish to build the visual tree </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnInitInspectorAttribute : Z3InspectorMemberAttribute
    {
    }
}