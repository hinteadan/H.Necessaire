using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.Dapper
{
    public static class SqlEntryExtensions
    {
        static readonly ConcurrentDictionary<Type, string[]> knownColumnNamesPerType = new ConcurrentDictionary<Type, string[]>();
        static readonly ConcurrentDictionary<Type, PropertyInfo[]> knownColumnPropertiesPerType = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static string[] GetColumnNames(this Type sqlEntryType)
        {
            if (knownColumnNamesPerType.ContainsKey(sqlEntryType))
                return knownColumnNamesPerType[sqlEntryType];

            string[] columnNames
                = sqlEntryType
                .GetPropertiesThatCountAsColumns()
                .Select(p => p.Name)
                .ToArray();

            knownColumnNamesPerType.AddOrUpdate(sqlEntryType, columnNames, (a, b) => columnNames);

            return columnNames;
        }

        public static PropertyInfo[] GetPropertiesThatCountAsColumns(this Type sqlEntryType)
        {
            if (knownColumnPropertiesPerType.ContainsKey(sqlEntryType))
                return knownColumnPropertiesPerType[sqlEntryType];

            PropertyInfo[] result
                = sqlEntryType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            knownColumnPropertiesPerType.AddOrUpdate(sqlEntryType, result, (a, b) => result);

            return result;
        }
    }
}
