using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.CommandInterpreter;
using H.Necessaire.Runtime.CLI.Common;
using H.Necessaire.Runtime.CLI.UI;
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

                .WithCliCommons()

                .WithCliUI()

                .With(x => x.WithCliInterpreter())

                .With(x => AddAllCommandsInAllAssemblies(x))
                .With(x => AddAllSubCommandsInAllAssemblies(x))

                ;
        }

        private static void AddAllCommandsInAllAssemblies(ImADependencyRegistry registry)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Type useCaseInterfaceType = typeof(ImACliCommand);
            Type[] useCaseTypes = assemblies.SelectMany(assembly => assembly.GetTypes().Where(p => useCaseInterfaceType.IsAssignableFrom(p) && !p.IsAbstract)).ToArray();

            foreach (Type useCaseType in useCaseTypes)
            {
                registry.RegisterAlwaysNew(useCaseType, () => Activator.CreateInstance(useCaseType));
            }
        }

        private static void AddAllSubCommandsInAllAssemblies(ImADependencyRegistry registry)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Type subCommandInterfaceType = typeof(ImACliSubCommand);
            Type[] subCommandsTypes = assemblies.SelectMany(assembly => assembly.GetTypes().Where(p => subCommandInterfaceType.IsAssignableFrom(p) && !p.IsAbstract)).ToArray();

            foreach (Type subCommandsType in subCommandsTypes)
            {
                registry.RegisterAlwaysNew(subCommandsType, () => Activator.CreateInstance(subCommandsType));
            }
        }
    }
}
