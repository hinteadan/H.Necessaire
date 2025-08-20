using System.Threading.Tasks;

namespace H.Necessaire.UI
{
    public interface ImAModalController : ImAModal
    {
        Task SubmitModal(params UserOption[] userSelection);
        Task CancelModal();
    }
}
