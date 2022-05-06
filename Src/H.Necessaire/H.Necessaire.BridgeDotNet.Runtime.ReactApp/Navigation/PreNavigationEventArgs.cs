using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class PreNavigationEventArgs : EventArgs
    {
        #region Construct
        public PreNavigationEventArgs()
        {

        }
        #endregion

        public bool IsNavigationCancelled { get; private set; } = false;

        public PreNavigationEventArgs CancelNavigation()
        {
            return
                this
                .And(x => x.IsNavigationCancelled = true)
                ;
        }
    }
}
