using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Common
{
    internal sealed class ArgsParser
    {
        static readonly string[] valueSplitters = { "=" };

        public Task<Note[]> Parse(params string[] args)
        {
            return
                args
                ?.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => ParseArgument(x))
                .ToArray()
                .AsTask()
                ??
                new Note[0].AsTask()
                ;
        }

        private Note ParseArgument(string arg)
        {
            string[] parts = arg.Split(valueSplitters, 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return string.Empty.NoteAs(parts[0].Trim());

            return parts[1].NoteAs(parts[0].Trim());
        }
    }
}
