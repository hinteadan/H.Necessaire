using H.Necessaire.CLI.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Commands
{
    [Alias("dbg", "debugger", "dbgr", "dbggr", "dbx", "x")]
    internal class DebugCommand : CommandBase
    {
        #region Construct
        static readonly string[] usageSyntax = new string[] {
            "debug|dbg|debugger|dbgr|dbggr|dbx|x IDebugConcreteImplementation:string",
        };
        protected override string[] GetUsageSyntaxes() => usageSyntax;

        Func<string, IDebug> debugProvider;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            debugProvider = id => id.MorphIfNotEmpty(key => dependencyProvider.Build<IDebug>(key, defaultTo: null));
        }
        #endregion

        public override async Task<OperationResult> Run()
        {
            if (!(await HSafe.Run(async () => (await GetArguments()).Jump(1)?.FirstOrDefault().ID)).Ref(out var debuggerKeyRes, out var debuggerKey))
                return debuggerKeyRes;

            if ((debugProvider?.Invoke(debuggerKey)).RefTo(out var debugger) is null)
                return $"Cannot find any debugger identified by: {debuggerKey.IfEmpty("<<NULL or EMPTY>>")}";

            return await HSafe.Run(debugger.Debug);
        }
    }
}
