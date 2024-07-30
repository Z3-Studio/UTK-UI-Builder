using System;

namespace Z3.UIBuilder.Core
{
    /// <summary>
    /// Used to bind Elemenets
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method /*TODO: | AttributeTargets.Property*/)]
    public sealed class UIElementAttribute : Attribute 
    {
        public string ElementName { get; }
        public bool Optional { get; }

        /// <summary>
        /// It will try to use declared name and use this converter <see cref="Utils.StringFormater.FormatToLowerCaseAndHyphenateWords"/>
        /// </summary>
        /// <remarks>
        /// Example: 
        /// <para> Declaration: variableTagName0 </para> 
        /// <para> Search Match: variable-tag-name-0 </para>
        /// </remarks>
        public UIElementAttribute(bool optional = false) // Idea: Maybe be possible to inherit to change string conventions?
        {
            Optional = optional;
        }

        public UIElementAttribute(string bindName, bool isPropertyField = false, bool optional = false) : this(optional)
        { 
            if (isPropertyField)
            {
                ElementName = "PropertyField:" + bindName;
            }
            else
            {
                ElementName = bindName;
            }
        }
    }
}