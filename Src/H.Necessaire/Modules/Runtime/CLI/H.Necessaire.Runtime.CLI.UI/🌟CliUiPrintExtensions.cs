using Spectre.Console;
using Spectre.Console.Json;
using System;

namespace H.Necessaire.Runtime.CLI.UI
{
    public static class CliUiPrintExtensions
    {
        public static void CliUiPrint(this Exception exception)
        {
            if (exception is null)
                return;

            Exception[] exceptions = exception.Flatten();

            if (exceptions.IsEmpty())
                return;

            foreach (Exception ex in exceptions)
            {
                AnsiConsole.WriteException(ex);
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine();
            }
        }


        /// <summary>
        /// Markup as of https://spectreconsole.net/markup
        /// </summary>
        /// <param name="markupString">markup string to print</param>
        public static void CliUiPrintMarkup(this string markupString)
        {
            if (markupString.IsEmpty())
                return;

            AnsiConsole.Markup(markupString);
        }

        /// <summary>
        /// Markup as of https://spectreconsole.net/markup
        /// </summary>
        /// <param name="markupString">markup string to print</param>
        public static void CliUiPrintMarkupLine(this string markupString)
        {
            if (markupString.IsEmpty())
                return;

            AnsiConsole.MarkupLine(markupString);
        }

        /// <summary>
        /// Markup as of https://spectreconsole.net/markup
        /// </summary>
        public static string CliUiEscapeForMarkup(this string stringToEscape)
        {
            if (stringToEscape.IsEmpty())
                return stringToEscape;

            return stringToEscape.EscapeMarkup();
        }

        public static OperationResult<Exception> CliUiPrintJson(this string jsonString, string title = null)
        {
            if (jsonString.IsEmpty())
                return OperationResult.Win().WithoutPayload<Exception>();

            OperationResult<Exception> result = OperationResult.Fail("Not yet started").WithoutPayload<Exception>();

            new Action(() =>
            {
                JsonText json = new JsonText(jsonString);

                if (title.IsEmpty())
                {
                    AnsiConsole.Write(json);
                }
                else
                {
                    AnsiConsole.Write(json.InPanel(title));
                }

                AnsiConsole.WriteLine();

                result = OperationResult.Win().WithoutPayload<Exception>();
            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to print the json. Reason: {ex.Message}.").WithPayload(ex);
            });

            return result;
        }

        public static void CliUiPrintPath(this string pathString, bool isInline = false)
        {
            TextPath path
                = new TextPath(pathString)
                .RootColor(Color.Red)
                .SeparatorColor(Color.Green)
                .StemColor(Color.Blue)
                .LeafColor(Color.Yellow);
            ;

            AnsiConsole.Write(path);

            if (!isInline)
                AnsiConsole.WriteLine();
        }
    }
}
