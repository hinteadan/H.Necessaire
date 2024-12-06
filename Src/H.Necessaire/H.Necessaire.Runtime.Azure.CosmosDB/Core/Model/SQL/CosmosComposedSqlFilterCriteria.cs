using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Model.SQL
{
    public class CosmosComposedSqlFilterCriteria : ImACosmosSqlFilterCriteria
    {
        public CosmosComposedSqlFilterCriteria(IEnumerable<ImACosmosSqlFilterCriteria> criterias, string joinBy)
        {
            JoinOperator = joinBy;
            Criterias = criterias?.ToNoNullsArray();
        }

        public CosmosComposedSqlFilterCriteria(params ImACosmosSqlFilterCriteria[] criterias) : this(criterias, "OR") { }

        public string JoinOperator { get; } = "OR";

        public ImACosmosSqlFilterCriteria[] Criterias { get; }

        public override string ToString() => ToString(itemAlias: "doc");

        public string ToString(string itemAlias)
        {
            if (Criterias?.Any() != true)
                return null;

            return
                $"({string.Join($" {JoinOperator} ", Criterias.Select(x => x.ToString(itemAlias)))})";
        }
    }
}
