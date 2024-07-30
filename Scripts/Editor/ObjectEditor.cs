using UnityEditor;
using UnityEngine;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// BUG: https://issuetracker.unity3d.com/issues/propertydrawer-dot-createpropertygui-will-not-get-called-when-using-a-custompropertydrawer-with-a-generic-struct
    /// </summary>
    [CustomEditor(typeof(Object), true)]
    public class ObjectEditor : Z3Editor<Object> { }
}