using System.Linq;

namespace H.Necessaire.Runtime.Config.DataContracts
{
    public class ConfigNode
    {
        public MultiType<string, ConfigNode[]> this[string id]
        {
            get
            {
                if ((Value?.ToObject() ?? string.Empty) is string)
                    return null;

                return
                    (Value.ToObject() as ConfigNode[])
                    ?.FirstOrDefault(x => string.Equals(x?.Id, id, System.StringComparison.InvariantCultureIgnoreCase))
                    ?.Value;
            }
        }

        public string Id { get; set; }
        public MultiType<string, ConfigNode[]> Value { get; set; } = null as string;
    }
}
