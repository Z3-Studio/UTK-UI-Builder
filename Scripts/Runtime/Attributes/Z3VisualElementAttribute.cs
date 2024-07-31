namespace Z3.UIBuilder.Core
{
    /// <summary>
    /// Created to make adding multiple <see cref="UnityEngine.PropertyAttribute"/> easy
    /// </summary>
    /// <remarks>
    /// NOTE: It doesn't work with collection. In this case you have to use PropertyCollectionAttribute
    /// <para> Luckily in version 2023.3 or Unity 6 there will be a new attribute that works with both! </para>
    /// </remarks>
    /// https://discussions.unity.com/t/please-replace-propertycollectionattribute-with-a-bool-property-in-propertyattribute/919513
    public class Z3VisualElementAttribute : UnityEngine.PropertyAttribute { }
}