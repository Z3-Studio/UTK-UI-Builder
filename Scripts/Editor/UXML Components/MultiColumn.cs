using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace Z3.UIBuilder.Editor
{
    public class CustomMultiColumnListView : MultiColumnListView
    {
        public new class UxmlFactory : UxmlFactory<CustomMultiColumnListView, UxmlTraits> { }

    }

    //public class CustomMultiColumnListViewController : MultiColumnListViewController
    //{
    //    public new class UxmlFactory : UxmlFactory<CustomMultiColumnListViewController, UxmlTraits> { }

    //}

    //public class CustomColumns : Columns
    //{
    //    public new class UxmlFactory : UxmlFactory<CustomColumns, UxmlTraits> { }

    //}

    //public class CustomColumn : Column
    //{
    //    public new class UxmlFactory : UxmlFactory<CustomColumn, UxmlTraits> { }

    //}


    public class Test : EditorWindow
    {
        private void CreateGUI()
        {
            MultiColumnListView multiColumnList = new MultiColumnListView();
            //multiColumnList.columns.Add(); Header
            var cols = multiColumnList.columns;
            cols["dd"].makeCell = () => new Label();
            //cols["dd"].bindCell = () => new Label(); bindingPath + bind (serializedObject)
            // set unbindCell ?

            multiColumnList.RefreshItems(); // after changes?
            // Show Alternating Row!
        }
    }
}