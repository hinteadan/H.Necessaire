using System;

namespace H.Necessaire.Runtime.CLI
{
    public static class CLIPrinter
    {
        public static string PrintUsageSyntax(params string[] usageSyntaxes)
        {
            return
                $"Usage Syntax:{Environment.NewLine}============={Environment.NewLine}{Environment.NewLine}{string.Join($"{Environment.NewLine}{Environment.NewLine}", usageSyntaxes ?? new string[0])}";
        }

        public static string PrintLog(string message, bool printToConsoleAsWell = true)
        {
            string result = $"{DateTime.Now} | {message}";

            if (printToConsoleAsWell)
                Console.WriteLine(result);

            return result;
        }
    }
}
