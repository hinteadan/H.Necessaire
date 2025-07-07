using System;

namespace H.Necessaire
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IDAttribute : Attribute, IStringIdentity
    {
        public IDAttribute(string id)
        {
            ID = id;
        }

        public string ID { get; }
    }
}
