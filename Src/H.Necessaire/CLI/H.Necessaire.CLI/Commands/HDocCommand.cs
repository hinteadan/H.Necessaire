using H.Necessaire.CLI.Commands.HDoc;
using H.Necessaire.Runtime.CLI.Commands;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands
{
    [ID("h-doc")]
    public class HDocCommand : CommandBase
    {
        HDocManager hDocManager;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            hDocManager = dependencyProvider.Get<HDocManager>();
        }

        public override async Task<OperationResult> Run()
        {
            return await hDocManager.ParseAndExportDocumentation(dstFolder: new System.IO.DirectoryInfo(@"C:\Users\Hintea Dan Alexandru\Downloads"));
        }
    }
}
