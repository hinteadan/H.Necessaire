using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.Dapper
{
    public abstract class SqlEntityMapperBase<TEntity, TSqlEntity> : ISqlEntityMapper where TEntity : new() where TSqlEntity : new()
    {
        #region Construct
        static readonly Dictionary<string, PropertyInfo> sqlEntityProperties;
        static readonly Dictionary<string, PropertyInfo> entityProperties;
        static SqlEntityMapperBase()
        {
            sqlEntityProperties = typeof(TSqlEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(x => x.Name, x => x);
            entityProperties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(x => x.Name, x => x);
        }
        #endregion

        public virtual TSqlEntity MapEntityToSql(TEntity entity)
        {
            TSqlEntity sqlEntity = new TSqlEntity();

            foreach (PropertyInfo property in entityProperties.Values)
            {
                if (!sqlEntityProperties.ContainsKey(property.Name))
                    continue;

                PropertyInfo sqlEntityProperty = sqlEntityProperties[property.Name];
                if (sqlEntityProperty.PropertyType != property.PropertyType)
                    continue;

                HSafe.Run(() => sqlEntityProperty.AndIf(x => x.CanWrite, x => x.SetValue(sqlEntity, property.GetValue(entity))));

                string ticksPropName = $"{property.Name}Ticks";
                if (sqlEntityProperties.ContainsKey(ticksPropName))
                {
                    if (property.PropertyType.In(typeof(DateTime), typeof(DateTime?)))
                        HSafe.Run(() => sqlEntityProperties[ticksPropName].AndIf(x => x.CanWrite, x => x.SetValue(sqlEntity, ((DateTime?)property.GetValue(entity))?.Ticks)));

                    else if (property.PropertyType.In(typeof(TimeSpan), typeof(TimeSpan?)))
                        HSafe.Run(() => sqlEntityProperties[ticksPropName].AndIf(x => x.CanWrite, x => x.SetValue(sqlEntity, ((TimeSpan?)property.GetValue(entity))?.Ticks)));
                }
            }

            return sqlEntity;
        }

        public virtual TEntity MapSqlToEntity(TSqlEntity sqlEntity)
        {
            TEntity entity = new TEntity();

            foreach (PropertyInfo property in sqlEntityProperties.Values)
            {
                if (property.Name.EndsWith("Ticks"))
                {
                    string actualPropertyName = property.Name.Substring(0, property.Name.Length - "Ticks".Length);
                    if (!entityProperties.ContainsKey(actualPropertyName))
                        continue;

                    long? ticks = (long?)property.GetValue(sqlEntity);

                    if (entityProperties[actualPropertyName].PropertyType == typeof(DateTime?))
                        HSafe.Run(() => sqlEntityProperties[actualPropertyName].AndIf(x => x.CanWrite, x => x.SetValue(sqlEntity, ticks == null ? null as DateTime? : new DateTime(ticks.Value, DateTimeKind.Utc))));
                    else if (entityProperties[actualPropertyName].PropertyType == typeof(DateTime))
                        HSafe.Run(() => sqlEntityProperties[actualPropertyName].AndIf(x => x.CanWrite, x => x.SetValue(sqlEntity, new DateTime(ticks.Value, DateTimeKind.Utc))));
                    else if (entityProperties[actualPropertyName].PropertyType == typeof(TimeSpan?))
                        HSafe.Run(() => sqlEntityProperties[actualPropertyName].AndIf(x => x.CanWrite, x => x.SetValue(sqlEntity, ticks == null ? null as TimeSpan? : new TimeSpan(ticks.Value))));
                    else if (entityProperties[actualPropertyName].PropertyType == typeof(TimeSpan))
                        HSafe.Run(() => sqlEntityProperties[actualPropertyName].AndIf(x => x.CanWrite, x => x.SetValue(sqlEntity, new TimeSpan(ticks.Value))));

                }

                if (!entityProperties.ContainsKey(property.Name))
                    continue;

                PropertyInfo entityProperty = entityProperties[property.Name];
                if (entityProperty.PropertyType != property.PropertyType)
                    continue;

                HSafe.Run(() => entityProperty.AndIf(x => x.CanWrite, x => x.SetValue(entity, property.GetValue(sqlEntity))));
            }

            return entity;
        }
    }
}
