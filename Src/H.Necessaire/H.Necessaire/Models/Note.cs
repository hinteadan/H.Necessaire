using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    public struct Note : IEquatable<Note>
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

        public bool IsEmpty() => ID.IsEmpty() && Value.IsEmpty();

        public bool IsSameAs(Note other)
        {
            return
                ID == other.ID
                && Value == other.Value
                ;
        }
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
        public static bool operator ==(Note a, Note b) => a.IsSameAs(b);
        public static bool operator !=(Note a, Note b) => !a.IsSameAs(b);

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

                    $"{DateTime.UtcNow.PrintTimeStampAsIdentifier()}".NoteAs($"Environment-AsOf"),
                    $"{DateTime.UtcNow.Ticks}".NoteAs($"Environment-AsOf-Ticks"),

                    0.SafeRead(x => Environment.MachineName).NoteAs($"Environment-MachineName"),
                    $"{Environment.ProcessorCount}".NoteAs($"Environment-ProcessorCount"),
                    0.SafeRead(x => Environment.UserDomainName).NoteAs($"Environment-UserDomainName"),
                    $"{Environment.UserName}".NoteAs($"Environment-UserName"),
                    $"{Environment.UserInteractive}".NoteAs($"Environment-UserInteractive"),

                    0.SafeRead(x => $"{Environment.OSVersion}").NoteAs($"Environment-OSVersion"),
                    $"{Environment.Version}".NoteAs($"Environment-Version"),

                    $"{Environment.Is64BitOperatingSystem}".NoteAs($"Environment-Is64BitOperatingSystem"),
                    $"{Environment.Is64BitProcess}".NoteAs($"Environment-Is64BitProcess"),

                    $"{Environment.CommandLine}".NoteAs($"Environment-CommandLine"),
                    0.SafeRead(x => Environment.CurrentDirectory).NoteAs($"Environment-CurrentDirectory"),
                    $"{Environment.SystemDirectory}".NoteAs($"Environment-SystemDirectory"),
                    $"{Environment.NewLine}".NoteAs($"Environment-NewLine"),

                    $"{Environment.SystemPageSize}".NoteAs($"Environment-SystemPageSize"),
                    $"{Environment.TickCount}".NoteAs($"Environment-TickCount"),
                    $"{Environment.WorkingSet}".NoteAs($"Environment-WorkingSet"),

                    $"{Environment.CurrentManagedThreadId}".NoteAs($"Environment-CurrentManagedThreadId"),

                };
        }

        public override bool Equals(object obj)
        {
            return obj is Note note && Equals(note);
        }

        public bool Equals(Note other)
        {
            return ID == other.ID &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            int hashCode = 429689802;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ID);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        public static implicit operator KeyValuePair<string, string>(Note note)
        {
            return new KeyValuePair<string, string>(note.ID, note.Value);
        }
        public static implicit operator Note(KeyValuePair<string, string> kvp)
        {
            return new Note(kvp.Key, kvp.Value);
        }
        public static implicit operator Note((string id, string value) parts)
        {
            return new Note(parts.id, parts.value);
        }
    }
}
