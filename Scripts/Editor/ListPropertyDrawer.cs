using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections;

// Note: I fould a problem in AssemblyDefinition showing "No GUI Implemented" in namespace field
// TODO: Just create a VisualElement class to display Global list. PropertyDrawers should be small
//namespace Z3.UIBuilder.Editor
//{    
//    // this is a old version of task list drawer
//    [CustomPropertyDrawer(typeof(IEnumerable), true)]
//    public class ListPropertyDrawer : Z3PropertyDrawer<IEnumerable> 
//    {
//        //  Create ListDrawSettingsAttribute
//        protected override VisualElement CreateVisualElement()
//        {
//            if (typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType))
//            {
//                Debug.Log("collection");
//            }

//            Debug.Log("Hello world");
//            return base.CreateVisualElement();
//        }
//    }
//}