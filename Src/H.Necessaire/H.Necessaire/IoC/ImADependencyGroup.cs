using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImADependencyGroup
    {
        void RegisterDependencies(ImADependencyRegistry dependencyRegistry);
    }
}
