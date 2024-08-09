using System;

namespace Z3.UIBuilder.Core
{
    public sealed class ClassStyleAttribute : Attribute // TODO: Create implementation
    {
        public string[] ClassStyles { get; }

        public ClassStyleAttribute(params string[] classStyles)
        {
            ClassStyles = classStyles;
        }
    }
}