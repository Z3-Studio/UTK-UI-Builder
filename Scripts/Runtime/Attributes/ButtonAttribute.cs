using System;

namespace Z3.UIBuilder.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ButtonAttribute : Z3InspectorMemberAttribute
    {
        public string Name { get; set; }

        public ButtonAttribute() { }
        public ButtonAttribute(string name) 
        {
            Name = name;
        }
    }
}