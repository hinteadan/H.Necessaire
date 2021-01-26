using System;
using System.Collections.Concurrent;
using System.Linq;

namespace H.Necessaire.Dapper
{
    public static class SqlEntryExtensions
    {
        static readonly ConcurrentDictionary<Type, string[]> knownColumnNamesPerType = new ConcurrentDictionary<Type, string[]>();

        public static string[] GetColumnNames(this Type sqlEntryType)
        {
            if (knownColumnNamesPerType.ContainsKey(sqlEntryType))
                return knownColumnNamesPerType[sqlEntryType];

            string[] columnNames
                = sqlEntryType
                .GetProperties()
                .Select(p => p.Name)
                .ToArray()
                ;

            knownColumnNamesPerType.AddOrUpdate(sqlEntryType, columnNames, (a, b) => columnNames);

            return columnNames;
        }
    }
}
