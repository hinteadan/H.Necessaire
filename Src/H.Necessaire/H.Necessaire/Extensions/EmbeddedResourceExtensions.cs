using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

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
    }
}
