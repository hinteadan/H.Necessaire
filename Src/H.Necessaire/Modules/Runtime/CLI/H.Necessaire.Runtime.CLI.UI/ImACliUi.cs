using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.UI
{
    public interface ImACliUI : ImACliUiProgressIndicator, ImACliUiStatusIndicator
    {
    }

    public interface ImACliUiProgressIndicator
    {
        Task<IDisposable> DeterministicProgressScope(params ProgressiveScope[] progressiveScopes);
        Task<IDisposable> DeterministicProgressScope(params string[] progressiveScopes);
    }

    public interface ImACliUiStatusIndicator
    {
        Task<IDisposable> IndeterministicProgressScope(string defaultStatus = "...", params ProgressiveScope[] progressiveScopes);
        Task<IDisposable> IndeterministicProgressScope(string defaultStatus = "...", params string[] progressiveScopes);
    }
}
