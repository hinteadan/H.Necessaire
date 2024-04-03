using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML
{
    internal static class TemplateExtensions
    {
        public static async Task<string> ProcessEmbeddedResource(this ImATemplate template, string embeddedResourceID)
        {
            return
                await template.Process(
                    await embeddedResourceID.OpenEmbeddedResource().ReadAsStringAsync(isStreamLeftOpen: false),
                    await template.ReadParams()
                );
        }

        public static async Task<string> ProcessEmbeddedResource(this IEnumerable<ImATemplate> templates, string embeddedResourceID)
        {
            if (!templates.Any())
                return "";

            string[] sections
                = await Task.WhenAll(
                    templates.Select(template => template.ProcessEmbeddedResource(embeddedResourceID))
                );

            return
                string.Join(Environment.NewLine, sections);
        }
    }
}
