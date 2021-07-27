using H.Necessaire.Runtime.Security.Managers;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    class SecurityUseCase : UseCaseBase, ImASecurityUseCase
    {
        #region Construct
        ImASecurityManager securityManager;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            securityManager = dependencyProvider.Get<ImASecurityManager>();
        }
        #endregion

        public Task<OperationResult<SecurityContext>> Login(LoginCommand command)
        {
            return securityManager.AuthenticateCredentials(command?.Username, command?.Password);
        }

        public Task<OperationResult<SecurityContext>> Refresh(RefreshAccessTokenCommand command)
        {
            return securityManager.RefreshAccessToken(command?.ExpiredAccessToken, command?.RefreshToken);
        }
    }
}
