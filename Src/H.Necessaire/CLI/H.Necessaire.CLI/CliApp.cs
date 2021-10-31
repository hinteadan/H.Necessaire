using H.Necessaire.Runtime;
using H.Necessaire.Runtime.CLI;

namespace H.Necessaire.CLI
{
    class CliApp : CliWireup
    {
        public override ImAnApiWireup WithEverything()
        {
            return
                base
                .WithEverything()
                .WithDefaultRuntimeConfig()
                .With(x => x.Register<Commands.NuGetVersioning.DependencyGroup>(() => new Commands.NuGetVersioning.DependencyGroup()))
                ;
        }
    }
}
