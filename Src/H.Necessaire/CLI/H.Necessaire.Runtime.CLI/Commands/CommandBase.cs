using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Commands
{
    public abstract class CommandBase : UseCaseBase, ImACliCommand
    {
        Func<string, ImACliSubCommand> globalSubCommandFinder;
        Func<string, IEnumerable<Type>, ImACliSubCommand> concreteTypesSubCommandFinder;
        CustomCommandRunner customCommandRunner;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            globalSubCommandFinder = globalSubCommandFinder ?? (name => dependencyProvider.Build<ImACliSubCommand>(name));
            concreteTypesSubCommandFinder = concreteTypesSubCommandFinder ?? ((name, types) => dependencyProvider.Build<ImACliSubCommand>(name, types));
            customCommandRunner = dependencyProvider.Get<CustomCommandRunner>();
        }

        protected async Task<Note[]> GetArguments()
        {
            return (await GetCurrentContext() ?? new UseCaseContext()).Notes;
        }

        public abstract Task<OperationResult> Run();

        protected async Task<OperationResult> RunCliCommand(params Note[] args)
            => await customCommandRunner.RunCliCommand(args);

        protected async Task<OperationResult> RunCliCommand(params string[] args)
            => await customCommandRunner.RunCliCommand(args);

        protected virtual async Task<OperationResult> RunSubCommand(bool failOnMissingCommand = false)
        {
            Note[] args = (await GetArguments())?.Jump(1);

            if (args?.Any() != true)
            {
                return failOnMissingCommand ? OperationResult.Fail("No Args") : WinWithUsageSyntax();
            }

            ImACliSubCommand subCommand = FindSubCommand(args.First().ID);

            if (subCommand is null)
            {
                return failOnMissingCommand ? OperationResult.Fail("No Matching Sub-Command") : WinWithUsageSyntax();
            }

            return await subCommand.Run(args.Jump(1));
        }

        protected virtual ImACliSubCommand FindSubCommand(string identifier)
        {
            if (identifier.IsEmpty())
                return null;

            Type commandType = GetType();

            Type[] matchingTypesInCurrentAssembly
                = typeof(ImACliSubCommand)
                .FindMatchingConcreteTypes(identifier, commandType.Assembly)
                .ToArray()
                ;

            Type[] matchingNestedTypes
                = matchingTypesInCurrentAssembly
                .Where(x => x.IsNestedUnder(commandType))
                .ToArray()
                ;

            if (matchingNestedTypes.Any())
                return concreteTypesSubCommandFinder.Invoke(identifier, matchingNestedTypes);

            if (matchingTypesInCurrentAssembly.Any())
                return concreteTypesSubCommandFinder.Invoke(identifier, matchingTypesInCurrentAssembly);

            return globalSubCommandFinder.Invoke(identifier);
        }

        protected virtual string[] GetUsageSyntaxes() => new string[0];

        protected virtual string PrintUsageSyntax()
        {
            return CLIPrinter.PrintUsageSyntax(GetUsageSyntaxes());
        }

        protected virtual OperationResult FailWithUsageSyntax()
        {
            return OperationResult.Fail(PrintUsageSyntax());
        }

        protected virtual OperationResult WinWithUsageSyntax(string reason = null)
        {
            Log($"{reason}{Environment.NewLine}{Environment.NewLine}{PrintUsageSyntax()}");
            return OperationResult.Win();
        }

        protected virtual string Log(string message)
        {
            return CLIPrinter.PrintLog(message);
        }
    }
}
