using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAQdActionProcessor
    {
        Task<bool> IsEligibleFor(QdAction action);
        Task<QdActionResult> Process(QdAction action);
    }
}
