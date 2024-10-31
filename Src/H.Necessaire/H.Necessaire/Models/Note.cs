﻿using System;
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

                    $"{Environment.MachineName}".NoteAs($"Environment-MachineName"),
                    $"{Environment.ProcessorCount}".NoteAs($"Environment-ProcessorCount"),
                    $"{Environment.UserDomainName}".NoteAs($"Environment-UserDomainName"),
                    $"{Environment.UserName}".NoteAs($"Environment-UserName"),
                    $"{Environment.UserInteractive}".NoteAs($"Environment-UserInteractive"),

                    $"{Environment.OSVersion}".NoteAs($"Environment-OSVersion"),
                    $"{Environment.Version}".NoteAs($"Environment-Version"),

                    $"{Environment.Is64BitOperatingSystem}".NoteAs($"Environment-Is64BitOperatingSystem"),
                    $"{Environment.Is64BitProcess}".NoteAs($"Environment-Is64BitProcess"),

                    $"{Environment.CommandLine}".NoteAs($"Environment-CommandLine"),
                    $"{Environment.CurrentDirectory}".NoteAs($"Environment-CurrentDirectory"),
                    $"{Environment.SystemDirectory}".NoteAs($"Environment-SystemDirectory"),
                    $"{Environment.NewLine}".NoteAs($"Environment-NewLine"),

                    $"{Environment.SystemPageSize}".NoteAs($"Environment-SystemPageSize"),
                    $"{Environment.TickCount}".NoteAs($"Environment-TickCount"),
                    $"{Environment.WorkingSet}".NoteAs($"Environment-WorkingSet"),

                    $"{Environment.CurrentManagedThreadId}".NoteAs($"Environment-CurrentManagedThreadId"),

                };
        }
    }
}
