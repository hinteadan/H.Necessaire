using System;

namespace H.Necessaire
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(params string[] categories)
        {
            Categories = categories;
        }

        public string[] Categories { get; }
    }
}
