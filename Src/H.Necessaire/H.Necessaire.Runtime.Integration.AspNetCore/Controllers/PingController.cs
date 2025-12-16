using Microsoft.AspNetCore.Mvc;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Controllers
{
    [Route(HAspNetCoreConstants.ApiControllerBaseRoute)]
    [ApiController]
    public class PingController : ControllerBase, ImAPingUseCase
    {
        #region Construct
        readonly ImAPingUseCase useCase;
        public PingController
            (
            ImAPingUseCase useCase
            )
        {
            this.useCase = useCase;
        }
        #endregion

        [Route(""), HttpGet]
        public Task<string> Pong()
            => useCase.Pong();

        [Route("secured"), HttpGet]
        public Task<string> SecuredPong()
            => useCase.SecuredPong();
    }
}
