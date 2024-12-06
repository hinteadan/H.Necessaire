using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImACacherRegistry
    {
        Task ClearAll();
        Task Clear(params string[] ids);
    }
}
