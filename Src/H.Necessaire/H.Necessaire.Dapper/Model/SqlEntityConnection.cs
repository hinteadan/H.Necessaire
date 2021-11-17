using System;

namespace H.Necessaire.Dapper
{
    public class SqlEntityConnection
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public Type Type { get; set; }
    }
}
