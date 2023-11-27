using H.Necessaire.CLI.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Builders
{
    public class CliCommandFactory : UseCaseBase, ImADependency
    {
        #region Construct
        static readonly string[] commandTypeNameEndings = new[] { "UseCase", "Command", "CliCommand", "CommandUseCase", "UseCaseCommand", "CliCommandUseCase", "CliUseCaseCommand" };
        readonly ImADependencyBrowser dependencyBrowser;
        ImADependencyProvider dependencyProvider;

        public CliCommandFactory(ImADependencyBrowser dependencyBrowser)
        {
            this.dependencyBrowser = dependencyBrowser;
        }

        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            this.dependencyProvider = dependencyProvider;
        }
        #endregion

        public async Task<OperationResult> Run(bool askForCommandIfEmpty = false)
        {
            OperationResult<string> commandRetrieveResult = await GetCommandToRun(askForCommandIfEmpty);

            if (!commandRetrieveResult.IsSuccessful)
                return commandRetrieveResult;

            string commandName = commandRetrieveResult.Payload;

            Type commandType
                =
                dependencyBrowser
                .GetAllAlwaysNewTypes()
                .SingleOrDefault(x => IsCommandNameMatch(commandName, x.Key) || x.Key.IsMatch(commandName))
                .Key

                ??

                dependencyBrowser
                .GetAllOneTimeTypes()
                .SingleOrDefault(x => IsCommandNameMatch(commandName, x.Key) || x.Key.IsMatch(commandName))
                .Key
                ;

            if (commandType == null)
                return OperationResult.Fail($"Command [{commandName}] doesn't exist. Will display help here.");

            ImACliCommand command = dependencyProvider?.Get(commandType) as ImACliCommand;

            if (command == null)
                return OperationResult.Fail($"Command [{commandName}] cannot be instantiated. Will display help here.");

            return await command.Run();
        }

        private async Task<OperationResult<string>> GetCommandToRun(bool askForCommandIfEmpty)
        {
            UseCaseContext context = await GetCurrentContext() ?? new UseCaseContext();

            string commandName = context.Notes.FirstOrDefault().ID;

            if (string.IsNullOrWhiteSpace(commandName) && !askForCommandIfEmpty)
                return OperationResult.Fail("Command name is empty. Will display help here.").WithoutPayload<string>();

            if (askForCommandIfEmpty && string.IsNullOrWhiteSpace(commandName))
            {
                Console.WriteLine("Please type in the command you want to run:");
                commandName = (Console.ReadLine() ?? string.Empty).Trim();
            }

            if (string.IsNullOrWhiteSpace(commandName))
                return OperationResult.Fail("Command name is empty. Will display help here.").WithoutPayload<string>();

            return OperationResult.Win().WithPayload(commandName);
        }

        private bool IsCommandNameMatch(string commandName, Type type)
        {
            if (type.IsMatch(commandName))
                return true;

            string[] possibleTypeNames = commandName.AsArray().Concat(commandTypeNameEndings.Select(x => $"{commandName}{x}")).ToArray();

            return possibleTypeNames.Any(x => string.Equals(x, type.Name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
