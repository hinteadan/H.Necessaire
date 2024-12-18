namespace H.Necessaire
{
    public class ProgressiveScope : ScopedRunner, IStringIdentity
    {
        public ProgressiveScope(string scopeIdentifier, AsyncEventHandler<ProgressEventArgs> onProgress = null)
            : base(
                onStart: () =>
                {
                    CallContext<ProgressReporter>.SetData(scopeIdentifier, new ProgressReporter(scopeIdentifier).And(x =>
                    {
                        if (onProgress != null)
                            x.OnProgress += onProgress;
                    }));
                },
                onStop: () =>
                {
                    CallContext<ProgressReporter>.ZapData(scopeIdentifier);
                }
            )
        {
            ID = scopeIdentifier;
        }

        public string ID { get; }
    }
}
