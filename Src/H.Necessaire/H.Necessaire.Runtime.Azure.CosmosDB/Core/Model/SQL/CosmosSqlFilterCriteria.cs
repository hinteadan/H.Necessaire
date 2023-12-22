using System;
using System.Text;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Model.SQL
{
    public class CosmosSqlFilterCriteria : ImACosmosSqlFilterCriteria
    {
        public CosmosSqlFilterCriteria(string property, string parameterName, string @operator)
        {
            Property = property;
            ParameterName = parameterName;
            Operator = @operator;
        }

        public CosmosSqlFilterCriteria(string property, string @operator) : this(property, property, @operator) { }
        public CosmosSqlFilterCriteria(string property) : this(property, property, "=") { }

        public string Property { get; }
        public string ParameterName { get; }
        public string Operator { get; }

        public LikeOperatorMatchType LikeOperatorMatch { get; set; } = LikeOperatorMatchType.Anywhere;

        public override string ToString() => ToString(itemAlias: "doc");

        public string ToString(string itemAlias)
        {
            if (Operator?.Trim().Is("IN") == true)
            {
                return $"ARRAY_CONTAINS(@{ParameterName},{Property.ToCosmosProperty(itemAlias)}) = true";
            }

            if (Operator?.Trim().Is("NOT IN") == true)
            {
                return $"ARRAY_CONTAINS(@{ParameterName},{Property.ToCosmosProperty(itemAlias)}) != true";
            }

            bool isLikeOperator = Operator?.Trim().Is("LIKE") == true;
            
            string value = $"@{ParameterName}";
            if (isLikeOperator)
            {
                switch (LikeOperatorMatch)
                {
                    case LikeOperatorMatchType.Beginning:
                        value = $"LOWER(CONCAT(@{ParameterName},'%'))";
                        break;
                    case LikeOperatorMatchType.Ending:
                        value = $"LOWER(CONCAT('%', @{ParameterName}))";
                        break;
                    case LikeOperatorMatchType.Exact:
                        value = $"LOWER(@{ParameterName})";
                        break;
                    case LikeOperatorMatchType.Anywhere:
                    default:
                        value = $"LOWER(CONCAT('%',@{ParameterName},'%'))";
                        break;
                }
            }

            return $"{(isLikeOperator ? $"LOWER({Property.ToCosmosProperty(itemAlias)})" : Property.ToCosmosProperty(itemAlias))} {Operator}{(ParameterName.IsEmpty() ? null : $" {value}")}";
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
