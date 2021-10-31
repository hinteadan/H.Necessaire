using H.Necessaire.Runtime.CLI.Commands;

namespace H.Necessaire.CLI.Commands
{
    public class PingCommand : CommandBase
    {
        public override async Task<OperationResult> Run()
        {
            UserInfo? user = (await EnsureAuthentication()).ThrowOnFailOrReturn().SecurityContext.User;

            Console.WriteLine($"Pong from {(user?.DisplayName ?? user?.Username ?? "[NoUser]")} @ {DateTime.Now}");

            return OperationResult.Win();
        }
    }
}
