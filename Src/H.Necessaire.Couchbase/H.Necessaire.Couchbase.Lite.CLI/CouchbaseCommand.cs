using H.Necessaire.CLI.Commands;
using System.Threading.Tasks;

namespace H.Necessaire.Couchbase.Lite.CLI
{
    [Alias("couch")]
    internal class CouchbaseCommand : CommandBase
    {
        static readonly string[] usageSyntax = [
            "couchbase|couch debug"
        ];
        protected override string[] GetUsageSyntaxes() => usageSyntax;

        public override Task<OperationResult> Run() => RunSubCommand();

        class DebugSubCommand : SubCommandBase
        {
            public override async Task<OperationResult> Run(params Note[] args)
            {
                await Task.CompletedTask;

                var couch = new CouchbaseInteractor("dev-play-db");

                using (var scope = couch.NewOperationScope())
                {
                    scope.Select<DataBinMeta>(x => x.Description.)
                }

                return OperationResult.Win();
            }
        }
    }
}
