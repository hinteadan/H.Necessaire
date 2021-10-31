using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Commands
{
    public class TesticlesUseCase : CommandBase
    {
        public override async Task<OperationResult> Run()
        {
            UserInfo user = (await EnsureAuthentication()).ThrowOnFailOrReturn().SecurityContext.User;

            Console.WriteLine($"All OK from {(user?.DisplayName ?? user?.Username ?? "Unknown User")} @ {DateTime.Now}");

            return OperationResult.Win();
        }
    }
}
