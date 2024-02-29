using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImACacher<T>
    {
        Task ClearAll();
    }
}
