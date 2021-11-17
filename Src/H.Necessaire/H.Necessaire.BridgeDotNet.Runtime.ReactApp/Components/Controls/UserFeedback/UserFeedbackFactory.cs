using Bridge.React;
using H.Necessaire.UI;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class UserFeedbackFactory
    {
        public static Func<UserOptionsContext, ReactElement> Alert => x => new Alert(new UserFeedbackProps { UserOptionsContext = x });
        public static Func<UserOptionsContext, ReactElement> Confirm => x => new Confirm(new UserFeedbackProps { UserOptionsContext = x });
    }
}
