using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore.Controllers
{
    [Route("[controller]")]
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
