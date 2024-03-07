using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAnAttachmentStore<TEntityID>
    {
        Task<OperationResult> SaveAttachment(TEntityID entityID, DataBin attachment);
        Task<OperationResult<DataBin[]>> LoadAllAttachments(TEntityID entityID);
    }
}
