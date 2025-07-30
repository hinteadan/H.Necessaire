using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImANotificationPermissionsEnsurer
    {
        Task<OperationResult> RequestPermissions();
    }
}
