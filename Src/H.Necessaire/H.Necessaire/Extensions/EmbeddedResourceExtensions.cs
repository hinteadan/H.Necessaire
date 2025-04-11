using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace H.Necessaire
{
    public static class EmbeddedResourceExtensions
    {
        public static Stream OpenEmbeddedResource(this string embeddedResourceName, Assembly embeddedResourceAssembly = null)
        {
            string embeddedResourceFullName = null;

            if (embeddedResourceAssembly != null)
            {
                embeddedResourceFullName = HSafe.Run(() => embeddedResourceAssembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(embeddedResourceName, StringComparison.InvariantCultureIgnoreCase))).Payload;
            }
            else
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    embeddedResourceFullName = HSafe.Run(() => assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(embeddedResourceName, StringComparison.InvariantCultureIgnoreCase))).Payload;

                    if (embeddedResourceFullName != null)
                    {
                        embeddedResourceAssembly = assembly;
                        break;
                    }
                }

                if (embeddedResourceAssembly == null)
                    return null;
            }

            return embeddedResourceAssembly.GetManifestResourceStream(embeddedResourceFullName);
        }
    }
}
