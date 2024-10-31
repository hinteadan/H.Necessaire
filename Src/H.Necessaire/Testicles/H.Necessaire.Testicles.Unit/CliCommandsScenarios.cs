using FluentAssertions;
using H.Necessaire.Runtime;
using H.Necessaire.Runtime.CLI;
using H.Necessaire.Runtime.CLI.Commands;
using Org.BouncyCastle.Crypto.Agreement;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class CliCommandsScenarios
    {
        [Fact(DisplayName = "CliCommandFactory Runs Expected Unique Command")]
        public async Task CliCommandFactory_Runs_Expected_Unique_Command()
        {
            CliRunCommandsLogger commandsLogger = new CliRunCommandsLogger();
            ImAnApiWireup cliWireup
                = new CliWireup()
                .WithEverything()
                .With(x => x.Register<CliRunCommandsLogger>(() => commandsLogger))
                ;

            using (CustomizableCliContextProvider.WithArgs("".NoteAs("UniqueTest")))
            {
                await cliWireup.Run();
            }

            var commandLog = commandsLogger.StreamAll().Last();

            commandLog.CommandType.Should().Be(typeof(UniqueTestCommand), because: $"{nameof(UniqueTestCommand)} has no ID or Alias attributes, the type name starts with TestDebug, and there's no other command that starts with TestDebug");


            using (CustomizableCliContextProvider.WithArgs("".NoteAs("sec-uni-test")))
            {
                await cliWireup.Run();
            }

            commandLog = commandsLogger.StreamAll().Last();

            commandLog.CommandType.Should().Be(typeof(SecondUniqueTestCommand), because: $"{nameof(SecondUniqueTestCommand)} has ID attribute sec-uni-test, and there's no other command with the same ID or same alias or type name starting with it");


            using (CustomizableCliContextProvider.WithArgs("".NoteAs("third-uni-test")))
            {
                await cliWireup.Run();
            }

            commandLog = commandsLogger.StreamAll().Last();

            commandLog.CommandType.Should().Be(typeof(ThirdUniqueTestCommand), because: $"{nameof(ThirdUniqueTestCommand)} has Alias attribute third-uni-test, and there's no other command with the same ID or same alias or type name starting with it");
        }

        [Fact(DisplayName = "CliCommandFactory Runs Expected Multi Match By Type Name Commands")]
        public async Task CliCommandFactory_Runs_Expected_Multi_Match_By_Type_Name_Commands()
        {
            CliRunCommandsLogger commandsLogger = new CliRunCommandsLogger();
            ImAnApiWireup cliWireup
                = new CliWireup()
                .WithEverything()
                .With(x => x.Register<CliRunCommandsLogger>(() => commandsLogger))
                ;

            using (CustomizableCliContextProvider.WithArgs("".NoteAs("MultiMatchByTypeName")))
            {
                await cliWireup.Run();
            }

            var commandLog = commandsLogger.StreamAll().Last();

            commandLog.CommandType.Should().Be(typeof(MultiMatchByTypeNameCommand), because: $"{nameof(MultiMatchByTypeNameCommand)} is best mathcing command based on type name as it matches exact name, even though it also partially matches MultiMatchByTypeNameSecondCommand");


            using (CustomizableCliContextProvider.WithArgs("".NoteAs("MultiMatchByTypeNameSecond")))
            {
                await cliWireup.Run();
            }

            commandLog = commandsLogger.StreamAll().Last();

            commandLog.CommandType.Should().Be(typeof(MultiMatchByTypeNameSecondCommand), because: $"{nameof(MultiMatchByTypeNameSecondCommand)} is best mathcing command based on type name as it matches exact name");
        }

        [Fact(DisplayName = "CliCommandFactory Runs Expected SubCommand")]
        public async Task CliCommandFactory_Runs_Expected_SubCommand()
        {
            CliRunCommandsLogger commandsLogger = new CliRunCommandsLogger();
            ImAnApiWireup cliWireup
                = new CliWireup()
                .WithEverything()
                .With(x => x.Register<CliRunCommandsLogger>(() => commandsLogger))
                ;

            using (CustomizableCliContextProvider.WithArgs("".NoteAs("UniqueSubCommRunnerTest"), "".NoteAs("UniqueTestSubComm")))
            {
                await cliWireup.Run();
            }

            var commandLog = commandsLogger.StreamAll().Last();

            commandLog.CommandType.Should().Be(typeof(UniqueSubCommRunnerTestCommand.UniqueTestSubComm), because: $"UniqueSubCommRunnerTestCommand.UniqueTestSubComm is the closest relative class for UniqueSubCommRunnerTestCommand, event though there's another UniqueTestSubComm class available in the upper scope");

            using (CustomizableCliContextProvider.WithArgs("".NoteAs("UniqueSubCommRunnerTest"), "".NoteAs("UniqueTestSubComm-Top")))
            {
                await cliWireup.Run();
            }

            commandLog = commandsLogger.StreamAll().Last();

            commandLog.CommandType.Should().Be(typeof(UniqueTestSubComm), because: $"UniqueTestSubComm is the identified command, by its alias, even though there's a matching, closer relative class");
        }


        private class UniqueSubCommRunnerTestCommand : TestDebugCommandBase
        {
            public override Task<OperationResult> Run()
            {
                return RunSubCommand();
            }

            public class UniqueTestSubComm : TestDebugSubCommandBase { }
        }

        [Alias("UniqueTestSubComm-Top")]
        private class UniqueTestSubComm : TestDebugSubCommandBase { }

        private class CrossAssemblySubComm : TestDebugSubCommandBase { }


        private class MultiMatchByTypeNameSecondCommand : TestDebugCommandBase { }
        private class MultiMatchByTypeNameCommand : TestDebugCommandBase { }
        


        private class UniqueTestCommand : TestDebugCommandBase { }

        [ID("sec-uni-test")]
        private class SecondUniqueTestCommand : TestDebugCommandBase { }

        [Alias("third-uni-test")]
        private class ThirdUniqueTestCommand : TestDebugCommandBase { }

        private abstract class TestDebugCommandBase : CommandBase
        {
            CliRunCommandsLogger commandsLogger;
            public override void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                base.ReferDependencies(dependencyProvider);
                commandsLogger = dependencyProvider.Get<CliRunCommandsLogger>();
            }

            public override async Task<OperationResult> Run()
            {
                CliCommandRunLogEntry logEntry = new CliCommandRunLogEntry
                {
                    CommandType = GetType(),
                    RanAt = DateTime.UtcNow,
                };

                using (new TimeMeasurement(x => logEntry.RanDuration = x))
                {
                    var args = await GetArguments();
                    logEntry.CommandName = args.First().ID;
                    logEntry.CommandArgs = args.Jump(1);
                }

                commandsLogger.Append(logEntry);

                return OperationResult.Win();
            }
        }

        private abstract class TestDebugSubCommandBase : SubCommandBase
        {
            CliRunCommandsLogger commandsLogger;
            public override void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                base.ReferDependencies(dependencyProvider);
                commandsLogger = dependencyProvider.Get<CliRunCommandsLogger>();
            }

            public override Task<OperationResult> Run(params Note[] args)
            {
                CliCommandRunLogEntry logEntry = new CliCommandRunLogEntry
                {
                    CommandName = GetType().Name,
                    CommandType = GetType(),
                    RanAt = DateTime.UtcNow,
                    CommandArgs = args,
                };

                using (new TimeMeasurement(x => logEntry.RanDuration = x))
                {
                    
                }

                commandsLogger.Append(logEntry);

                return OperationResult.Win().AsTask();
            }
        }

        private class CliRunCommandsLogger
        {
            private readonly ConcurrentQueue<CliCommandRunLogEntry> logQueue = new ConcurrentQueue<CliCommandRunLogEntry>();

            public void Append(CliCommandRunLogEntry logEntry)
            {
                logQueue.Enqueue(logEntry);
            }

            public IEnumerable<CliCommandRunLogEntry> StreamAll() => logQueue;
        }

        private class CliCommandRunLogEntry
        {
            public string CommandName { get; set; }
            public Type CommandType { get; set; }
            public Note[] CommandArgs { get; set; } = null;
            public DateTime RanAt { get; set; } = DateTime.UtcNow;
            public TimeSpan RanDuration { get; set; } = TimeSpan.Zero;
        }
    }
}
