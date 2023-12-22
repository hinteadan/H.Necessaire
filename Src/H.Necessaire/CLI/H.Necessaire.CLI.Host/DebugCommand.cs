using DeviceDetectorNET;
using DeviceDetectorNET.Parser;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Client;
using H.Necessaire.Runtime.Azure.CosmosDB.Core;
using H.Necessaire.Runtime.CLI.Commands;
using H.Necessaire.Runtime.Security.Managers;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Host
{
    internal class DebugCommand : CommandBase
    {
        HsCosmosDebugger cosmosDebugger;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            cosmosDebugger = dependencyProvider.Get<HsCosmosDebugger>();
        }

        public override async Task<OperationResult> Run()
        {
            await cosmosDebugger.Debug();
            return OperationResult.Win();
        }
    }
}
