using System.Threading.Tasks;

namespace H.Necessaire.UI
{
    public interface ImAModal
    {
        Task<OperationResult<UserOption[]>> GetModalResult();
    }
}
