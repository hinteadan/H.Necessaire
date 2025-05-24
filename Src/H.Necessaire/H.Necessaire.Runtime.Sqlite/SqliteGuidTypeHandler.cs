using Dapper;
using System;
using System.Data;

namespace H.Necessaire.Runtime.Sqlite
{
    internal class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString().ToUpperInvariant();
        }

        public override Guid Parse(object value)
        {
            return Guid.Parse((string)value);
        }
    }
}
