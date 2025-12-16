using Microsoft.AspNetCore.Mvc;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Controllers
{
    [Route(HAspNetCoreConstants.ApiControllerBaseRoute)]
    [ApiController]
    public class SecurityController : ControllerBase, ImASecurityUseCase
    {
        #region Construct
        readonly ImASecurityUseCase useCase;
        public SecurityController
            (
            ImASecurityUseCase useCase
            )
        {
            this.useCase = useCase;
        }
        #endregion

        [Route(nameof(Login)), HttpPost]
        public Task<OperationResult<SecurityContext>> Login([FromBody] LoginCommand command) => useCase.Login(command);

        [Route(nameof(Refresh)), HttpPost]
        public Task<OperationResult<SecurityContext>> Refresh([FromBody] RefreshAccessTokenCommand command) => useCase.Refresh(command);
    }
}
