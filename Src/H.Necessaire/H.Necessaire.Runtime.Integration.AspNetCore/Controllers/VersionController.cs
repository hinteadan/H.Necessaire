using Microsoft.AspNetCore.Mvc;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Controllers
{
    [Route(HAspNetCoreConstants.ApiControllerBaseRoute)]
    [ApiController]
    public class VersionController : ControllerBase, ImAVersionUseCase
    {
        #region Construct
        readonly ImAVersionUseCase useCase;
        public VersionController
            (
            ImAVersionUseCase useCase
            )
        {
            this.useCase = useCase;
        }
        #endregion

        [Route("json"), HttpGet]
        public Task<Version> GetCurrentVersion() => useCase.GetCurrentVersion();

        [Route(""), HttpGet]
        public Task<string> GetCurrentVersionAsString() => useCase.GetCurrentVersionAsString();
    }
}
