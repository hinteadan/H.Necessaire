using System.Threading.Tasks;

namespace H.Necessaire.Runtime.ExternalCommandRunner
{
    internal class ContextualExternalCommandRunner : ImAContextualExternalCommandRunner
    {
        readonly ImAnExternalCommandRunner externalCommandRunner;
        readonly ExternalCommandRunContext context;
        public ContextualExternalCommandRunner
            (
            ImAnExternalCommandRunner externalCommandRunner,
            ExternalCommandRunContext context
            )
        {
            this.externalCommandRunner = externalCommandRunner;
            this.context = context;
        }

        public async Task<OperationResult<ExternalCommandRunContext>> Run(params Note[] args)
        {
            using (context.Scope())
            {
                return await externalCommandRunner.Run(args);
            }
        }

        public async Task<OperationResult<ExternalCommandRunContext>> RunCmd(params Note[] args)
        {
            using (context.Scope())
            {
                return await externalCommandRunner.RunCmd(args);
            }
        }
    }
}
