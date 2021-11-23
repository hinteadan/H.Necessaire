using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Commands
{
    public class TesticlesUseCase : CommandBase
    {
        public override async Task<OperationResult> Run()
        {
            UserInfo user = (await EnsureAuthentication()).ThrowOnFailOrReturn().SecurityContext.User;

            await Logger.LogInfo($"All OK from {(user?.DisplayName ?? user?.Username ?? "Unknown User")}");

            return OperationResult.Win();
        }
    }
}
