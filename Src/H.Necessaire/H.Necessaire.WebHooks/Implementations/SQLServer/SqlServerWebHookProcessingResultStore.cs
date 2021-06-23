using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using H.Necessaire.WebHooks.Implementations.SQLServer.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.WebHooks.Implementations.SQLServer
{
    class SqlServerWebHookProcessingResultStore : DapperSqlResourceBase, IWebHookProcessingResultStorage
    {
        #region Construct
        const string defaultTableName = "WebHookProcessingResult";

        public SqlServerWebHookProcessingResultStore(string connectionString)
            : base(connectionString, defaultTableName)
        { }
        #endregion

        public async Task Append(WebHookProcessingResult processingResult)
        {
            await
                SaveEntity(Map(processingResult));
        }

        public async Task<WebHookProcessingResult[]> GetAllResultsForRequest(Guid webHookRequestID)
        {
            return
                (
                    await
                        LoadEntitiesByCustomCriteria<WebHookProcessingResultSqlEntry>(
                            new SqlFilterCriteria(nameof(WebHookProcessingResultSqlEntry.WebHookRequestID), nameof(webHookRequestID), "=").AsArray(),
                            new { webHookRequestID }
                        )
                )
                ?.Select(Map)
                .ToArray()
                ??
                new WebHookProcessingResult[0];
        }

        WebHookProcessingResultSqlEntry Map(WebHookProcessingResult processingResult)
        {
            return
                new WebHookProcessingResultSqlEntry
                {
                    ID = processingResult.ID,
                    HappenedAt = processingResult.HappenedAt,
                    IsSuccessful = processingResult.IsSuccessful,
                    Reason = processingResult.Reason,
                    WebHookRequestID = processingResult.WebHookRequestID,
                    CommentsJson = processingResult.Comments.ToJsonArray(),
                };
        }

        WebHookProcessingResult Map(WebHookProcessingResultSqlEntry sqlEntry)
        {
            return
                new WebHookProcessingResult
                {
                    ID = sqlEntry.ID,
                    HappenedAt = sqlEntry.HappenedAt,
                    IsSuccessful = sqlEntry.IsSuccessful,
                    Reason = sqlEntry.Reason,
                    WebHookRequestID = sqlEntry.WebHookRequestID,
                    Comments = sqlEntry.CommentsJson.JsonToObject(defaultTo: new string[0]),
                };
        }
    }
}
