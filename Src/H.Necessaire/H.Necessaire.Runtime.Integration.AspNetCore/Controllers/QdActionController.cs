using H.Necessaire.Runtime.Integration.AspNetCore.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Controllers
{
    [Route(HAspNetCoreConstants.ApiControllerBaseRoute)]
    [ApiController]
    public class QdActionController : ControllerBase, ImAnActionQer
    {
        #region Construct
        readonly QdActionHttpApiUseCase useCase;
        public QdActionController(ImADependencyProvider dependencyProvider)
        {
            useCase = dependencyProvider.Get<QdActionHttpApiUseCase>();
        }
        #endregion

        [Route(nameof(ImAnActionQer.Queue)), HttpPut]
        public async Task<OperationResult> Queue(QdAction action) => await useCase.Queue(action);
    }
}
