using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Integration.DotNet.Concrete
{
    public class NetCoreConfigProvider : ImAConfigProvider
    {
        public static void RegisterWith(ImADependencyRegistry dependencyRegistry, IConfiguration configuration)
        {
            if (dependencyRegistry is null)
                return;

            if (configuration is null)
                return;

            RuntimeConfig hRuntimeConfig = dependencyRegistry.GetRuntimeConfig();

            dependencyRegistry.Register<ImAConfigProvider>(() => new NetCoreConfigProvider(hRuntimeConfig, configuration));
        }

        #region Construct
        readonly IConfiguration configuration;
        readonly RuntimeConfig hRuntimeConfig;
        readonly Lazy<RuntimeConfig> lazyRuntimeConfig;
        public NetCoreConfigProvider(RuntimeConfig hRuntimeConfig, IConfiguration configuration)
        {
            this.hRuntimeConfig = hRuntimeConfig;
            this.configuration = configuration;
            this.lazyRuntimeConfig = new Lazy<RuntimeConfig>(BuildRuntimeConfig);
        }
        #endregion

        public RuntimeConfig GetRuntimeConfig() => lazyRuntimeConfig.Value;

        RuntimeConfig BuildRuntimeConfig()
        {
            if (configuration is null)
                return hRuntimeConfig;

            RuntimeConfig runtimeConfig = new RuntimeConfig
            {
                Values 
                    = (hRuntimeConfig?.Values ?? Array.Empty<ConfigNode>())
                    .Concat(configuration.GetChildren()?.Select(Map).ToArray() ?? Array.Empty<ConfigNode>())
                    .ToArray()
                    ,
            };

            return runtimeConfig;
        }

        static ConfigNode Map(IConfigurationSection configurationSection)
        {
            IEnumerable<IConfigurationSection> children = configurationSection.GetChildren();

            if (!children?.Any() ?? true)
            {
                return
                    new ConfigNode
                    {
                        ID = configurationSection.Key,
                        Value = configurationSection.Value,
                    };
            }

            return
                new ConfigNode
                {
                    ID = configurationSection.Key,
                    Value = children.Select(Map).ToArray(),
                };
        }
    }
}
