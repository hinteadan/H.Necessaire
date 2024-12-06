using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;
}
