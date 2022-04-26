using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.Builders;
using H.Necessaire.Runtime.Wireup.Abstracts;
using System;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.Runtime.CLI
{
    public class CliWireup : ApiWireupBase
    {
        public override ImAnApiWireup WithEverything()
        {
            return
                base
                .WithEverything()
                .With(x => x.Register<ArgsParser>(() => new ArgsParser()))
                .With(x => x.Register<ImAUseCaseContextProvider>(() => new CliUseCaseContextProvider()))
                .With(x => x.Register<CliCommandFactory>(() => new CliCommandFactory(DependencyRegistry)))
                .With(x => AddAllUseCasesInAssemblyForType<CliWireup>(x))
                ;
        }

        private static void AddAllUseCasesInAssemblyForType<T>(ImADependencyRegistry registry)
        {
            Assembly[] assemblies = Assembly.GetExecutingAssembly().AsArray().Union(AppDomain.CurrentDomain.GetAssemblies()).ToArray();

            Type useCaseInterfaceType = typeof(ImACliCommand);
            Type[] useCaseTypes = assemblies.SelectMany(assembly => assembly.GetTypes().Where(p => useCaseInterfaceType.IsAssignableFrom(p) && !p.IsAbstract)).ToArray();

            foreach (Type useCaseType in useCaseTypes)
            {
                registry.RegisterAlwaysNew(useCaseType, () => Activator.CreateInstance(useCaseType));
            }
        }
    }
}
