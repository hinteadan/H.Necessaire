using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Integration.NetCore.Concrete
{
    public class NetCoreConfigProvider : ImAConfigProvider
    {
        #region Construct
        readonly IConfiguration configuration;
        public NetCoreConfigProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        #endregion

        public RuntimeConfig GetRuntimeConfig()
        {
            if (configuration == null)
                return new RuntimeConfig();

            return
                new RuntimeConfig
                {
                    Values = configuration.GetChildren()?.Select(Map).ToArray() ?? new ConfigNode[0],
                };
        }

        static ConfigNode Map(IConfigurationSection configurationSection)
        {
            IEnumerable<IConfigurationSection> children = configurationSection.GetChildren();

            if (!children?.Any() ?? true)
            {
                return
                    new ConfigNode
                    {
                        Id = configurationSection.Key,
                        Value = configurationSection.Value,
                    };
            }

            return
                new ConfigNode
                {
                    Id = configurationSection.Key,
                    Value = children.Select(Map).ToArray(),
                };
        }
    }
}
