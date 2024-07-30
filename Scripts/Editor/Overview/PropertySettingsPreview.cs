using System;
using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class PropertySettingsPreview
    {
        [PropertySettings]
        public PropertyExample propertyExample;

        [Serializable]
        public class PropertyExample
        {
            public GameObject gameObject;
        }
    }
}