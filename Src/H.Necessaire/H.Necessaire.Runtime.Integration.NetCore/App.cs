namespace H.Necessaire.Runtime.Integration.NetCore
{
    public class App
    {
        public ImAnApiWireup Wireup { get; }
        public App(ImAnApiWireup wireup)
        {
            this.Wireup = wireup;
        }
    }
}
