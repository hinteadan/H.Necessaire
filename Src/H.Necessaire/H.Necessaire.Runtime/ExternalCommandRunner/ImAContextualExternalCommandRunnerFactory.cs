using System.Threading;

namespace H.Necessaire.Runtime.ExternalCommandRunner
{
    public interface ImAContextualExternalCommandRunnerFactory
    {
        ImAContextualExternalCommandRunner WithContext(ExternalCommandRunContext context);
        ImAContextualExternalCommandRunner WithContext(bool isOutputPrinted = true, bool isOutputCaptured = false, bool isUserInputExpected = false, CancellationToken? cancellationToken = null);
    }
}