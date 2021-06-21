using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire
{
    public interface ImADependencyBrowser
    {
        IEnumerable<KeyValuePair<Type, Func<object>>> GetAllOneTimeTypes();
        IEnumerable<KeyValuePair<Type, Func<object>>> GetAllAlwaysNewTypes();
    }
}
