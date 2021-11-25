using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAnActionQer
    {
        Task<OperationResult> Queue(QdAction action);
    }
}
