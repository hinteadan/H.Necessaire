using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore.Controllers
{
    [Route("[controller]")]
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
