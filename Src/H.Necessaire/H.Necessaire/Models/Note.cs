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

                    $"Environment-{Environment.MachineName}".NoteAs(nameof(Environment.MachineName)),
                    $"Environment-{Environment.ProcessorCount}".NoteAs(nameof(Environment.ProcessorCount)),
                    $"Environment-{Environment.UserDomainName}".NoteAs(nameof(Environment.UserDomainName)),
                    $"Environment-{Environment.UserName}".NoteAs(nameof(Environment.UserName)),
                    $"Environment-{Environment.UserInteractive}".NoteAs(nameof(Environment.UserInteractive)),

                    $"Environment-{Environment.OSVersion}".NoteAs(nameof(Environment.OSVersion)),
                    $"Environment-{Environment.Version}".NoteAs(nameof(Environment.Version)),

                    $"Environment-{Environment.Is64BitOperatingSystem}".NoteAs(nameof(Environment.Is64BitOperatingSystem)),
                    $"Environment-{Environment.Is64BitProcess}".NoteAs(nameof(Environment.Is64BitProcess)),

                    $"Environment-{Environment.CommandLine}".NoteAs(nameof(Environment.CommandLine)),
                    $"Environment-{Environment.CurrentDirectory}".NoteAs(nameof(Environment.CurrentDirectory)),
                    $"Environment-{Environment.SystemDirectory}".NoteAs(nameof(Environment.SystemDirectory)),
                    $"Environment-{Environment.NewLine}".NoteAs(nameof(Environment.NewLine)),

                    $"Environment-{Environment.SystemPageSize}".NoteAs(nameof(Environment.SystemPageSize)),
                    $"Environment-{Environment.TickCount}".NoteAs(nameof(Environment.TickCount)),
                    $"Environment-{Environment.WorkingSet}".NoteAs(nameof(Environment.WorkingSet)),

                    $"Environment-{Environment.CurrentManagedThreadId}".NoteAs(nameof(Environment.CurrentManagedThreadId)),

                };
        }
    }
}
