using System;

namespace H.Necessaire
{
    public class QdActionResult : OperationResult<QdAction>, IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid QdActionID => Payload.ID;
        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;
    }

    public static class QdActionResultExtensions
    {
        public static QdActionResult ToQdActionResult(this OperationResult<QdAction> operationResult)
        {
            return
                new QdActionResult
                {
                    Comments = operationResult.Comments,
                    IsSuccessful = operationResult.IsSuccessful,
                    Payload = operationResult.Payload,
                    Reason = operationResult.Reason,
                };
        }
    }
}
