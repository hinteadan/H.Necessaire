namespace H.Necessaire.Runtime.ExternalCommandRunner
{
    internal class ExternalCommandRunScope : ScopedRunner
    {
        public ExternalCommandRunScope(ExternalCommandRunContext externalCommandRunContext)
            : base(
                  onStart: () =>
                  {
                      CallContext<ExternalCommandRunContext>.SetData(nameof(ExternalCommandRunScope), externalCommandRunContext);
                  },
                  onStop: () =>
                  {
                      CallContext<ExternalCommandRunContext>.ZapData(nameof(ExternalCommandRunScope));
                  }
            )
        {

        }

        public static ExternalCommandRunContext GetCurrentContext() => CallContext<ExternalCommandRunContext>.GetData(nameof(ExternalCommandRunScope));
    }
}
