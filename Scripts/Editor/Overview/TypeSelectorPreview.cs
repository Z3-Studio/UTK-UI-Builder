using System.Collections.Generic;
using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="TypeSelectorDrawer"/>
    /// </summary>
    public class TypeSelectorPreview
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

    public class TypeSelectorExampleD : ITypeSelectorExample // TODO: Fix this case
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
    }
}
