using System;

namespace Z3.UIBuilder.Core
{
    // SceneObject vs Asset Only
    public sealed class UIElementClass : Attribute { }// RequireComponent
    public sealed class VisualTreeAttribute : Attribute { } // Path to find easily. Inspector parameters (DrawBefore, after, bind). Serialize field avalaible
    public sealed class DictionaryDrawerAttribute : Attribute { }// High priorirt
    public sealed class ValueDropdownAttribute : Attribute { } // High priorirt
    public sealed class LabelAttribute : Attribute { } // Live update using bind and label settings
    public sealed class AbstractObjectAttribute : Attribute { } // Params -> valid rypes + dorpdown
    public sealed class HideLabelAttribute : Attribute { } // Check item, remove from root

    public sealed class InlineButtonAttribute : Attribute { }
    public sealed class InlineEditorAttribute : Attribute { } // Scriptable Object vs other objects
    public sealed class HideReferenceObjectPicker : Attribute { }
    public sealed class InlineProperty : Attribute { }

    public sealed class TableMatrixAttribute : Attribute { }
    public sealed class AssetOnlyAttribute : Attribute { }

    public sealed class ShowIfAttribute : Attribute 
    {
        public ShowIfAttribute(string fieldName)
        {

        }
    }
    public sealed class EnableIfAttribute : Attribute { }
    public sealed class ShowPropertyAttribute : Attribute { }
    public sealed class ObjectFieldSettingsAttribute : Attribute { }

    // DisplayAllFields = Show properties and private fields
    public sealed class OnValueChangedAttribute : Attribute { } // Collection vs field

    public sealed class EnumToggleAttribute : Attribute { } // Toggle Button Group, Radio Button, Enum Button? (unity 6)
    public sealed class BoxPropertyAttribute : Attribute { } // property draw by default
    public sealed class TabAttribute : Attribute { } // low priority
    public sealed class CollorPaletteAttribute : Attribute { } // low priority
    public sealed class ProgressBarAttribute : Attribute { } // Component is already done?
    public sealed class FolderPathAttribute : Attribute { }
    public sealed class AssetPathAttribute : Attribute { }
    public sealed class SuffixLabel : Attribute { }
    public sealed class PrefixLabel : Attribute { }

    // SceneObject vs Asset Only


    public sealed class TableTreeAttribute : Attribute { }
    public sealed class TableListAttribute : Attribute { } // By default all fields are Column. Note: Check how search workds
    public sealed class ColumnAttribute : Attribute { } // Used to combine to items in same Column and change width
    public sealed class RowAttribute : Attribute { }
}