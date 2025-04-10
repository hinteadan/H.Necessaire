using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Dapper
{
    internal class ConnectionStringBuilder
    {
        #region Construct
        const string partsSeparator = ";";
        const string kvSeparator = "=";
        readonly Dictionary<string, Note> connectionStringParts = new Dictionary<string, Note>();
        public ConnectionStringBuilder(string connectionString)
        {
            foreach (Note part in ParseConnectionString(connectionString))
            {
                connectionStringParts.Add(part.ID.ToLowerInvariant(), part);
            }
        }
        public ConnectionStringBuilder() : this(null) { }
        #endregion

        public ConnectionStringBuilder Set(Note part)
        {
            if (part.ID.IsEmpty())
                return this;

            string key = part.ID.ToLowerInvariant();

            if (connectionStringParts.ContainsKey(key))
            {
                connectionStringParts[key] = part;
                return this;
            }

            connectionStringParts.Add(key, part);

            return this;
        }
        public ConnectionStringBuilder Set(string key, string value) => Set(new Note(key, value));
        public ConnectionStringBuilder Set(string flag) => Set(flag, null);

        public ConnectionStringBuilder Zap(string key)
        {
            connectionStringParts.Remove(key.ToLowerInvariant());

            return this;
        }

        public Note? Get(string key)
        {
            if (connectionStringParts.TryGetValue(key.ToLowerInvariant(), out Note result))
                return result;

            return null;
        }

        public Note? GetFirst(params string[] keys)
        {
            if (keys.IsEmpty())
                return null;

            foreach (string key in keys)
            {
                if (connectionStringParts.TryGetValue(key.ToLowerInvariant(), out Note result))
                    return result;
            }

            return null;
        }

        public override string ToString()
        {
            return string.Join(partsSeparator, connectionStringParts.Select(x => ConnectionStringPartToString(x.Value)));
        }

        static string ConnectionStringPartToString(Note connectionStringPart)
        {
            if (connectionStringPart.IsEmpty())
                return null;

            if (connectionStringPart.ID.IsEmpty() || connectionStringPart.Value.IsEmpty())
                return connectionStringPart.Value.IsEmpty() ? connectionStringPart.ID.Trim() : connectionStringPart.Value.Trim();

            return $"{connectionStringPart.ID.Trim()}{kvSeparator}{connectionStringPart.Value.Trim()}";
        }

        static Note[] ParseConnectionString(string connectionString)
        {
            if (connectionString.IsEmpty())
                return Array.Empty<Note>();

            string[] parts = connectionString.Split(partsSeparator.AsArray(), StringSplitOptions.RemoveEmptyEntries);

            return
                parts
                .Select(ParseConnectionStringPart)
                .Where(x => x?.IsEmpty() == false)
                .Select(x => x.Value)
                .ToArray()
                ;
        }

        static Note? ParseConnectionStringPart(string part)
        {
            if (part.IsEmpty())
                return null;

            string[] kv = part.Split(kvSeparator.AsArray(), 2, StringSplitOptions.RemoveEmptyEntries);

            return (kv.Length > 1 ? kv[1].Trim() : null).NoteAs(kv[0].Trim());
        }
    }
}
