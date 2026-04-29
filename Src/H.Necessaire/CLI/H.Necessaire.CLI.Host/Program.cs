using H.Necessaire.CLI;
using H.Necessaire.Runtime.CLI;
using System.Threading.Tasks;

public static class Program
{
    public static async Task Main()
    {
        await
            new App()
            .WithEverything()
            .With(x => x.Register<H.Necessaire.Runtime.RuntimeDependencyGroup>(() => new H.Necessaire.Runtime.RuntimeDependencyGroup()))
            .Run()
            ;
    }
}
