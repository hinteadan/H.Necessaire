using H.Necessaire.Runtime.Azure.CosmosDB.Security.Resources.Model;
using System;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Security.Resources.Filters
{
    class UserAuthKeyFilter : IDFilter<Guid>
    {
        static readonly string[] validSortNames = new string[] {
            nameof(UserAuthKey.ID),
            nameof(UserAuthKey.Key),
        };
        protected override string[] ValidSortNames => validSortNames;
    }
}
