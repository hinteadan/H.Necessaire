using System.Reflection;

namespace H.Necessaire.Runtime.MAUI.WellKnown.FluentUI
{
    public static class WellKnownFluentUiGlyphVariant
    {
        public const string Filled = "Filled";
        public const string Light = "Light";
        public const string Regular = "Regular";
        public const string Resizable = "Resizable";

        static readonly Lazy<string[]> allWellKnownVariants = new Lazy<string[]>(BuildAllWellKnownVariants);
        public static string[] AllVariants => allWellKnownVariants.Value;

        public static string GetVariantOrDefault(string variant)
        {
            return AllVariants.FirstOrDefault(v => v.Is(variant)) ?? Filled;
        }

        static string[] BuildAllWellKnownVariants()
        {
            return
                typeof(WellKnownFluentUiGlyphVariant)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => f.GetValue(null) as string)
                .ToArray()
                ;
        }
    }
}
