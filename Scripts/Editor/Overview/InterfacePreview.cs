using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Implementation: <see cref="SerializableInterfacePropertyDrawer"/>
    /// </summary>
    public class InterfacePreview
    {
        public SerializableInterface<IInterfaceExample> serializableInterface;

        public InterfacePreview()
        {
            serializableInterface = ScriptableObject.CreateInstance<ExampleSO>();
        }
    }

    public interface IInterfaceExample
    {

    }

    public class ExampleSO : ScriptableObject, IInterfaceExample
    {
        [SerializeField] private string myField;
    }
}