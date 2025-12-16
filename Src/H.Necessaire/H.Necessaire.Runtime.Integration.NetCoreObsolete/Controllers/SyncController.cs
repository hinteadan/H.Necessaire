using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SyncController : ControllerBase, ImASyncUseCase
    {
        #region Construct
        readonly ImASyncUseCase useCase;
        public SyncController
            (
            ImASyncUseCase useCase
            )
        {
            this.useCase = useCase;
        }
        #endregion

        [Route(nameof(Sync)), HttpPost]
        public Task<OperationResult<SyncResponse>[]> Sync(SyncRequest[] syncRequests) => useCase.Sync(syncRequests);
    }
}
