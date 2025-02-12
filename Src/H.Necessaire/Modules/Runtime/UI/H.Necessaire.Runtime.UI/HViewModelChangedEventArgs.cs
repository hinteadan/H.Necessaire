using System;

namespace H.Necessaire.Runtime.UI
{
    public class HViewModelChangedEventArgs : EventArgs
    {
        public HViewModelChangedEventArgs(params HViewModelPropertyChangeInfo[] changedProperties)
        {
            this.ChangedProperties = changedProperties;
        }

        public HViewModelPropertyChangeInfo[] ChangedProperties { get; }
    }
}
