using System;
using System.Linq;

namespace H.Necessaire.Runtime.CLI.UI
{
    public class CliUiProgressiveScope : IDisposable
    {
        public static CliUiProgressiveScope Current => CallContext<CliUiProgressiveScope>.GetData("CliUiProgressiveScope");

        readonly ProgressiveScope[] progressiveScopes;
        readonly ProgressReporter[] progressReporters;
        public CliUiProgressiveScope(params ProgressiveScope[] progressiveScopes)
        {
            this.progressiveScopes = progressiveScopes;
            this.progressReporters = progressiveScopes?.Select(s => ProgressReporter.Get(s.ID)).ToNoNullsArray();

            CallContext<CliUiProgressiveScope>.SetData("CliUiProgressiveScope", this);
        }

        public CliUiProgressiveScope(params string[] progressiveScopeIDs)
            : this(progressiveScopeIDs?.Select(id => new ProgressiveScope(id)).ToArray()) { }

        public void Dispose()
        {
            CallContext<CliUiProgressiveScope>.ZapData("CliUiProgressiveScope");

            new CollectionOfDisposables<ProgressiveScope>(progressiveScopes).Dispose();
        }

        public ProgressReporter[] ProgressReporters => progressReporters;
        public ProgressReporter this[string id]
        {
            get => progressReporters?.LastOrDefault(r => r.ID == id);
        }
    }
}
