namespace H.Necessaire.Runtime.Security
{
    public class ExternalUseCaseCallScope : ScopedRunner
    {
        public ExternalUseCaseCallScope()
            : base(
                onStart: () =>
                {
                    CallContext<bool>.SetData(UseCaseCallScope.IsExternalUseCaseCallCallContextKey, true);
                },
                onStop: () =>
                {
                    CallContext<bool>.ZapData(UseCaseCallScope.IsExternalUseCaseCallCallContextKey);
                }
            )
        { }
    }
}
