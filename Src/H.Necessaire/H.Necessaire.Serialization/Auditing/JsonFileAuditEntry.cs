using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Serialization
{
    public class JsonFileAuditEntry : JsonAuditEntry
    {
        public JsonFileAuditEntry(ImAnAuditEntry metadata, FileInfo jsonFileInfo) : base(metadata, async x => await ReadJsonFile(jsonFileInfo)) { }

        static Task<Stream> ReadJsonFile(FileInfo jsonFileInfo)
        {
            if (!jsonFileInfo.Exists)
                return (null as Stream).AsTask();

            return (File.OpenRead(jsonFileInfo.FullName) as Stream).AsTask();
        }
    }
}
