using System.Linq;

namespace H.Necessaire.Runtime.Config.DataContracts
{
    public class RuntimeConfig
    {
        public ConfigNode this[string id]
        {
            get
            {
                return
                    Values
                    ?.FirstOrDefault(x => string.Equals(x?.Id, id, System.StringComparison.InvariantCultureIgnoreCase))
                    ;
            }
        }

        public ConfigNode[] Values { get; set; } = new ConfigNode[0];
    }
}
