using System.Linq;

namespace H.Necessaire
{
    public class RuntimeConfig
    {
        public ConfigNode Get(string id)
        {
            return
                Values
                ?.FirstOrDefault(x => string.Equals(x?.Id, id, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public ConfigNode[] Values { get; set; } = new ConfigNode[0];
    }
}
