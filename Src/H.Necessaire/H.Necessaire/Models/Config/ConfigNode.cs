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
