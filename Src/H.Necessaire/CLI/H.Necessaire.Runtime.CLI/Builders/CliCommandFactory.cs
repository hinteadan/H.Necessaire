using H.Necessaire.CLI.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Builders
{
    public sealed class CliCommandFactory : UseCaseBase, ImADependency
    {
        #region Construct
        ImADependencyProvider dependencyProvider;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            this.dependencyProvider = dependencyProvider;
        }
        #endregion

        public async Task<OperationResult> Run()
        {
            OperationResult<string> commandRetrieveResult = await GetCommandToRun();

            if (!commandRetrieveResult.IsSuccessful)
                return commandRetrieveResult;

            string commandName = commandRetrieveResult.Payload;

            ImACliCommand command = dependencyProvider?.Build<ImACliCommand>(commandName);

            if (command == null)
                return OperationResult.Fail($"Command [{commandName}] cannot be found or instantiated. Will display help here.");

            return await command.Run();
        }

        private async Task<OperationResult<string>> GetCommandToRun()
        {
            UseCaseContext context = await GetCurrentContext() ?? new UseCaseContext();

            string commandName = context.Notes.FirstOrDefault().ID;

            if (commandName.IsEmpty())
                commandName = "cli";

            return OperationResult.Win().WithPayload(commandName);
        }
    }
}
