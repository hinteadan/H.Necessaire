using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.CLI.CommandInterpreter
{
    internal static class CliCommandHelpInfoConsolePrintExtensions
    {
        public static CliCommandHelpInfo PrintToConsole(this CliCommandHelpInfo commandHelpInfo)
        {
            if (commandHelpInfo == null)
            {
                return null;
            }

            using (new ScopedRunner(() => Console.ForegroundColor = ConsoleColor.Green, Console.ResetColor))
            {
                Console.WriteLine($"{commandHelpInfo.Name}");
            }

            string preferredSyntax = commandHelpInfo.GetPreferredCommandSyntax();
            string[] otherSyntaxes = commandHelpInfo.GetAllCommandSyntaxes()?.Where(x => !x.Is(preferredSyntax) && !x.Is(commandHelpInfo.Name)).ToArrayNullIfEmpty();

            if (!preferredSyntax.Is(commandHelpInfo.Name))
            {
                using (new ScopedRunner(() => Console.ForegroundColor = ConsoleColor.Yellow, Console.ResetColor))
                {
                    Console.WriteLine(preferredSyntax);
                }
            }

            if (otherSyntaxes?.Any() == true)
            {
                using (new ScopedRunner(() => Console.ForegroundColor = ConsoleColor.DarkYellow, Console.ResetColor))
                {
                    Console.WriteLine(string.Join(" | ", otherSyntaxes));
                }
            }

            string[] usageSyntaxes = commandHelpInfo.UsageSyntaxes?.Where(x => !x.IsEmpty()).ToArrayNullIfEmpty();
            if (usageSyntaxes?.Any() == true)
            {
                Console.WriteLine();

                using (new ScopedRunner(() => Console.ForegroundColor = ConsoleColor.Cyan, Console.ResetColor))
                {
                    Console.WriteLine("Usage Syntax:");
                    Console.WriteLine("=============");
                }

                using (new ScopedRunner(() => Console.ForegroundColor = ConsoleColor.Gray, Console.ResetColor))
                {
                    foreach (string usageSyntax in usageSyntaxes)
                    {
                        Console.WriteLine(usageSyntax);
                    }
                }
            }

            return commandHelpInfo;
        }

        public static TCliCommandHelpInfoCollection PrintToConsole<TCliCommandHelpInfoCollection>(this TCliCommandHelpInfoCollection commandHelpInfos)
            where TCliCommandHelpInfoCollection : IEnumerable<CliCommandHelpInfo>
        {
            if (commandHelpInfos?.Any() != true)
                return commandHelpInfos;

            foreach (CliCommandHelpInfo commandHelpInfo in commandHelpInfos)
            {
                commandHelpInfo.PrintToConsole();
                using (new ScopedRunner(() => Console.ForegroundColor = ConsoleColor.DarkBlue, Console.ResetColor))
                {
                    Console.WriteLine("~~~~~~~~~~~~");
                    Console.WriteLine();
                }
            }

            return commandHelpInfos;
        }
    }
}
