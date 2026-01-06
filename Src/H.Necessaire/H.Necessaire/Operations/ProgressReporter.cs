using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class ProgressReporter : IStringIdentity
    {
        DataNormalizer percentNormalizer;
        double progressSourceValue = 0;
        string currentActionName;
        readonly AsyncEventRaiser<ProgressEventArgs> progressRaiser;

        public ProgressReporter() : this(null) { }
        public ProgressReporter(string identifier)
        {
            ID = identifier.IsEmpty() ? Guid.NewGuid().ToString() : identifier;
            progressRaiser = new AsyncEventRaiser<ProgressEventArgs>(this);
        }

        public string ID { get; }

        public ProgressReporter SetSourceInterval(NumberInterval sourceInterval)
        {
            percentNormalizer = DataNormalizer.Percent(sourceInterval);
            return this;
        }


        public event AsyncEventHandler<ProgressEventArgs> OnProgress { add => progressRaiser.OnEvent += value; remove => progressRaiser.OnEvent -= value; }

        public async Task RaiseOnProgress(string currentActionName, double progressSourceValue, params string[] additionalInfo)
        {
            this.progressSourceValue = progressSourceValue;
            this.currentActionName = currentActionName;

            await RaiseOnProgressAction(additionalInfo);
        }

        private async Task RaiseOnProgressAction(params string[] additionalInfo)
        {
            if (percentNormalizer == null)
                return;

            await progressRaiser.Raise(new ProgressEventArgs(
                currentActionName,
                percentNormalizer.From,
                progressSourceValue,
                percentNormalizer.Do(progressSourceValue),
                additionalInfo
            ));
        }

        public static ProgressReporter Get(string scopeIdentifier)
            => CallContext<ProgressReporter>.GetData(scopeIdentifier);
    }

    public static class ProgressReporterExtensions
    {
        public static async Task ReportProgress(this string progressiveScopeID, string currentActionName, double progressRelativeToSourceInterval = 0, NumberInterval? sourceIntervalToResetTo = null, params string[] additionalInfo)
        {
            ProgressReporter progressReporter = ProgressReporter.Get(progressiveScopeID);
            if (progressReporter is null)
                return;

            if (sourceIntervalToResetTo != null && !sourceIntervalToResetTo.Value.IsInfinite)
                progressReporter.SetSourceInterval(sourceIntervalToResetTo.Value);

            await progressReporter.RaiseOnProgress(currentActionName, progressRelativeToSourceInterval, additionalInfo);
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(string currentActionName, NumberInterval sourceInterval, double progressSourceValue, double percentValue, params string[] additionalInfo)
        {
            CurrentActionName = currentActionName;
            SourceInterval = sourceInterval;
            ProgressSourceValue = progressSourceValue;
            PercentValue = percentValue;
            AdditionalInfo = additionalInfo?.Where(x => !x.IsEmpty()).ToArrayNullIfEmpty();
        }
        public string CurrentActionName { get; }
        public NumberInterval SourceInterval { get; }
        public double ProgressSourceValue { get; }
        public double PercentValue { get; }
        public string[] AdditionalInfo { get; }
    }
}
