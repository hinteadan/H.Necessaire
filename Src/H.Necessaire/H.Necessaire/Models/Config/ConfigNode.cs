using System;
using System.Linq;

namespace H.Necessaire
{
    public class ConfigNode : IStringIdentity
    {
        public ConfigNode Get(string id)
        {
            if ((Value?.ToObject() ?? string.Empty) is string)
                return null;

            return
                (Value.ToObject() as ConfigNode[])
                ?.FirstOrDefault(x => string.Equals(x?.ID, id, System.StringComparison.InvariantCultureIgnoreCase))
                ;
        }

        public ConfigNode[] GetAll()
        {
            return
                Value
                ?.ToObject()
                as
                ConfigNode[]
                ??
                new ConfigNode[0]
                ;
        }

        public string[] GetAllStrings()
        {
            return
                GetAll()
                .Select(x => x?.ToString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray()
                ;
        }

        public string ID { get; set; }
        public MultiType<string, ConfigNode[]> Value { get; set; } = null as string;

        public override string ToString()
        {
            return Value?.ToString();
        }

        public string ToStringWithId()
        {
            return $"{ID}={ToString()}";
        }
    }
}
