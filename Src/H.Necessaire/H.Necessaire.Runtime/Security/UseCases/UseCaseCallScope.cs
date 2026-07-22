namespace H.Necessaire.Runtime.Security
{
    public static class UseCaseCallScope
    {
        internal const string IsExternalUseCaseCallCallContextKey = "IsExternalUseCaseCall";

        public static bool IsExternalUseCaseCall => CallContext<bool>.GetData(IsExternalUseCaseCallCallContextKey);
    }
}
