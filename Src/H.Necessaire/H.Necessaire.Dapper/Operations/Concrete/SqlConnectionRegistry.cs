using System;
using System.Collections.Concurrent;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    internal class SqlConnectionRegistry : ImASqlEntityConnectionRegistry, ImADependency
    {
        #region Construct
        readonly SqlEntityConnection defaultSqlEntityConnection = new SqlEntityConnection();
        readonly ConcurrentDictionary<Type, SqlEntityConnection> entityConnectionsDictionary = new ConcurrentDictionary<Type, SqlEntityConnection>();
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            RuntimeConfig runtimeConfig = dependencyProvider.GetRuntimeConfig();
            defaultSqlEntityConnection.ConnectionString = runtimeConfig?.Get("SqlConnections")?.Get("DefaultConnectionString")?.ToString();
        }
        #endregion

        public SqlEntityConnection GetConnectionStringByType(Type type)
        {
            if (!entityConnectionsDictionary.ContainsKey(type))
                return defaultSqlEntityConnection;

            return entityConnectionsDictionary[type] ?? defaultSqlEntityConnection;
        }

        public void RegisterConnectionForType(Type type, SqlEntityConnection entityConnection)
        {
            entityConnection.Type = type;
            entityConnectionsDictionary.AddOrUpdate(type, entityConnection, (x, y) => entityConnection);
        }

        public SqlEntityConnection GetConnectionString<T>() => GetConnectionStringByType(typeof(T));

        public void RegisterConnection<T>(SqlEntityConnection entityConnection) => RegisterConnectionForType(typeof(T), entityConnection);
    }
}
