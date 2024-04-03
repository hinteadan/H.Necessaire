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
    }
}
