namespace H.Necessaire.Runtime.SqlServer
{
    internal static class SqlScriptExtensions
    {
        public static string PrintColumnIndexCreationSqlScriptOn(this string columnName, string tableName)
        {
            return $@"
CREATE NONCLUSTERED INDEX [IX_{tableName}_{columnName}] ON [dbo].[{tableName}]
(
	[{columnName}] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

";
        }

        public static string PrintColumnAsPrimaryKeyConstraintSqlScriptOn(this string columnName, string tableName)
        {
            return $@"
CONSTRAINT [PK_{tableName}] PRIMARY KEY
	(
		[{columnName}] ASC
	) 
	WITH 
	(
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
	)
	ON [PRIMARY]
";
        }
    }
}
