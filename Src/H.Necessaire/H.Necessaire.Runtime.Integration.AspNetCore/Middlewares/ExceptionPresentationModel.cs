namespace H.Necessaire.Runtime.Integration.AspNetCore.Middlewares
{
    internal class ExceptionPresentationModel
    {
        const string defaultError = "There was an error processing your request. If any details are available you'll see them below in the Reasons section.";
#if DEBUG
        const bool isDebug = true;
#else
        const bool isDebug = false;
#endif

        public ExceptionPresentationModel() { }
        public ExceptionPresentationModel(Exception exception) : this()
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (exception is null)
                return;

            if (exception is OperationResultException opResException)
            {
                OperationResult opRes = opResException?.OperationResult;
                if (opRes?.HasReasonsToDisplay() == true)
                {
                    if (opRes.ReasonsToDisplay.Length == 1)
                        Error = opRes.ReasonsToDisplay[0];
                    else
                        Reasons = opRes.ReasonsToDisplay;

                    if (!isDebug)
                        return;
                }

                if (!isDebug)
                    return;

                var reasons = opRes?.FlattenReasons();
                if (!reasons.IsEmpty())
                {
                    if (reasons.Length == 1 && Error == defaultError)
                        Error = reasons[0];
                    else
                        Reasons = Reasons.IsEmpty() ? reasons : Reasons.Push(reasons);
                }
            }

            if (!isDebug)
                return;

            var exReasons = exception.Flatten()?.Select(ex => string.Join("", ex.GetType().Name, " ", "Exception Message: ", ex.Message.IfEmpty("[No Message]"))).ToArrayNullIfEmpty();

            if (exReasons.IsEmpty())
                return;

            if (exReasons.Length == 1 && Error == defaultError)
                Error = exReasons[0];
            else
                Reasons = Reasons.IsEmpty() ? exReasons : Reasons.Push(exReasons, checkDistinct: false);

#pragma warning restore CS0162 // Unreachable code detected
        }

        public string Error { get; set; } = defaultError;
        public string[] Reasons { get; set; }

        public static implicit operator ExceptionPresentationModel(Exception exception) => new ExceptionPresentationModel(exception);
    }
}
