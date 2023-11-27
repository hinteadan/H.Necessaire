namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Model.AppState.Abstract
{
    public abstract class AppStateEntryBase<TPayload> : EphemeralType<TPayload>, IStringIdentity
    {
        protected AppStateEntryBase()
        {
            DoNotExpire();
        }

        public abstract string ID { get; }
    }
}
