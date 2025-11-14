using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace H.Necessaire
{
    public static class EmbeddedResourceExtensions
    {
        static readonly ConcurrentDictionary<string, string[]> embeddedResourceNamesCacheDictionary = new ConcurrentDictionary<string, string[]>();
        public static Stream OpenEmbeddedResource(this string embeddedResourceName, params Assembly[] assembliesToScan)
        {
            assembliesToScan = !assembliesToScan.IsEmpty() ? assembliesToScan : AppDomain.CurrentDomain.GetNonCoreAssemblies();
            Assembly embeddedResourceAssembly = null;
            string embeddedResourceFullName = null;

            foreach (Assembly assembly in assembliesToScan)
            {
                new Action(() =>
                    {
                        embeddedResourceFullName
                            = embeddedResourceNamesCacheDictionary
                            .GetOrAdd(assembly.FullName, key => assembly.GetManifestResourceNames())
                            .FirstOrDefault(
                                x => x.EndsWith(embeddedResourceName, StringComparison.InvariantCultureIgnoreCase)
                            );
                    }
                ).TryOrFailWithGrace();

                if (embeddedResourceFullName != null)
                {
                    embeddedResourceAssembly = assembly;
                    break;
                }
            }

            if (embeddedResourceAssembly == null)
                return Stream.Null;

            return embeddedResourceAssembly.GetManifestResourceStream(embeddedResourceFullName);
        }

        public static string ReadSecretFromEmbeddedResources(this string secretName, string defaultTo, params Assembly[] assembliesToScan)
            => secretName.ReadSecretFromEmbeddedResources(defaultTo, encoding: Encoding.UTF8, assembliesToScan);
        public static string ReadSecretFromEmbeddedResources(this string secretName, Encoding encoding, params Assembly[] assembliesToScan)
            => secretName.ReadSecretFromEmbeddedResources(defaultTo: null, encoding, assembliesToScan);
        public static string ReadSecretFromEmbeddedResources(this string secretName, params Assembly[] assembliesToScan)
            => secretName.ReadSecretFromEmbeddedResources(defaultTo: null, assembliesToScan);
        public static string ReadSecretFromEmbeddedResources(this string secretName, string defaultTo, Encoding encoding, params Assembly[] assembliesToScan)
        {
            string secretValue = defaultTo;
            if (!HSafe.Run(() =>
            {
                using (Stream stream = secretName.OpenEmbeddedResource(assembliesToScan))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    secretValue = encoding.GetString(bytes);
                }
            }))
                return defaultTo;

            return secretValue;
        }

    }
}
