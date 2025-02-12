using H.Necessaire.CLI;
using H.Necessaire.Runtime.CLI;

namespace H.Necessaire.Runtime.UI.CLI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await new CliApp()
                .WithEverything()
                .With(x => x.WithHNecessaireRuntimeUI())
                .With(x => x.Register<BLL.DependencyGroup>(() => new BLL.DependencyGroup()))
                .Run()
                ;
        }
    }
}
