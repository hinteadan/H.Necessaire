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

        public static TSqlEntity ToSqlEntity<TEntity, TSqlEntity>(this TEntity entity) where TEntity : new() where TSqlEntity : ISqlEntry, new()
        {
            return GetMapper<TEntity, TSqlEntity>().MapEntityToSql(entity);
        }
        public static TEntity ToEntity<TEntity, TSqlEntity>(this TSqlEntity sqlEntity) where TEntity : new() where TSqlEntity : ISqlEntry, new()
        {
            return GetMapper<TEntity, TSqlEntity>().MapSqlToEntity(sqlEntity);
        }

        public static void InitializeHNecessaireDapperMappers()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Type sqlMapperType = typeof(ISqlEntityMapper);

            Type[] mapperTypes
                = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(type => type != null)
                .Where(type => sqlMapperType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .ToArray();

            foreach (Type mapperType in mapperTypes)
            {
                mapperType.TypeInitializer.Invoke(null, null);
            }
        }

        private static SqlEntityMapperBase<TEntity, TSqlEntity> GetMapper<TEntity, TSqlEntity>() where TEntity : new() where TSqlEntity : new()
        {
            if (!mappers.ContainsKey(typeof(SqlEntityMapperBase<TEntity, TSqlEntity>)))
                throw new InvalidOperationException($"No mapper exists for: {typeof(SqlEntityMapperBase<TEntity, TSqlEntity>)}");

            return (SqlEntityMapperBase<TEntity, TSqlEntity>)mappers[typeof(SqlEntityMapperBase<TEntity, TSqlEntity>)];
        }
    }
}
