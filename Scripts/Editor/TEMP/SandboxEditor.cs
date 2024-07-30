using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor.ExtensionMethods;
using System.Reflection;
using System;
using Unity.Properties;
using Codice.Client.BaseCommands.BranchExplorer;

namespace Z3.UIBuilder.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;

    //[Serializable]
    //public class Test
    //{
    //    [SerializeField] public GameObject go;
    //    [SerializeField] public int integer;
    //}

    //public class TestDrawer : EditorWindow
    //{
    //    private Test test;

    //    private void OnEnable()
    //    {
    //        Test obj = new();
    //    }

    //    private void CreateGUI()
    //    {
    //        // TODO: Draw test by VisualElement
    //    }

    //    private void OnGUI()
    //    {
    //        // TODO: Draw test by GUI
    //    }
    //}

    //[CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class SandboxEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return null;


            // TANKS TUTORIAL
            //Binding binding = new Binding();
            //BindableElement element = new();
            //UIScreenManager
            //PropertyContainer.IsPathValid(ref element, PropertyPath.FromName("value")))
            //    element.SetBinding("value", new Binding());

            // --------------- Default inspector


            Editor editor = Editor.CreateEditor(target);
            VisualElement root = editor.CreateInspectorGUI(); // unused
            IMGUIContainer defaultInspector = new IMGUIContainer(() => editor.OnInspectorGUI());

            root.Add(defaultInspector);


            // --------------- Default Property IMGUIContainer


            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty serializedProperty = serializedObject.FindProperty("ex"); // EX seria o nome da minha PROPRIEDADE Serializable
            IMGUIContainer imguiContainer = new IMGUIContainer();
            //container.Bind();
            imguiContainer.onGUIHandler += () =>
            {
                // Reflect modifies by code and update the ui
                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedProperty);
                serializedObject.ApplyModifiedProperties();
            };

            // ---------------- Tests

            // After draw attribute? Inspector init? 
            //ReflectionUtils.GetMethodsWith<OnCreateVisualElementsAttribute>(target)
            //    .ToList()
            //    .ForEach(m => m.Invoke(target, null));

            // Codigo de Test
            PropertyField prop = root.Q<PropertyField>("PropertyField:m_Script");
            VisualElement parent = prop.parent;
            PropertyField newProp = new PropertyField();

            //newProp.AddToClassList() prop.st;

            var objectField = new ObjectField() { label = "Script" };
            newProp.Add(objectField);
            //objectField.AddToClassList("unity-object-field");
            //objectField.AddToClassList("unity-inspector-element");
            parent.Insert(0, newProp);

            objectField.Children().First().AddToClassList("unity-property-field__label");
            objectField.Children().Last().AddToClassList("unity-property-field__input");
            //newProp.bindingPath = prop.bindingPath;
            //newProp.Bind(serializedObject);
            newProp.SetEnabled(false);

            // Field Tests
            FieldTest(root, serializedObject);

            return root;
        }

        private static void FieldTest(VisualElement root, SerializedObject serializedObject)
        {
            Label label = root.Q<Label>();
            label.bindingPath = serializedObject.FindProperty($"FieldName.Array.data[{0}]").propertyPath;
            label.Bind(serializedObject);
            label.RegisterValueChangedCallback((evt) =>
            {
                Debug.Log("Hi Test");
            });


            label.RegisterCallback<ChangeEvent<object>>(x =>
            {
                Debug.Log("Changes to SerializedProperty");
            });
        }

        private Editor editor;
        private VisualElement root;

        internal void DisplayNode(Object obj)
        {
            root.Clear();

            // Primary
            InspectorElement element = new InspectorElement();
            element.Bind(new SerializedObject(obj));
            root.Add(element);

            // Secondary
            PropertyField propertyField = new PropertyField();
            propertyField.Bind(new SerializedObject(obj));
            root.Add(propertyField);

            // Default Inspector
            Object.DestroyImmediate(editor);
            editor = Editor.CreateEditor(obj);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                editor.OnInspectorGUI();
            });
            root.Add(container);
        }


        void OnClick(VisualElement container) // Clickable
        {
            container.AddManipulator(new Clickable(Cick2));

            void Cick2()
            {

            }
        }

        public void Example()
        {
            VisualElement rootVisualElement = new VisualElement();
            string path = "Assets/_Project/Scripts/Editor/DevelopmentTools/";

            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            VisualElement label = new Label("Hello World! From C#");
            root.Add(label);

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "DevelopmentToolsWindow.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path + "DevelopmentToolsWindow.uss");
            VisualElement labelWithStyle = new Label("Hello World! With Style");
            labelWithStyle.styleSheets.Add(styleSheet);
            root.Add(labelWithStyle);

            //InspectorElement element = new InspectorElement();
            //element.Bind(new SerializedObject(GameData.Instance));
            //rootVisualElement.Add(element); 

            ObjectField objectField = rootVisualElement.Q<ObjectField>("Prefab");
            objectField.RegisterValueChangedCallback(e =>
            {
                Debug.Log(objectField.value);
            });
        }

        private void SerializedObjects()
        {
            //SerializedProperty iterator = serializedObject.GetIterator();
            //while (iterator.Next(false))
            //{
            //    if (iterator.isExpanded)
            //    {
            //    }
            //    else
            //    {
            //        PropertyField property = new PropertyField(iterator);
            //        //property.name = $"Z3:{property.name}";
            //        //property.bindingPath = item.propertyPath;
            //        property.Bind(serializedObject);
            //        root.Add(property);
            //    }
            //    Debug.Log(iterator.displayName);
            //    // Faça algo com a propriedade aqui...
            //}

            //property.bindingPath = nameof(genericProperty.property);
            //property.
            //return property.Q<VisualElement>("unity-content");
            //Debug.Log(property.childCount);
        }


        private static void AddRecursive(VisualElement root, SerializedObject serializedObject, SerializedProperty serializedProperty, object target)
        {
            string lastPropPath = string.Empty;
            foreach (SerializedProperty p in serializedProperty) // Interate 
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    AddRecursive(root, serializedObject, serializedProperty, target);
                }
                else if (string.IsNullOrEmpty(lastPropPath) || !p.propertyPath.Contains(lastPropPath))
                {
                    lastPropPath = p.propertyPath;

                    PropertyField property = new PropertyField(p);
                    property.bindingPath = p.propertyPath;
                    property.Bind(serializedObject);
                    property.name = p.name;

                    root.Add(property);
                }
            }
        }

        public static VisualElement CreateAsEditorVE(object instance) // Is not working
        {
            PropertyWrapper genericProperty = CreateInstance<PropertyWrapper>();
            genericProperty.property = instance;

            Editor editor = Editor.CreateEditor(genericProperty);
            return editor.CreateInspectorGUI();
        }

        public static PropertyWrapper CreateAsEditorIMGUI(object instance)
        {
            PropertyWrapper genericProperty = CreateInstance<PropertyWrapper>();
            genericProperty.property = instance;

            Editor editor = Editor.CreateEditor(genericProperty);
            //genericProperty.draw = () => editor.OnInspectorGUI();

            return genericProperty;
        }

        public static PropertyWrapper CreateAsPropertyFieldIMGUI(object instance)
        {
            PropertyWrapper genericProperty = CreateInstance<PropertyWrapper>();
            genericProperty.property = instance;

            SerializedObject serializedObject = new SerializedObject(genericProperty);

            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(genericProperty.property));

            //genericProperty.draw = () => EditorGUILayout.PropertyField(serializedProperty);

            return genericProperty;
        }

        //private Parameter MakeGeneric(Type type)
        //{
        //    Type variableType = typeof(Parameter<>).MakeGenericType(new Type[] { type });
        //    return (Parameter)Activator.CreateInstance(variableType);
        //}
    }
}