using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Dapper
{
    public class ComposedSqlFilterCriteria : ISqlFilterCriteria
    {
        public ComposedSqlFilterCriteria(IEnumerable<ISqlFilterCriteria> criterias, string joinBy)
        {
            JoinOperator = joinBy;
            Criterias = criterias?.Where(x => x != null).ToArray() ?? new ISqlFilterCriteria[0];
        }

        public ComposedSqlFilterCriteria(params ISqlFilterCriteria[] criterias) : this(criterias, "OR")
        {

        }

        public string JoinOperator { get; } = "OR";
        public ISqlFilterCriteria[] Criterias { get; } = new ISqlFilterCriteria[0];

        public override string ToString()
        {
            return
                $"({string.Join($" {JoinOperator} ", Criterias.Select(x => x.ToString()))})";
        }
    }
}
