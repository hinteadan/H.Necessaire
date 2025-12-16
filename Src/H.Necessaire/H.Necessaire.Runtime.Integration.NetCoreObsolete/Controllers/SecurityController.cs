using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore.Controllers
{
    [Route("[controller]")]
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
