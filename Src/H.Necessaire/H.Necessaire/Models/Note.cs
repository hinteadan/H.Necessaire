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

        public static Note[] FromDictionary(IDictionary<string, string> keyValuePairs)
        {
            if (!keyValuePairs?.Any() ?? true)
                return new Note[0];

            return keyValuePairs.Select(x => new Note(x.Key, x.Value)).ToArray();
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
    }
}
