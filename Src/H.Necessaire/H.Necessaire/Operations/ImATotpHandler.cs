using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImATotpHandler
    {
        Task<OperationResult<string>> Safeguard(TotpToken token);

        Task<OperationResult<TotpToken>> Validate(string safeguardedToken);
    }
}
