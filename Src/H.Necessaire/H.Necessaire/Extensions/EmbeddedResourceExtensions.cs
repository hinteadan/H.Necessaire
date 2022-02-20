using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace H.Necessaire
{
    public static class EmbeddedResourceExtensions
    {
        public static Stream OpenEmbeddedResource(this string embeddedResourceName)
        {
            Assembly embeddedResourceAssembly = null;
            string embeddedResourceFullName = null;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                embeddedResourceFullName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(embeddedResourceName, StringComparison.InvariantCultureIgnoreCase));
                if (embeddedResourceFullName != null)
                {
                    embeddedResourceAssembly = assembly;
                    break;
                }
            }

            if (embeddedResourceAssembly == null)
                return null;

            return embeddedResourceAssembly.GetManifestResourceStream(embeddedResourceFullName);
        }
    }
}
