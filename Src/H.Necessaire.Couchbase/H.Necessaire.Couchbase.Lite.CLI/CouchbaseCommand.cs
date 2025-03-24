using H.Necessaire.CLI.Commands;
using System.Threading.Tasks;

namespace H.Necessaire.Couchbase.Lite.CLI
{
    [Alias("couch")]
    internal class CouchbaseCommand : CommandBase
    {
        public override Task<OperationResult> Run()
        {
            return OperationResult.Fail("Not yet implemented").AsTask();
        }
    }
}
