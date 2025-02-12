using System;

namespace H.Necessaire
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DisplayLabelAttribute : Attribute
    {
        public DisplayLabelAttribute(string label)
        {
            DisplayLabel = label;
        }

        public string DisplayLabel { get; }
    }
}
