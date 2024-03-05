using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImACacher
    {
        Task ClearAll();
    }

    public interface ImACacher<T> : ImACacher
    {
    }
}
