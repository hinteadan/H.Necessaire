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

                new Action(() => sqlEntityProperty.SetValue(sqlEntity, property.GetValue(entity))).TryOrFailWithGrace();
            }

            return sqlEntity;
        }

        public virtual TEntity MapSqlToEntity(TSqlEntity sqlEntity)
        {
            TEntity entity = new TEntity();

            foreach (PropertyInfo property in sqlEntityProperties.Values)
            {
                if (!entityProperties.ContainsKey(property.Name))
                    continue;

                PropertyInfo entityProperty = entityProperties[property.Name];
                if (entityProperty.PropertyType != property.PropertyType)
                    continue;

                new Action(() => entityProperty.SetValue(entity, property.GetValue(sqlEntity))).TryOrFailWithGrace();
            }

            return entity;
        }
    }
}
