using System.Diagnostics;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Middlewares
{
    internal class ExceptionPresentationModel
    {
        const string defaultError = "There was an error processing your request. If any details are available you'll see them below in the Reasons section.";

        public ExceptionPresentationModel() { }
        public ExceptionPresentationModel(Exception exception) : this()
        {
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

                    if (!Debugger.IsAttached)
                        return;
                }

                if (!Debugger.IsAttached)
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

            if (!Debugger.IsAttached)
                return;

            var exReasons = exception.Flatten()?.Select(ex => string.Join("", ex.GetType().Name, " ", "Exception Message: ", ex.Message.IfEmpty("[No Message]"))).ToArrayNullIfEmpty();

            if (exReasons.IsEmpty())
                return;

            if (exReasons.Length == 1 && Error == defaultError)
                Error = exReasons[0];
            else
                Reasons = Reasons.IsEmpty() ? exReasons : Reasons.Push(exReasons, checkDistinct: false);
        }

        public string Error { get; set; } = defaultError;
        public string[] Reasons { get; set; }

        public static implicit operator ExceptionPresentationModel(Exception exception) => new ExceptionPresentationModel(exception);
    }
}
