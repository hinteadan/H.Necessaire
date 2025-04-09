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
        readonly List<Note> connectionStringParts = new List<Note>();
        public ConnectionStringBuilder(string connectionString)
        {
            connectionStringParts.AddRange(ParseConnectionString(connectionString));
        }
        public ConnectionStringBuilder() : this(null) { }
        #endregion

        public override string ToString()
        {
            return string.Join(partsSeparator, connectionStringParts.Select(ConnectionStringPartToString));
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

            return (kv.Length > 1 ? kv[1] : null).NoteAs(kv[0]);
        }
    }
}
