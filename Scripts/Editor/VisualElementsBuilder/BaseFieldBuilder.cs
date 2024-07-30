using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;
using UnityEditor;

namespace Z3.UIBuilder.Editor
{
    internal static class BaseFieldBuilder // TODO: Use TextValueField<T>
    {
        #region Visual Element
        internal static IntegerField CreateIntegerFieldFromVisualElement(FieldInfo field, object target)
        {
            if (field.GetValue(target) is not IntegerField integerField)
            {
                integerField = new IntegerField()
                {
                    name = field.Name,
                    label = field.Name.GetNiceString(),
                };
            }

            // Bind
            field.SetValue(target, integerField);

            return integerField;
        }
        #endregion

        internal static IBaseFieldReader CreateBaseField(Type fieldType)
        {
            if (fieldType == null)
                return new BaseFieldReader<object>(new Label($"Type is Null. Considere fix or use {nameof(MonoScript)}"));

            if (fieldType == typeof(int)) return CreateField<IntegerField, int>();
            if (fieldType == typeof(float)) return CreateField<FloatField, float>();
            if (fieldType == typeof(string)) return CreateField<TextField, string>();
            if (fieldType == typeof(bool)) return CreateField<Toggle, bool>();
            if (fieldType == typeof(double)) return CreateField<DoubleField, double>();
            if (fieldType == typeof(long)) return CreateField<LongField, long>();
            if (fieldType == typeof(Gradient)) return CreateField<GradientField, Gradient>();
            if (fieldType == typeof(Hash128)) return CreateField<Hash128Field, Hash128>();
            if (fieldType == typeof(BoundsInt)) return CreateField<BoundsIntField, BoundsInt>();
            if (fieldType == typeof(Bounds)) return CreateField<BoundsField, Bounds>();
            if (fieldType == typeof(Color)) return CreateField<ColorField, Color>();
            if (fieldType == typeof(LayerMask)) return CreateField<LayerMaskField, int>();
            if (fieldType == typeof(RectInt)) return CreateField<RectIntField, RectInt>();
            if (fieldType == typeof(Rect)) return CreateField<RectField, Rect>();
            if (fieldType == typeof(Vector2Int)) return CreateField<Vector2IntField, Vector2Int>();
            if (fieldType == typeof(Vector2)) return CreateField<Vector2Field, Vector2>();
            if (fieldType == typeof(Vector3Int)) return CreateField<Vector3IntField, Vector3Int>();
            if (fieldType == typeof(Vector3)) return CreateField<Vector3Field, Vector3>();
            if (fieldType == typeof(Vector4)) return CreateField<Vector4Field, Vector4>();
            if (fieldType == typeof(AnimationCurve)) return CreateField<CurveField, AnimationCurve>();

            if (typeof(Object).IsAssignableFrom(fieldType)) 
                return CreateObjectFieldFromObject(fieldType);

            if (fieldType.IsEnum)
                return CreateEnumField(fieldType);

            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(BaseField<>)) 
                return CreateBaseField(fieldType.GenericTypeArguments[0]);

            //Type t when t == typeof(Matrix4x4) => BaseFieldBuilder.CreateField<Matrix4x4, Matrix4x4>(field, target),
            //Type t when t == typeof(Quaternion) => BaseFieldBuilder.CreateField<Vector3Field, Quaternion>(field, target),
            //Type t when t == typeof(RaycastHit) => BaseFieldBuilder.CreateField<Vector3Field, RaycastHit>(field, target),
            //Type t when t == typeof(RaycastHit2D) => BaseFieldBuilder.CreateField<Vector3Field, RaycastHit2D>(field, target),
            //Type t when t == typeof(string) => BaseFieldBuilder.CreateField<TagField, string>(field, target),
            //Type t when t == typeof(int) => BaseFieldBuilder.CreateField<MaskField, int>(field, target),
            //Type t when t == typeof(int) => BaseFieldBuilder.CreateField<LayerField, int>(field, target),

            return new BaseFieldReader<object>(new Label($"Unsuported type '{fieldType.FullName}'"));
        }

        public static IBaseFieldReader CreateField<TField, TValue>() where TField : BaseField<TValue>, new()
        {
            TField field = new TField();
            return new BaseFieldReader<TValue>(field);
        }

        private static IBaseFieldReader CreateObjectFieldFromObject(Type subType)
        {
            BaseField<Object> field = new ObjectField()
            {
                label = "None", // Required
                objectType = subType
            };

            return new BaseFieldReader<Object>(field);
        }

        public static IBaseFieldReader CreateEnumField(Type enumType)
        {
            bool isFlags = enumType.GetCustomAttribute<FlagsAttribute>() != null;
            Array enumValues = Enum.GetValues(enumType);
            Enum defaultValue = (Enum)enumValues.GetValue(0);

            BaseField<Enum> field;
            if (isFlags)
            {
                field = new EnumFlagsField(defaultValue);
            }
            else
            {
                field = new EnumField(defaultValue);
            }

            return new BaseFieldReader<Enum>(field);
        }
    }
}