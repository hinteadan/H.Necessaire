using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Caching.Concrete
{
    internal class InMemoryCacher<T> : ImACacher<T>
    {
        public Task ClearAll()
        {
            throw new NotImplementedException();
        }
    }
}
