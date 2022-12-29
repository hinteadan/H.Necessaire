using H.Necessaire.Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using static H.Necessaire.Runtime.SqlServer.Core.Resources.NetworkTraceSqlServerStorageResource;
using static H.Necessaire.Runtime.SqlServer.Core.Resources.RuntimeTraceSqlServerStorageResource;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class DataBinSqlServerStorageResource
    {
        const string dataBinTableName = "H.Necessaire.DataBin";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(DataBin),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{dataBinTableName}] 
(
	[{nameof(DataBinSqlMetaEntry.ID)}] [uniqueidentifier] NOT NULL,
    [{nameof(DataBinSqlMetaEntry.Name)}] [nvarchar](450) NULL,
    [{nameof(DataBinSqlMetaEntry.Description)}] [ntext] NULL,
    [{nameof(DataBinSqlMetaEntry.AsOf)}] [datetime2](7) NOT NULL,
    [{nameof(DataBinSqlMetaEntry.AsOfTicks)}] [bigint] NOT NULL,

    [{nameof(DataBinSqlMetaEntry.FormatJson)}] [ntext] NULL,
    [{nameof(DataBinSqlMetaEntry.FormatID)}] [nvarchar](150) NULL,
    [{nameof(DataBinSqlMetaEntry.FormatExtension)}] [nvarchar](50) NULL,
    [{nameof(DataBinSqlMetaEntry.FormatMimeType)}] [nvarchar](150) NULL,
    [{nameof(DataBinSqlMetaEntry.FormatEncoding)}] [nvarchar](150) NULL,

    [{nameof(DataBinSqlMetaEntry.NotesJson)}] [ntext] NULL,
    [{nameof(DataBinSqlMetaEntry.NotesString)}] [nvarchar](450) NULL,

    [Content] [VARBINARY](MAX) NULL, 

	{nameof(DataBinSqlMetaEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn(dataBinTableName)}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

{nameof(DataBinSqlMetaEntry.Name).PrintColumnIndexCreationSqlScriptOn(dataBinTableName)}
{nameof(DataBinSqlMetaEntry.AsOf).PrintColumnIndexCreationSqlScriptOn(dataBinTableName)}
{nameof(DataBinSqlMetaEntry.AsOfTicks).PrintColumnIndexCreationSqlScriptOn(dataBinTableName)}
{nameof(DataBinSqlMetaEntry.FormatID).PrintColumnIndexCreationSqlScriptOn(dataBinTableName)}
{nameof(DataBinSqlMetaEntry.FormatExtension).PrintColumnIndexCreationSqlScriptOn(dataBinTableName)}
{nameof(DataBinSqlMetaEntry.FormatMimeType).PrintColumnIndexCreationSqlScriptOn(dataBinTableName)}
{nameof(DataBinSqlMetaEntry.FormatEncoding).PrintColumnIndexCreationSqlScriptOn(dataBinTableName)}
{nameof(DataBinSqlMetaEntry.NotesString).PrintColumnIndexCreationSqlScriptOn(dataBinTableName)}

",
            },
        };
    }
}
