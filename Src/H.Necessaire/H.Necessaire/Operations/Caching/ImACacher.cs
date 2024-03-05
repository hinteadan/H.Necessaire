using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImACacher
    {
        Task RunHousekeepingSession();
        Task ClearAll();
    }

    public interface ImACacher<T> : ImACacher
    {
    }
}
