using H.Necessaire.CLI.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Common
{
    internal sealed class CliCommandFactory : UseCaseBase, ImADependency
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

            OperationResult commandRunResult = OperationResult.Fail($"Command {commandName} not yet run");

            await
                new Func<Task>(async () =>
                {
                    commandRunResult = await command.Run();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while running command {commandName}. Reason: {ex.Message}.";
                        await Logger.LogError(message, ex);
                        commandRunResult = OperationResult.Fail(ex, message);
                    }
                );

            return commandRunResult;
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
