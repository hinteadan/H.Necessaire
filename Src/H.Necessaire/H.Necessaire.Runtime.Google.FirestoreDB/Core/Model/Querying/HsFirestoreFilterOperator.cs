using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Model.Querying
{
    public enum HsFirestoreFilterOperator
    {
        ContainsAny = -11,
        ContainsAll = -10,

        NotEqualToCaseInsensitive = -2,
        EqualToCaseInsensitive = -1,


        EqualTo = 0,
        NotEqualTo = 1,

        LessThan = 10,
        LessThanOrEqualTo = 11,
        GreaterThan = 20,
        GreaterThanOrEqualTo = 21,

        ArrayContains = 100,
        ArrayContainsAny = 101,

        InArray = 110,
        NotInArray = 111,
    }
}
