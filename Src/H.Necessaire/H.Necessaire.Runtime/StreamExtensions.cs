using H.Necessaire.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public static class StreamExtensions
    {
        public static async Task<Stream> ToJsonUTF8Stream<T>(this IEnumerable<T> collection)
        {
            MemoryStream result = new MemoryStream();

            if (collection?.Any() != true)
            {
                await "[]".WriteToStreamAsync(result);
                result.Position = 0;
                return result;
            }

            await "[".WriteToStreamAsync(result);

            foreach (T entry in collection)
            {
                await entry.ToJsonObject().WriteToStreamAsync(result);
                await ",".WriteToStreamAsync(result);
            }

            result.Position -= 1;

            await "]".WriteToStreamAsync(result);

            result.Position = 0;

            return result;
        }
    }
}
