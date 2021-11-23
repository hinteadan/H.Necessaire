using H.Necessaire;
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    public class ConcurrentQueue<T> : Queue<T>
    {
        //
        // Summary:
        //     Tries to remove and return the object at the beginning of the concurrent queue.
        //
        // Parameters:
        //   result:
        //     When this method returns, if the operation was successful, result contains the
        //     object removed. If no object was available to be removed, the value is unspecified.
        //
        // Returns:
        //     true if an element was removed and returned from the beginning of the System.Collections.Concurrent.ConcurrentQueue`1
        //     successfully; otherwise, false.
        public bool TryDequeue(out T result)
        {
            bool isSuccessful = false;
            T triedResult = default(T);
            new Action(() => { triedResult = Dequeue(); isSuccessful = true; }).TryOrFailWithGrace(onFail: ex => isSuccessful = false);
            result = triedResult;
            return isSuccessful;
        }
        //
        // Summary:
        //     Tries to return an object from the beginning of the System.Collections.Concurrent.ConcurrentQueue`1
        //     without removing it.
        //
        // Parameters:
        //   result:
        //     When this method returns, result contains an object from the beginning of the
        //     System.Collections.Concurrent.ConcurrentQueue`1 or an unspecified value if the
        //     operation failed.
        //
        // Returns:
        //     true if an object was returned successfully; otherwise, false.
        public bool TryPeek(out T result)
        {
            bool isSuccessful = false;
            T triedResult = default(T);
            new Action(() => { triedResult = Peek(); isSuccessful = true; }).TryOrFailWithGrace(onFail: ex => isSuccessful = false);
            result = triedResult;
            return isSuccessful;
        }
    }
}
