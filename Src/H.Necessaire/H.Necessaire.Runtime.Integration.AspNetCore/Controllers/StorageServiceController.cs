using H.Necessaire.Runtime.Integration.AspNetCore.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Controllers
{
    [Route($"{HAspNetCoreConstants.ApiBaseRoute}/H-StorageService")]
    [ApiController]
    public class StorageServiceController : ControllerBase
    {
        readonly StorageServiceHttpApiUseCase storageServiceHttpApiUseCase;
        public StorageServiceController(ImADependencyProvider dependencyProvider)
        {
            storageServiceHttpApiUseCase = dependencyProvider.Get<StorageServiceHttpApiUseCase>();
        }

        [Route(nameof(ImAStorageService<,>.Save)), HttpPut]
        public async Task<IActionResult> Save() => await storageServiceHttpApiUseCase.HandleHttpApiStorageServiceRequest(Request);
    }
}
