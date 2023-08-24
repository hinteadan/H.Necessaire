using Bridge.Html5;
using System;
using System.IO;
using System.Reflection;

namespace H.Necessaire
{
    public static class BridgeDotNetExtensions
    {
        public static string ToUpperInvariant(this string value)
            => value?.ToUpperCase();

        public static string ToLowerInvariant(this string value)
            => value?.ToLowerCase();

        public static MethodInfo GetSetMethod(this PropertyInfo property)
            => property?.SetMethod;

        public static MethodInfo GetGetMethod(this PropertyInfo property)
            => property?.GetMethod;

        public static Stream OpenEmbeddedResource(this string embeddedResourceName)
            => throw new NotSupportedException("Embedded Resources are not yet supported in JavaScript");
    }
}
