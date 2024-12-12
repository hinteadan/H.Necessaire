using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.CommandInterpreter
{
    internal class CliConsoleInteractor
    {
        CliCommandHelpInfo[] currentCommandSuggestions = null;
        int currentCommandSuggestionIndex = -1;

        public async Task<string> ProcessConsoleUserInput(CancellationToken cancellationToken)
        {
            string searchKey = null;
            StringBuilder userInputStream = new StringBuilder();

            while (!cancellationToken.IsCancellationRequested)
            {
                ConsoleKeyInfo latestKey = Console.ReadKey(intercept: true);

                if (latestKey.Key == ConsoleKey.Enter)
                {
                    searchKey = null;
                    ResetCurrentSuggestions();
                    await Console.Out.WriteLineAsync();
                    break;
                }

                if (latestKey.Key == ConsoleKey.Backspace)
                {
                    if (userInputStream.Length == 0)
                        continue;

                    searchKey = null;
                    ResetCurrentSuggestions();
                    userInputStream.Remove(userInputStream.Length - 1, 1);
                    await DeleteCharsFromConsole(1);
                    continue;
                }

                if (latestKey.Key == ConsoleKey.Tab)
                {
                    searchKey = searchKey ?? userInputStream.ToString();
                    OperationResult<string> suggestion = await FindNextSuggestionFor(searchKey, isBackwardsSearch: latestKey.Modifiers == ConsoleModifiers.Shift);
                    if (!suggestion.IsSuccessful)
                        continue;

                    await DeleteCharsFromConsole(userInputStream.Length);

                    userInputStream.Clear().Append(suggestion.Payload);

                    await Console.Out.WriteAsync(suggestion.Payload);

                    continue;
                }

                await Console.Out.WriteAsync(latestKey.KeyChar);
                userInputStream.Append(latestKey.KeyChar);
            }

            return userInputStream.ToString();
        }

        private async Task DeleteCharsFromConsole(int length)
        {
            if (length <= 0)
                return;

            for (int i = 0; i < length; i++)
            {
                await Console.Out.WriteAsync("\b \b");
            }
        }

        private Task<OperationResult<string>> FindNextSuggestionFor(string searchKey, bool isBackwardsSearch = false)
        {
            if (currentCommandSuggestions is null)
            {
                currentCommandSuggestions = CliCommandsIndexer.FindCliCommands(searchKey).NullIfEmpty();
                if (currentCommandSuggestions?.Any() != true)
                    return OperationResult.Fail("No suggestions").WithoutPayload<string>().AsTask();
            }

            currentCommandSuggestionIndex = !isBackwardsSearch ? currentCommandSuggestionIndex + 1 : currentCommandSuggestionIndex - 1;
            if (currentCommandSuggestionIndex < 0)
                currentCommandSuggestionIndex = currentCommandSuggestions.Length - 1;
            if (currentCommandSuggestionIndex > currentCommandSuggestions.Length - 1)
                currentCommandSuggestionIndex = 0;

            CliCommandHelpInfo suggestion = currentCommandSuggestions[currentCommandSuggestionIndex];

            string command = suggestion.ID ?? suggestion.Name;

            return $"{command}".ToWinResult().AsTask();
        }

        private void ResetCurrentSuggestions()
        {
            currentCommandSuggestions = null;
            currentCommandSuggestionIndex = -1;
        }
    }
}
