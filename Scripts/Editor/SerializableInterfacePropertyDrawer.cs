using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;
using Object = UnityEngine.Object;

namespace Z3.UIBuilder.Editor
{
    /// <summary>
    /// Used to display <see cref="SerializableInterface{TInterface}"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableInterface<>), true)]
    public class SerializableInterfacePropertyDrawer : Z3PropertyDrawer<ISerializableInterface>
    {
        private Type InterfaceType => ResolvedValue.InterfaceType;

        private ObjectField objectField;
        private VisualElement display;

        private const string DisplayClass = "unity-object-field-display";
        private const string AcceptDropClass = "unity-object-field-display--accept-drop";
        private const string FieldPath = "serializedObject";

        protected override VisualElement CreateVisualElement()
        {
            objectField = new()
            {
                value = ResolvedValue.SerializedObject,
                objectType = InterfaceType,
                label = SerializedProperty.displayName
            };

            // Setup bind
            SerializedProperty serializedObjectField = SerializedProperty.FindPropertyRelative(FieldPath);
            objectField.BindProperty(serializedObjectField);

            // Prepare ObjectFieldDisplay
            display = objectField.Q(null, DisplayClass);

            display.RegisterCallback<DragUpdatedEvent>(ValidateDrag, TrickleDown.TrickleDown);
            display.RegisterCallback<DragPerformEvent>(ValidateDrag, TrickleDown.TrickleDown);
            display.RegisterCallback<DragLeaveEvent>(OnDragLeave, TrickleDown.TrickleDown);

            return objectField;
        }

        private void ValidateDrag(EventBase e)
        {
            if (DragAndDrop.objectReferences.Length == 0) 
                return;

            Object obj = GetObjectWithInterface(DragAndDrop.objectReferences[0]);
            bool ok = obj != null;
            DragAndDrop.visualMode = ok ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

            if (ok && e is DragPerformEvent)
            {
                objectField.value = obj;
                display.EnableInClassList(AcceptDropClass, false);
            }
            else
            {
                display.EnableInClassList(AcceptDropClass, ok);
            }
        }

        /// <summary> Used to remove Accetable drop color </summary>
        private void OnDragLeave(DragLeaveEvent e) => display.EnableInClassList(AcceptDropClass, false);

        /// <summary> Find Object to be serialized, if exist </summary>
        private Object GetObjectWithInterface(Object obj)
        {
            if (obj == null)
                return null;

            if (InterfaceType.IsAssignableFrom(obj.GetType()))
                return obj;

            if (obj is GameObject go && go.TryGetComponent(InterfaceType, out Component comp))
                return comp;

            return null;
        }
    }
}
