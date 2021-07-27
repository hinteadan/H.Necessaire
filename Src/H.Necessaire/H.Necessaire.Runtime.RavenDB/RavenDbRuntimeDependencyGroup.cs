using H.Necessaire.RavenDB;
using H.Necessaire.Runtime.Config.DataContracts;
using H.Necessaire.Runtime.RavenDB.Security.Resources;
using H.Necessaire.Runtime.Security.Resources;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.Runtime.RavenDB
{
    public class RavenDbRuntimeDependencyGroup : RavenDbDependencyGroup
    {
        #region Constructor
        public RavenDbRuntimeDependencyGroup(RuntimeConfig runtimeConfig)
            : base(
                  ParseClientCertificateStreamFrom(runtimeConfig),
                  ParseDatabaseUrlsFrom(runtimeConfig)
            )
        {

        }

        static Func<Stream> ParseClientCertificateStreamFrom(RuntimeConfig runtimeConfig)
        {
            ConfigNode ravenConfig = runtimeConfig[RavenDbRuntimeConfigKey.RavenDbNodeId];

            Assembly certificateAssembly = null;
            string certificateManifestResourceStreamName = null;
            Exception configException = null;
            new Action(() =>
            {
                ravenConfig[RavenDbRuntimeConfigKey.RavenDbClientCertificateAssemblyTypeName].Read(x => certificateAssembly = Assembly.GetAssembly(Type.GetType(x)));
                ravenConfig[RavenDbRuntimeConfigKey.RavenDbClientCertificateManifestResourceStreamName].Read(x => certificateManifestResourceStreamName = x);
            }).TryOrFailWithGrace(onFail: ex => configException = ex);


            if (certificateAssembly == null)
                throw new InvalidOperationException("The RavenDB Configuration is invalid. Client Certificate assembly cannot be found.", configException);

            if (string.IsNullOrWhiteSpace(certificateManifestResourceStreamName))
                throw new InvalidOperationException("The RavenDB Configuration is invalid. Client Certificate resource stream name is empty.", configException);

            return () => certificateAssembly.GetManifestResourceStream(certificateManifestResourceStreamName);
        }

        static string[] ParseDatabaseUrlsFrom(RuntimeConfig runtimeConfig)
        {
            ConfigNode ravenConfig = runtimeConfig[RavenDbRuntimeConfigKey.RavenDbNodeId];

            string[] databaseUrls = new string[0];

            ravenConfig[RavenDbRuntimeConfigKey.RavenDbDatabaseUrls]
                .Read(x =>
                    databaseUrls
                        = x
                        ?.Split(new char[] { ',', ';', ' ', '\t', '|' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(y => y?.Trim())
                        .Where(y => !string.IsNullOrWhiteSpace(y))
                        .ToArray()
                );

            if (!databaseUrls?.Any() ?? true)
                throw new InvalidOperationException("The RavenDB Configuration is invalid. Database URLs are not specified. At least one url needs to be specified.");

            return databaseUrls;
        }
        #endregion

        public override void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            base.RegisterDependencies(dependencyRegistry);

            dependencyRegistry.Register<ImAUserInfoStorageResource>(() => new RavenDbUserIdentityStorageResource());
            dependencyRegistry.Register<ImAUserCredentialsStorageResource>(() => new RavenDbUserCredentialsStorageResource());
            dependencyRegistry.Register<ImAUserAuthInfoStorageResource>(() => new RavenDbCachedUserAuthInfoStorageResource());

            dependencyRegistry.Register<RuntimeDependencyGroup>(() => new RuntimeDependencyGroup());
        }
    }
}
