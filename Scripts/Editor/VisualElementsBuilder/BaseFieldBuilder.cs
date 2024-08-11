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

            if (fieldType == typeof(int)) return CreateField<IntegerField, int>(fieldType);
            if (fieldType == typeof(float)) return CreateField<FloatField, float>(fieldType);
            if (fieldType == typeof(string)) return CreateField<TextField, string>(fieldType);
            if (fieldType == typeof(bool)) return CreateField<Toggle, bool>(fieldType);
            if (fieldType == typeof(double)) return CreateField<DoubleField, double>(fieldType);
            if (fieldType == typeof(long)) return CreateField<LongField, long>(fieldType);
            if (fieldType == typeof(Gradient)) return CreateField<GradientField, Gradient>(fieldType);
            if (fieldType == typeof(Hash128)) return CreateField<Hash128Field, Hash128>(fieldType);
            if (fieldType == typeof(BoundsInt)) return CreateField<BoundsIntField, BoundsInt>(fieldType);
            if (fieldType == typeof(Bounds)) return CreateField<BoundsField, Bounds>(fieldType);
            if (fieldType == typeof(Color)) return CreateField<ColorField, Color>(fieldType);
            if (fieldType == typeof(LayerMask)) return CreateField<LayerMaskField, int>(fieldType);
            if (fieldType == typeof(RectInt)) return CreateField<RectIntField, RectInt>(fieldType);
            if (fieldType == typeof(Rect)) return CreateField<RectField, Rect>(fieldType);
            if (fieldType == typeof(Vector2Int)) return CreateField<Vector2IntField, Vector2Int>(fieldType);
            if (fieldType == typeof(Vector2)) return CreateField<Vector2Field, Vector2>(fieldType);
            if (fieldType == typeof(Vector3Int)) return CreateField<Vector3IntField, Vector3Int>(fieldType);
            if (fieldType == typeof(Vector3)) return CreateField<Vector3Field, Vector3>(fieldType);
            if (fieldType == typeof(Vector4)) return CreateField<Vector4Field, Vector4>(fieldType);
            if (fieldType == typeof(AnimationCurve)) return CreateField<CurveField, AnimationCurve>(fieldType);

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

        public static IBaseFieldReader CreateField<TField, TValue>(Type fieldType) where TField : BaseField<TValue>, new()
        {
            TField field = new TField();
            field.label = fieldType.Name.GetNiceString();
            return new BaseFieldReader<TValue>(field);
        }

        private static IBaseFieldReader CreateObjectFieldFromObject(Type fieldType)
        {
            BaseField<Object> field = new ObjectField()
            {
                objectType = fieldType,
                label = fieldType.Name.GetNiceString()
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