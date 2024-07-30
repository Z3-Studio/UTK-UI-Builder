using UnityEditor;
using System;
using System.Collections.Generic;
using Z3.Utils;

namespace Z3.UIBuilder.Editor
{
    public static class UIBuilderCache
    {
        // Key: GenericTypeArgument of Z3PropertyAttribute, Value: AttributeDrawers
        public static Dictionary<Type, IZ3AttributeDrawer> AttributeDrawers { get; } = new();

        // Key: GenericTypeArgument of Z3PropertyDrawer, Value: PropertyDrawer Type
        public static Dictionary<Type, Type> PropertyDrawers { get; } = new();

        public static List<Z3InspectorMemberAttributeProcessor> AttributeProcessor { get; } = new();

        [InitializeOnLoadMethod]
        private static void CacheDrawers()
        {
            // Attribute Drawer
            // Example: Title, Slider, ReadOnly
            TypeCache.TypeCollection attributeDrawerTypes = TypeCache.GetTypesDerivedFrom(typeof(Z3AttributeDrawer<>));
            foreach (Type attributeDrawerType in attributeDrawerTypes)
            {
                if (attributeDrawerType.IsAbstract)
                    continue;

                Type attributeType = ReflectionUtils.GetGenericArgumentFromBaseType(attributeDrawerType, typeof(Z3AttributeDrawer<>));

                IZ3AttributeDrawer attributeDrawer = Activator.CreateInstance(attributeDrawerType) as IZ3AttributeDrawer;
                AttributeDrawers[attributeType] = attributeDrawer;
            }

            // Attribute Processor
            // Example: Button, OnInitInspector
            TypeCache.TypeCollection attributeProcessorTypes = TypeCache.GetTypesDerivedFrom(typeof(Z3InspectorMemberAttributeProcessor<>));
            foreach (Type attributeProcessorType in attributeProcessorTypes)
            {
                if (attributeProcessorType.IsAbstract)
                    continue;

                Z3InspectorMemberAttributeProcessor processor = Activator.CreateInstance(attributeProcessorType) as Z3InspectorMemberAttributeProcessor;
                AttributeProcessor.Add(processor);
            }

            // Property Drawer
            // Example: List, Parameter<T>, TaskList<T>
            TypeCache.TypeCollection propertyDrawerTypes = TypeCache.GetTypesDerivedFrom(typeof(Z3PropertyDrawer<>));
            foreach (Type propertyDrawerType in propertyDrawerTypes)
            {
                if (propertyDrawerType.IsAbstract)
                    continue;

                Type propertyType = ReflectionUtils.GetGenericArgumentFromBaseType(propertyDrawerType, typeof(Z3PropertyDrawer<>));
                PropertyDrawers[propertyType] = propertyDrawerType;
            }
        }
    }
}