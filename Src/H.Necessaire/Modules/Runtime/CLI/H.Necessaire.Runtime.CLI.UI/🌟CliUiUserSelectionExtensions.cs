using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.UI
{
    public static partial class CliUiUserInputExtensions
    {
        public static async Task<OperationResult<T>> CliUiAskForSingleSelection<T>
        (
            this string label,
            IEnumerable<T> choices,
            IDictionary<T, string> labels = null,
            int pageSize = 10,
            string moreChoicesLabel = "[grey](Up&Down for more...)[/]",
            bool isSearchEnabled = false,
            string searchPlaceholderLabel = null,
            CancellationToken? cancellationToken = null
        )
        {
            if (choices.IsEmpty())
                return OperationResult.Fail("The given choices are empty").WithoutPayload<T>();

            var prompt
                = new SelectionPrompt<T>()
                .Title(label)
                .AddChoices(choices)
                .UseConverter(x => GetDataLabel(x, labels))
                .PageSize(pageSize)
                ;

            if (!moreChoicesLabel.IsEmpty())
            {
                prompt = prompt.MoreChoicesText(moreChoicesLabel);
            }

            if (isSearchEnabled)
            {
                prompt = prompt.EnableSearch().SearchPlaceholderText(searchPlaceholderLabel);
            }

            OperationResult<T> result = OperationResult.Fail("Not yet started").WithoutPayload<T>();

            await new Func<Task>(async () =>
            {

                T userSelection = await prompt.ShowAsync(AnsiConsole.Console, cancellationToken ?? CancellationToken.None);

                result = OperationResult.Win().WithPayload(userSelection);

            })
            .TryOrFailWithGrace(
                onFail: ex =>
                {
                    result = OperationResult.Fail(ex, $"Error occurred while trying to get user selection. Reason: {ex.Message}.").WithoutPayload<T>();
                }
            );

            return result;
        }

        public static async Task<OperationResult<T[]>> CliUiAskForMultiSelection<T>
        (
            this string label,
            IEnumerable<T> choices,
            IDictionary<T, string> labels = null,
            bool canBeEmpty = true,
            int pageSize = 10,
            string moreChoicesLabel = "[grey](Up&Down for more...)[/]",
            string instructionsLabel = "[grey](Press [blue]<space>[/] to toggle a selection, [green]<enter>[/] to accept)[/]",
            CancellationToken? cancellationToken = null
        )
        {
            if (choices.IsEmpty())
                return OperationResult.Fail("The given choices are empty").WithoutPayload<T[]>();

            var prompt
                = new MultiSelectionPrompt<T>()
                .Title(label)
                .AddChoices(choices)
                .UseConverter(x => GetDataLabel(x, labels))
                .PageSize(pageSize)
                ;

            if (canBeEmpty)
            {
                prompt = prompt.NotRequired();
            }

            if (!moreChoicesLabel.IsEmpty())
            {
                prompt = prompt.MoreChoicesText(moreChoicesLabel);
            }

            if (!instructionsLabel.IsEmpty())
            {
                prompt = prompt.InstructionsText(instructionsLabel);
            }

            OperationResult<T[]> result = OperationResult.Fail("Not yet started").WithoutPayload<T[]>();

            await new Func<Task>(async () =>
            {

                List<T> userSelection = await prompt.ShowAsync(AnsiConsole.Console, cancellationToken ?? CancellationToken.None);

                result = OperationResult.Win().WithPayload(userSelection.ToArrayNullIfEmpty());

            })
            .TryOrFailWithGrace(
                onFail: ex =>
                {
                    result = OperationResult.Fail(ex, $"Error occurred while trying to get user selection. Reason: {ex.Message}.").WithoutPayload<T[]>();
                }
            );

            return result;
        }
    }
}
