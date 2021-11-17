using H.Necessaire.RavenDB;
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
        { }

        static Func<Stream> ParseClientCertificateStreamFrom(RuntimeConfig runtimeConfig)
        {
            string clientCertificateAssemblyTypeName = runtimeConfig?.Get("RavenDB")?.Get("ClientCertificateAssemblyTypeName")?.ToString();
            string clientCertificateFilePath = runtimeConfig?.Get("RavenDB")?.Get("ClientCertificatePath")?.ToString();

            if (string.IsNullOrWhiteSpace(clientCertificateAssemblyTypeName) && string.IsNullOrWhiteSpace(clientCertificateFilePath))
                throw new InvalidOperationException("The RavenDB Configuration is invalid. Client Certificate is not configured. Either the file path must be specified or the embedded resource.");

            if (!string.IsNullOrWhiteSpace(clientCertificateFilePath))
            {
                FileInfo clientCertificateFile = new FileInfo(clientCertificateFilePath);
                if (!clientCertificateFile.Exists)
                    throw new InvalidOperationException($"The RavenDB Configuration is invalid. Client Certificate file doesn't exist: {clientCertificateFile.FullName}");

                return () => clientCertificateFile.OpenRead();
            }


            Assembly certificateAssembly = null;
            string certificateManifestResourceStreamName = null;
            Exception configException = null;
            new Action(() =>
            {
                certificateAssembly = Assembly.GetAssembly(Type.GetType(clientCertificateAssemblyTypeName));
                runtimeConfig?.Get("RavenDB")?.Get("ClientCertificateManifestResourceStreamName").Value.Read(x => certificateManifestResourceStreamName = x);
            }).TryOrFailWithGrace(onFail: ex => configException = ex);

            if (certificateAssembly == null)
                throw new InvalidOperationException("The RavenDB Configuration is invalid. Client Certificate assembly cannot be found.", configException);

            if (string.IsNullOrWhiteSpace(certificateManifestResourceStreamName))
                throw new InvalidOperationException("The RavenDB Configuration is invalid. Client Certificate resource stream name is empty.", configException);

            return () => certificateAssembly.GetManifestResourceStream(certificateManifestResourceStreamName);
        }

        static string[] ParseDatabaseUrlsFrom(RuntimeConfig runtimeConfig)
        {
            ConfigNode ravenConfig = runtimeConfig.Get("RavenDB");

            string[] databaseUrls = new string[0];

            ravenConfig.Get("DatabaseUrls")
                .Value
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

            dependencyRegistry
                .Register<Core.DependencyGroup>(() => new Core.DependencyGroup())
                .Register<Security.DependencyGroup>(() => new Security.DependencyGroup())
                ;
        }
    }
}
