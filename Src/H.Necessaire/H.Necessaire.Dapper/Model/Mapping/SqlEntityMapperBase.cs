using System.Linq;

namespace H.Necessaire.Dapper
{
    public abstract class SqlEntityMapperBase<TEntity, TSqlEntity> : ISqlEntityMapper where TEntity : new() where TSqlEntity : new()
    {
        #region Construct
        static readonly System.Reflection.PropertyInfo[] sqlEntityProperties;
        static readonly System.Reflection.PropertyInfo[] entityProperties;
        static SqlEntityMapperBase()
        {
            sqlEntityProperties = typeof(TSqlEntity).GetProperties();
            entityProperties = typeof(TEntity).GetProperties();
        }
        #endregion

        public virtual TSqlEntity MapEntityToSql(TEntity entity)
        {
            TSqlEntity sqlEntity = new TSqlEntity();

            foreach (System.Reflection.PropertyInfo property in entityProperties)
            {
                System.Reflection.PropertyInfo sqlEntityProperty = sqlEntityProperties.SingleOrDefault(x => x.Name == property.Name);
                if (sqlEntityProperty == null)
                    continue;

                sqlEntityProperty.SetValue(sqlEntity, property.GetValue(entity));
            }

            return sqlEntity;
        }

        public virtual TEntity MapSqlToEntity(TSqlEntity sqlEntity)
        {
            TEntity entity = new TEntity();

            foreach (System.Reflection.PropertyInfo property in sqlEntityProperties)
            {
                System.Reflection.PropertyInfo entityProperty = entityProperties.SingleOrDefault(x => x.Name == property.Name);
                if (entityProperty == null)
                    continue;

                entityProperty.SetValue(entity, property.GetValue(sqlEntity));
            }

            return entity;
        }
    }
}
