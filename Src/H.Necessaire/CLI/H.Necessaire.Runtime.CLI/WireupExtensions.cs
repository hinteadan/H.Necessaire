using H.Necessaire.Runtime.CLI.Builders;

namespace H.Necessaire.Runtime.CLI
{
    public static class WireupExtensions
    {
        public static async Task Run(this ImAnApiWireup wireup, bool askForCommandIfEmpty = false)
        {
            OperationResult result =
                (
                await
                    wireup
                    .DependencyRegistry
                    .Get<CliCommandFactory>()
                    .Run(askForCommandIfEmpty)
                )
                ?? OperationResult.Win()
                ;

            if (!result.IsSuccessful)
            {
                Console.WriteLine(String.Join(Environment.NewLine, result.FlattenReasons()));
            }

        }
    }
}
