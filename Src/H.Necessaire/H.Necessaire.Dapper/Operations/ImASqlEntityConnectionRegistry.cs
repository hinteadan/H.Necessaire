using System;

namespace H.Necessaire.Dapper
{
    public interface ImASqlEntityConnectionRegistry : ImASqlEntityConnectionProvider
    {
        void RegisterConnectionForType(Type type, SqlEntityConnection entityConnection);
        void RegisterConnection<T>(SqlEntityConnection entityConnection);
    }
}
