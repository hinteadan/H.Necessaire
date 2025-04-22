using H.Necessaire.CLI.Commands;
using System;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.Runtime.CLI.CommandInterpreter
{
    internal class CliCommandsIndexer
    {
        static readonly string[] commandTypeNameEndings = new string[] { "UseCase", "Command", "CliCommand", "CommandUseCase", "UseCaseCommand", "CliCommandUseCase", "CliUseCaseCommand" };
        static readonly Lazy<CliCommandHelpInfo[]> allKnownCliCommands = new Lazy<CliCommandHelpInfo[]>(IndexAllKnownCliCommands);

        public static CliCommandHelpInfo[] AllKnownCliCommands => allKnownCliCommands.Value;

        public static CliCommandHelpInfo[] FindCliCommands(string searchKey)
        {
            if (searchKey.IsEmpty())
                return AllKnownCliCommands;

            CliCommandHelpInfo[] commandsStartingWithSearchKey
                = AllKnownCliCommands
                .Where(x => x.GetPreferredCommandSyntax().StartsWith(searchKey, StringComparison.InvariantCultureIgnoreCase))
                .Union(
                    AllKnownCliCommands
                    .Where(x => x.GetAllCommandSyntaxes()?.Any(s => s.StartsWith(searchKey, StringComparison.InvariantCultureIgnoreCase)) == true)
                )
                .ToArray()
                ;

            if (commandsStartingWithSearchKey.Any())
                return commandsStartingWithSearchKey;

            CliCommandHelpInfo[] commandsContainingSearchKey
                = AllKnownCliCommands
                .Where(x => x.GetPreferredCommandSyntax().IndexOf(searchKey, StringComparison.InvariantCultureIgnoreCase) >= 0)
                .Union(
                    AllKnownCliCommands
                    .Where(x => x.GetAllCommandSyntaxes()?.Any(s => s.IndexOf(searchKey, StringComparison.InvariantCultureIgnoreCase) >= 0) == true)
                )
                .ToArray()
                ;

            return commandsContainingSearchKey;
        }

        static CliCommandHelpInfo[] IndexAllKnownCliCommands()
        {
            Type[] allKnownCommands = typeof(ImACliCommand).GetAllImplementations();

            if (allKnownCommands?.Any() != true)
                return Array.Empty<CliCommandHelpInfo>();

            return
                allKnownCommands
                .Select(Map)
                .ToArray()
                ;
        }

        static CliCommandHelpInfo Map(Type cliCommandConcreteType, int index)
        {
            return
                new CliCommandHelpInfo
                {
                    ConcreteType = cliCommandConcreteType,
                    Name = MapCommandName(cliCommandConcreteType),
                    ID = cliCommandConcreteType.GetID(),
                    Aliases = cliCommandConcreteType.GetAliases(),
                    Categories = cliCommandConcreteType.GetCategories(),
                    UsageSyntaxes = BuildUsageSyntaxes(cliCommandConcreteType),
                };
        }

        static string MapCommandName(Type cliCommandConcreteType)
        {
            string commandName = cliCommandConcreteType.Name;

            foreach (string ending in commandTypeNameEndings)
            {
                if (!commandName.EndsWith(ending, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                commandName = commandName.Substring(0, commandName.Length - ending.Length);

                break;
            }

            return commandName.ToLowerInvariant();
        }

        static string[] BuildUsageSyntaxes(Type cliCommandConcreteType)
        {
            string[] result = null;

            new Action(() =>
            {

                object commandInstance = Activator.CreateInstance(cliCommandConcreteType);
                var method = cliCommandConcreteType.GetRuntimeMethods().Single(x => x.Name == "GetUsageSyntaxes");
                result = method?.Invoke(commandInstance, null) as string[];
                result = result?.Where(x => !x.IsEmpty()).ToArrayNullIfEmpty();

            }).TryOrFailWithGrace();

            return result;
        }
    }
}
