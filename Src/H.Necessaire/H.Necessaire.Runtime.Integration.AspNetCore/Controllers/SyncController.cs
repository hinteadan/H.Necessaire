using Microsoft.AspNetCore.Mvc;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Controllers
{
    [Route(HAspNetCoreConstants.ApiControllerBaseRoute)]
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
