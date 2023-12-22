using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class ProgressReporter
    {
        DataNormalizer percentNormalizer;
        decimal progressSourceValue = 0;
        string currentActionName;

        public ProgressReporter SetSourceInterval(NumberInterval sourceInterval)
        {
            percentNormalizer = DataNormalizer.Percent(sourceInterval);
            return this;
        }

        public event AsyncEventHandler<ProgressEventArgs> OnProgress;

        public async Task RaiseOnProgress(string currentActionName, decimal progressSourceValue)
        {
            this.progressSourceValue = progressSourceValue;
            this.currentActionName = currentActionName;

            await RaiseOnProgressAction();
        }

        private async Task RaiseOnProgressAction()
        {
            if (percentNormalizer == null)
                return;

            await
                new Func<Task>(async () =>
                {
                    if (OnProgress == null)
                        return;

                    await OnProgress.Invoke(this, new ProgressEventArgs(currentActionName, percentNormalizer.From, progressSourceValue, percentNormalizer.Do(progressSourceValue)));
                })
                .TryOrFailWithGrace(onFail: ex => { });
        }

        public static ProgressReporter Get(string scopeIdentifier)
            => CallContext<ProgressReporter>.GetData(scopeIdentifier);
    }

    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(string currentActionName, NumberInterval sourceInterval, decimal progressSourceValue, decimal percentValue)
        {
            CurrentActionName = currentActionName;
            SourceInterval = sourceInterval;
            ProgressSourceValue = progressSourceValue;
            PercentValue = percentValue;
        }
        public string CurrentActionName { get; }
        public NumberInterval SourceInterval { get; }
        public decimal ProgressSourceValue { get; }
        public decimal PercentValue { get; }
    }
}
