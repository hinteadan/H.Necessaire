using H.Necessaire.Runtime.CLI.UI.Concrete;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.UI
{
    public static class CliUiProgressiveExtensions
    {
        public static Task<IDisposable> CliUiDeterministicProgressScope(this ProgressiveScope[] progressiveScopes)
            => CliUI.Shared.DeterministicProgressScope(progressiveScopes);

        public static Task<IDisposable> CliUiDeterministicProgressScope(this ProgressiveScope progressiveScope)
            => CliUI.Shared.DeterministicProgressScope(progressiveScope);

        public static Task<IDisposable> CliUiDeterministicProgressScope(this string[] progressiveScopes)
            => CliUI.Shared.DeterministicProgressScope(progressiveScopes);

        public static Task<IDisposable> CliUiDeterministicProgressScope(this string progressiveScope)
            => CliUI.Shared.DeterministicProgressScope(progressiveScope);

        public static Task<IDisposable> CliUiIndeterministicProgressScope(this ProgressiveScope[] progressiveScopes, string defaultStatus = "...")
            => CliUI.Shared.IndeterministicProgressScope(defaultStatus, progressiveScopes);

        public static Task<IDisposable> CliUiIndeterministicProgressScope(this ProgressiveScope progressiveScope, string defaultStatus = "...")
            => CliUI.Shared.IndeterministicProgressScope(defaultStatus, progressiveScope);

        public static Task<IDisposable> CliUiIndeterministicProgressScope(this string[] progressiveScopes, string defaultStatus = "...")
            => CliUI.Shared.IndeterministicProgressScope(defaultStatus, progressiveScopes);

        public static Task<IDisposable> CliUiIndeterministicProgressScope(this string progressiveScope, string defaultStatus = "...")
            => CliUI.Shared.IndeterministicProgressScope(defaultStatus, progressiveScope);
    }
}
