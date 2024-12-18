using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class ProgressReporter : IStringIdentity
    {
        DataNormalizer percentNormalizer;
        decimal progressSourceValue = 0;
        string currentActionName;

        public ProgressReporter() : this(null) { }
        public ProgressReporter(string identifier)
        {
            ID = identifier.IsEmpty() ? Guid.NewGuid().ToString() : identifier;
        }

        public string ID { get; }

        public ProgressReporter SetSourceInterval(NumberInterval sourceInterval)
        {
            percentNormalizer = DataNormalizer.Percent(sourceInterval);
            return this;
        }

        readonly object onProgressLocker = new object();
        readonly List<AsyncEventHandler<ProgressEventArgs>> onProgressHandlers = new List<AsyncEventHandler<ProgressEventArgs>>();

        public event AsyncEventHandler<ProgressEventArgs> OnProgress
        {
            add
            {
                if (value is null)
                    return;

                lock (onProgressLocker)
                {
                    onProgressHandlers.Add(value);
                }
            }
            remove
            {
                if (value is null)
                    return;

                lock (onProgressLocker)
                {
                    onProgressHandlers.Remove(value);
                }
            }
        }

        public async Task RaiseOnProgress(string currentActionName, decimal progressSourceValue, params string[] additionalInfo)
        {
            this.progressSourceValue = progressSourceValue;
            this.currentActionName = currentActionName;

            await RaiseOnProgressAction(additionalInfo);
        }

        private async Task RaiseOnProgressAction(params string[] additionalInfo)
        {
            if (percentNormalizer == null)
                return;

            await
                new Func<Task>(async () =>
                {
                    if (onProgressHandlers.IsEmpty())
                        return;

                    await Task.WhenAll(
                        onProgressHandlers.Select(handler =>
                            handler.Invoke(
                                this,
                                new ProgressEventArgs(
                                    currentActionName,
                                    percentNormalizer.From,
                                    progressSourceValue,
                                    percentNormalizer.Do(progressSourceValue),
                                    additionalInfo
                                )
                            )
                        )
                    );
                })
                .TryOrFailWithGrace(onFail: ex => { });
        }

        public static ProgressReporter Get(string scopeIdentifier)
            => CallContext<ProgressReporter>.GetData(scopeIdentifier);
    }

    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(string currentActionName, NumberInterval sourceInterval, decimal progressSourceValue, decimal percentValue, params string[] additionalInfo)
        {
            CurrentActionName = currentActionName;
            SourceInterval = sourceInterval;
            ProgressSourceValue = progressSourceValue;
            PercentValue = percentValue;
            AdditionalInfo = additionalInfo?.Where(x => !x.IsEmpty()).ToArrayNullIfEmpty();
        }
        public string CurrentActionName { get; }
        public NumberInterval SourceInterval { get; }
        public decimal ProgressSourceValue { get; }
        public decimal PercentValue { get; }
        public string[] AdditionalInfo { get; }
    }
}
