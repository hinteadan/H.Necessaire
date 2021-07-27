using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAHasherEngine
    {
        Task<SecuredHash> Hash(string value, string key = null);
        Task<OperationResult> VerifyMatch(string value, SecuredHash againstHashedValue);
    }
}
