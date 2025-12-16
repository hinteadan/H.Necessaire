using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H.Necessaire.Runtime.Integration.AspNetCore.UseCases
{
    internal class StorageServiceHttpApiUseCase : UseCaseBase
    {
        ImADependencyProvider dependencyProvider;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            this.dependencyProvider = dependencyProvider;
        }

        public async Task<IActionResult> HandleHttpApiStorageServiceRequest(HttpRequest request)
        {
            if (!(await EnsureAuthentication()).Ref(out var authRes, out var ctx))
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            return await request.HandleHttpApiStorageServiceRequest(dependencyProvider);
        }
    }
}
