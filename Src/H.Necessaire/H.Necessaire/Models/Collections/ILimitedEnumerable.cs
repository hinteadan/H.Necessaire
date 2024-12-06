using System.Collections.Generic;

namespace H.Necessaire
{
    public interface ILimitedEnumerable<T> : IEnumerable<T>
    {
        int Offset { get; }
        int Length { get; }
        long TotalNumberOfItems { get; }
    }
}
