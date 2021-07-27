using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImASecurityUseCase : ImAUseCase
    {
        Task<OperationResult<SecurityContext>> Login(LoginCommand command);
        Task<OperationResult<SecurityContext>> Refresh(RefreshAccessTokenCommand command);
    }
}
