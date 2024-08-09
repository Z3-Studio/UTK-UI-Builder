using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class InterfacePreview
    {
        public SerializedInterface<IExample> dictionary;

        public interface IExample
        {

        }

        public class ExampleSO : ScriptableObject, IExample
        {

        }
    }
}