namespace H.Necessaire.Dapper
{
    public class SqlFilterCriteria : ISqlFilterCriteria
    {
        public SqlFilterCriteria(string columnName, string parameterName, string @operator)
        {
            ColumnName = columnName;
            ParameterName = parameterName;
            Operator = @operator;
        }

        public SqlFilterCriteria(string columnName, string @operator) : this(columnName, columnName, @operator) { }
        public SqlFilterCriteria(string columnName) : this(columnName, columnName, "=") { }

        public string ColumnName { get; }
        public string ParameterName { get; }
        public string Operator { get; }

        public LikeOperatorMatchType LikeOperatorMatch { get; set; } = LikeOperatorMatchType.Anywhere;

        public override string ToString()
        {
            string value = $" @{ParameterName}";
            if (string.Equals(Operator?.Trim(), "like", System.StringComparison.InvariantCultureIgnoreCase))
            {
                switch (LikeOperatorMatch)
                {
                    case LikeOperatorMatchType.Beginning:
                        value = $" CONCAT(@{ParameterName},'%')";
                        break;
                    case LikeOperatorMatchType.Ending:
                        value = $" CONCAT('%', @{ParameterName})";
                        break;
                    case LikeOperatorMatchType.Exact:
                        value = $" @{ParameterName}";
                        break;
                    case LikeOperatorMatchType.Anywhere:
                    default:
                        value = $" CONCAT('%',@{ParameterName},'%')";
                        break;
                }
            }

            return $"[{ColumnName}] {Operator}{(string.IsNullOrWhiteSpace(ParameterName) ? string.Empty : value)}";
        }

        public enum LikeOperatorMatchType
        {
            Anywhere = 0,
            Beginning = 1,
            Ending = 2,
            Exact = 3,
        }
    }
}
