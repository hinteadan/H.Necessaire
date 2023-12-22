namespace H.Necessaire
{
    public class ProgressiveScope : ScopedRunner
    {
        public ProgressiveScope(string scopeIdentifier, AsyncEventHandler<ProgressEventArgs> onProgress = null)
            : base(
                onStart: () =>
                {
                    CallContext<ProgressReporter>.SetData(scopeIdentifier, new ProgressReporter().And(x =>
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
        { }
    }
}
