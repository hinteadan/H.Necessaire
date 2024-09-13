using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    public struct Note
    {
        public const string IDSeparator = "=::=";
        public static readonly Note Empty = new Note();

        public Note(string id, string value)
        {
            this.ID = id;
            this.Value = value;
        }

        public string ID { get; set; }
        public string Value { get; set; }

        public bool IsEmpty() => string.IsNullOrWhiteSpace(ID) && string.IsNullOrWhiteSpace(Value);

        public override string ToString()
        {
            return $"{ID} {IDSeparator} {Value}";
        }

        public static implicit operator Note(string noteAsString)
        {
            if (noteAsString.IsEmpty())
                return Empty;

            string[] parts = noteAsString.Split(IDSeparator.AsArray(), 2, StringSplitOptions.RemoveEmptyEntries);

            return new Note(parts.Length > 1 ? parts[0]?.Trim().NullIfEmpty() : null, parts[parts.Length - 1]?.Trim().NullIfEmpty());
        }

        public static implicit operator string(Note note)
        {
            if (note.IsEmpty())
                return null;

            return note.ToString();
        }

        public static Note[] FromDictionary(IDictionary<string, string> keyValuePairs)
        {
            if (!keyValuePairs?.Any() ?? true)
                return new Note[0];

            return keyValuePairs.Select(x => new Note(x.Key, x.Value)).ToArray();
        }

        public static Note[] GetEnvironmentInfo()
        {
            return
                new Note[] {

                    $"{nameof(Environment.MachineName)}{IDSeparator}{Environment.MachineName}",
                    $"{nameof(Environment.ProcessorCount)}{IDSeparator}{Environment.ProcessorCount}",
                    $"{nameof(Environment.UserDomainName)}{IDSeparator}{Environment.UserDomainName}",
                    $"{nameof(Environment.UserName)}{IDSeparator}{Environment.UserName}",
                    $"{nameof(Environment.UserInteractive)}{IDSeparator}{Environment.UserInteractive}",

                    $"{nameof(Environment.OSVersion)}{IDSeparator}{Environment.OSVersion}",

                    $"{nameof(Environment.Is64BitOperatingSystem)}{IDSeparator}{Environment.Is64BitOperatingSystem}",
                    $"{nameof(Environment.Is64BitProcess)}{IDSeparator}{Environment.Is64BitProcess}",

                    $"{nameof(Environment.Version)}{IDSeparator}{Environment.Version}",

                    $"{nameof(Environment.CommandLine)}{IDSeparator}{Environment.CommandLine}",
                    $"{nameof(Environment.CurrentDirectory)}{IDSeparator}{Environment.CurrentDirectory}",
                    $"{nameof(Environment.SystemDirectory)}{IDSeparator}{Environment.SystemDirectory}",
                    $"{nameof(Environment.NewLine)}{IDSeparator}{Environment.NewLine}",

                    $"{nameof(Environment.SystemPageSize)}{IDSeparator}{Environment.SystemPageSize}",
                    $"{nameof(Environment.TickCount)}{IDSeparator}{Environment.TickCount}",
                    $"{nameof(Environment.WorkingSet)}{IDSeparator}{Environment.WorkingSet}",

                    $"{nameof(Environment.CurrentManagedThreadId)}{IDSeparator}{Environment.CurrentManagedThreadId}",

                };
        }
    }
}
