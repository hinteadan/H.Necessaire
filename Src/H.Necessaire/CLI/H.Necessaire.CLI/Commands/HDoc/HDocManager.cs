using H.Necessaire.CLI.Commands.HDoc.BLL;
using H.Necessaire.CLI.Commands.HDoc.Model;
using System.IO;

namespace H.Necessaire.CLI.Commands.HDoc
{
    internal class HDocManager : ImADependency
    {
        HDocCsProjParser hDocCsProjParser;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hDocCsProjParser = dependencyProvider.Get<HDocCsProjParser>();
        }

        public OperationResult<HDocumentation> ParseDocumentation(DirectoryInfo srcFolder = null)
        {
            OperationResult<HDocProjectInfo[]> csProjsResult = hDocCsProjParser.Parse(srcFolder);
            if (!csProjsResult.IsSuccessful)
                return csProjsResult.WithoutPayload<HDocumentation>();
            HDocProjectInfo[] csProjs = csProjsResult.Payload;

            return
                new HDocumentation
                {
                }
                .ToWinResult()
                ;
        }
    }
}
