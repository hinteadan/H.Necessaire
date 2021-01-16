using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    public static class ExceptionExtensions
    {
        public static Exception[] Flatten(this Exception ex)
        {
            if (ex == null)
                return new Exception[0];

            List<Exception> result = new List<Exception>();

            if (ex is AggregateException)
            {
                result.Add(ex);
                result.AddRange((ex as AggregateException).InnerExceptions?.SelectMany(x => x.Flatten()).ToArray() ?? new Exception[0]);
            }
            else if (ex.InnerException != null)
            {
                result.Add(ex);
                result.AddRange(ex.InnerException.Flatten());
            }
            else
            {
                result.Add(ex);
            }

            return result.Where(x => x != null).ToArray();
        }
    }
}
