using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.UI
{
    public static partial class CliUiUserInputExtensions
    {
        static readonly IDictionary<bool, string> defaultConfirmationLabels = new Dictionary<bool, string> {
            { true, "y" },
            { false, "n" },
        };

        public static async Task<bool> CliUiAskForConfirmation(this string label, bool? defaultTo = null, IDictionary<bool, string> labels = null, CancellationToken? cancellationToken = null)
        {
            TextPrompt<bool> prompt
                = new TextPrompt<bool>(label)
                .AddChoice(true)
                .AddChoice(false)
                .WithConverter(x => labels?.ContainsKey(x) == true ? labels[x] : defaultConfirmationLabels[x])
                ;

            if (defaultTo != null)
                prompt = prompt.DefaultValue(defaultTo.Value).ShowDefaultValue();

            return await prompt.ShowAsync(AnsiConsole.Console, cancellationToken ?? CancellationToken.None);
        }

        public static async Task<OperationResult<T>> CliUiAskForAnyInput<T>
        (
            this string label,
            T defaultValue = default,
            bool isOptional = true,
            Func<T, OperationResult> validator = null,
            bool isSecret = false,
            char secretMask = '•',
            CancellationToken? cancellationToken = null
        )
        {
            var prompt = new TextPrompt<T>(label);

            if (validator != null)
            {
                prompt = prompt.Validate(x => Map(validator.Invoke(x)));
            }

            if (isOptional)
            {
                prompt = prompt.AllowEmpty().DefaultValue(defaultValue).HideDefaultValue();
            }

            if (isSecret)
            {
                prompt = prompt.Secret(secretMask);
            }

            OperationResult<T> result = OperationResult.Fail("Not yet started").WithoutPayload<T>();

            await new Func<Task>(async () =>
            {

                T userInput = await prompt.ShowAsync(AnsiConsole.Console, cancellationToken ?? CancellationToken.None);

                result = OperationResult.Win().WithPayload(userInput);

            })
            .TryOrFailWithGrace(
                onFail: ex =>
                {
                    result = OperationResult.Fail(ex, $"Error occurred while trying to get user input. Reason: {ex.Message}.").WithoutPayload<T>();
                }
            );

            return result;
        }

        public static async Task<OperationResult<T>> CliUiAskForChoiceInput<T>
        (
            this string label,
            IEnumerable<T> choices,
            IDictionary<T, string> labels = null,
            bool hasDefaultValue = false,
            T defaultValue = default,
            CancellationToken? cancellationToken = null
        )
        {
            if (choices.IsEmpty())
                return OperationResult.Fail("The given choices are empty").WithoutPayload<T>();

            var prompt = new TextPrompt<T>(label);

            prompt = prompt
                .AddChoices(choices)
                .WithConverter(x => GetDataLabel(x, labels))
                ;

            if (hasDefaultValue)
            {
                prompt = prompt.DefaultValue(defaultValue).ShowDefaultValue();
            }

            OperationResult<T> result = OperationResult.Fail("Not yet started").WithoutPayload<T>();

            await new Func<Task>(async () =>
            {

                T userInput = await prompt.ShowAsync(AnsiConsole.Console, cancellationToken ?? CancellationToken.None);

                result = OperationResult.Win().WithPayload(userInput);

            })
            .TryOrFailWithGrace(
                onFail: ex =>
                {
                    result = OperationResult.Fail(ex, $"Error occurred while trying to get user input. Reason: {ex.Message}.").WithoutPayload<T>();
                }
            );

            return result;
        }

        static string GetDataLabel<T>(T data, IDictionary<T, string> valueLabels)
        {
            if (valueLabels?.ContainsKey(data) == true)
                return valueLabels[data];

            string result = null;

            PropertyInfo labelProperty
                = typeof(T).GetProperty("DisplayName")
                ?? typeof(T).GetProperty("DisplayLabel")
                ?? typeof(T).GetProperty("Label")
                ?? typeof(T).GetProperty("IDTag")
                ?? typeof(T).GetProperty("Tag")
                ?? typeof(T).GetProperty("Name")
                ?? typeof(T).GetProperty("ID")
                ;

            if (labelProperty != null && labelProperty.CanRead)
            {
                new Action(() => result = labelProperty.GetValue(data)?.ToString()).TryOrFailWithGrace();
            }

            if (!result.IsEmpty())
                return result;

            return data.ToString();
        }

        static ValidationResult Map(OperationResult validationResult)
        {
            if (validationResult.IsSuccessful)
                return ValidationResult.Success();

            return ValidationResult.Error(validationResult.Reason);
        }
    }
}
