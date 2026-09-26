using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.CommandInterpreter;
using H.Necessaire.Runtime.CLI.Common;
using H.Necessaire.Runtime.CLI.UI;
using H.Necessaire.Runtime.Wireup.Abstracts;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace H.Necessaire.Runtime.CLI
{
    public class CliWireup : ApiWireupBase
    {
        readonly CancellationTokenSource cliCancellationTokenSource = new CancellationTokenSource();
        public CliWireup()
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                HandleExit();
            };
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                HandleExit();
            };
        }


        public override ImAnApiWireup WithEverything()
        {
            return
                base

                .WithEverything()
                .With(x => x.Register<ImACancellationManager>(() => x.GetNewCancellationManager(cliCancellationTokenSource.Token)))

                .WithCliCommons()

                .WithCliUI()

                .With(x => x.WithCliInterpreter())

                .With(x => AddAllCommandsInAllAssemblies(x))
                .With(x => AddAllSubCommandsInAllAssemblies(x))

                ;
        }

        void HandleExit()
        {
            HSafe.Run(() =>
            {
                cliCancellationTokenSource.Cancel();
                cliCancellationTokenSource.Dispose();
            });

        }

        static void AddAllCommandsInAllAssemblies(ImADependencyRegistry registry)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetNonCoreAssemblies();

            Type useCaseInterfaceType = typeof(ImACliCommand);
            Type[] useCaseTypes = assemblies.SelectMany(assembly => assembly.GetTypes().Where(p => useCaseInterfaceType.IsAssignableFrom(p) && !p.IsAbstract)).ToArray();

            foreach (Type useCaseType in useCaseTypes)
            {
                registry.RegisterAlwaysNew(useCaseType, () => Activator.CreateInstance(useCaseType));
            }
        }

        static void AddAllSubCommandsInAllAssemblies(ImADependencyRegistry registry)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetNonCoreAssemblies();

            Type subCommandInterfaceType = typeof(ImACliSubCommand);
            Type[] subCommandsTypes = assemblies.SelectMany(assembly => assembly.GetTypes().Where(p => subCommandInterfaceType.IsAssignableFrom(p) && !p.IsAbstract)).ToArray();

            foreach (Type subCommandsType in subCommandsTypes)
            {
                registry.RegisterAlwaysNew(subCommandsType, () => Activator.CreateInstance(subCommandsType));
            }
        }
    }
}
