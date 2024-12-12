using H.Necessaire.CLI;
using H.Necessaire.Runtime.CLI;
using System.Threading.Tasks;

public static class Program
{
    public static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
        await
            new App()
            .WithEverything()
            .Run()
            ;
    }
}
