using System;

namespace H.Necessaire.Dapper
{
    public interface ImASqlEntityConnectionProvider
    {
        SqlEntityConnection GetConnectionStringByType(Type type);
        SqlEntityConnection GetConnectionString<T>();
    }
}
