using System.Linq;

namespace H.Necessaire.Runtime.ExternalCommandRunner
{
    internal sealed class ArgsBuilder
    {
        public string[] Build(params Note[] args)
        {
            return
                args
                ?.Where(a => !a.IsEmpty())
                .Select(a => BuildArgument(a).NullIfEmpty())
                .ToNoNullsArray()
                ;
        }

        public string BuildInline(params Note[] args)
        {
            string[] parts = Build(args);
            if (parts?.Any() != true)
                return null;

            return string.Join(" ", parts);
        }

        private static string BuildArgument(Note arg)
        {
            if (arg.IsEmpty())
                return null;

            if (arg.Value.IsEmpty())
                return EscapeArgumentPartIfNecessary(arg.ID);

            if (arg.ID.IsEmpty())
                return EscapeArgumentPartIfNecessary(arg.Value);

            return $"{EscapeArgumentPartIfNecessary(arg.ID)}={EscapeArgumentPartIfNecessary(arg.Value)}";
        }

        private static string EscapeArgumentPartIfNecessary(string part)
        {
            if (part.IsEmpty())
                return part;

            if (!part.Any(c => char.IsWhiteSpace(c)))
                return part;

            return $"\"{part.Replace("\"", "\"\"")}\"";
        }
    }
}
