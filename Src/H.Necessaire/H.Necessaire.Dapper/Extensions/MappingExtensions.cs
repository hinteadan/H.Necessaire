using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.Dapper
{
    public static class MappingExtensions
    {
        static readonly ConcurrentDictionary<Type, object> mappers = new ConcurrentDictionary<Type, object>();

        public static void RegisterMapper<TEntity, TSqlEntity>(this SqlEntityMapperBase<TEntity, TSqlEntity> mapper) where TEntity : new() where TSqlEntity : new()
        {
            mappers.AddOrUpdate(typeof(SqlEntityMapperBase<TEntity, TSqlEntity>), mapper, (a, b) => mapper);
        }

        public static TSqlEntity ToSqlEntity<TEntity, TSqlEntity>(this TEntity entity, out TSqlEntity result) where TEntity : new() where TSqlEntity : new()
        {
            result = GetMapper<TEntity, TSqlEntity>().MapEntityToSql(entity);
            return result;
        }
        public static TEntity ToEntity<TEntity, TSqlEntity>(this TSqlEntity sqlEntity, out TEntity result) where TEntity : new() where TSqlEntity : new()
        {
            result = GetMapper<TEntity, TSqlEntity>().MapSqlToEntity(sqlEntity);
            return result;
        }

        public static Assembly InitializeHNecessaireDapperMappers(this Assembly assemblyToScanForMappers)
        {
            if (assemblyToScanForMappers == null)
                throw new ArgumentNullException(nameof(assemblyToScanForMappers), "The given assembly is null");

            Type[] mapperTypes
                = assemblyToScanForMappers
                .GetTypes()
                .Where(type => type.GetInterfaces().Any(t => t == typeof(ISqlEntityMapper)) && type.IsClass && !type.IsAbstract)
                .ToArray();

            foreach (Type mapperType in mapperTypes)
            {
                mapperType.TypeInitializer.Invoke(null, null);
            }

            return assemblyToScanForMappers;
        }

        private static SqlEntityMapperBase<TEntity, TSqlEntity> GetMapper<TEntity, TSqlEntity>() where TEntity : new() where TSqlEntity : new()
        {
            if (!mappers.ContainsKey(typeof(SqlEntityMapperBase<TEntity, TSqlEntity>)))
                throw new InvalidOperationException($"No mapper exists for: {typeof(SqlEntityMapperBase<TEntity, TSqlEntity>)}");

            return (SqlEntityMapperBase<TEntity, TSqlEntity>)mappers[typeof(SqlEntityMapperBase<TEntity, TSqlEntity>)];

        }
    }
}
