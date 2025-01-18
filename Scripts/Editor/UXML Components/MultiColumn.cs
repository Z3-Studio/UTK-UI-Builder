using UnityEditor;
using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    [UxmlElement]
    public partial class CustomMultiColumnListView : MultiColumnListView { }

    //[UxmlElement]
    //public class CustomMultiColumnListViewController : MultiColumnListViewController { }

    //[UxmlElement]
    //public class CustomColumns : Columns { }

    //[UxmlElement]
    //public class CustomColumn : Column { }


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