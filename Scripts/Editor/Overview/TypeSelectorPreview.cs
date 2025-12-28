using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="TypeSelectorDrawer"/>
    /// </summary>
    public class TypeSelectorPreview : VisualElement
    {
        [TypeSelector, SerializeReference]
        public ITypeSelectorExample fieldExample;


        [TypeSelector, SerializeReference]
        public List<ITypeSelectorExample> listExample = new()
        {
            new TypeSelectorExampleA(),
            new TypeSelectorExampleB(),
            new TypeSelectorExampleC(),
        };

        public TypeSelectorPreview()
        {
            FieldInfo fieldInfo = GetType().GetField(nameof(fieldExample));
            TypeSelector fieldSelector = new(fieldInfo, this);
            Add(fieldSelector);

            TypeSelector listSelector = new(listExample, nameof(listExample).ToNiceString());
            Add(listSelector);
        }
    }

    public interface ITypeSelectorExample { }

    public class TypeSelectorExampleA : ITypeSelectorExample
    {
        [SerializeField] private string stringField;
    }

    public class TypeSelectorExampleB : ITypeSelectorExample
    {
        [SerializeField] private int intField;
    }

    public class TypeSelectorExampleC : ITypeSelectorExample
    {
        [SerializeField] private GameObject objectField;
    }
}