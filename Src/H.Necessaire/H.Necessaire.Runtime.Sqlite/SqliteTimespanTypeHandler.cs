using Dapper;
using System;
using System.Data;

namespace H.Necessaire.Runtime.Sqlite
{
    internal class SqliteTimespanTypeHandler : SqlMapper.TypeHandler<TimeSpan>
    {
        public override void SetValue(IDbDataParameter parameter, TimeSpan timeSpan)
        {
            parameter.Value = timeSpan.ToString();
        }

        public override TimeSpan Parse(object value)
        {
            return TimeSpan.Parse((string)value);
        }
    }
}
