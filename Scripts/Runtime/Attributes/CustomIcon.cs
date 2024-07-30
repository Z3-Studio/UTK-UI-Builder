using System;
using UnityEngine;

namespace Z3.UIBuilder.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class CustomIcon : Attribute
    {
        public string Method { get; }

        /// <param name="method"> Should return a <see cref="Texture2D"/> </param>
        public CustomIcon(string method)
        {
            Method = method;
        }
    }
}