using System.Threading.Tasks;

namespace H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing
{
    internal interface ImANetworkTraceProvider
    {
        Task<OperationResult<NetworkTrace>> Trace(string ipAddress);
    }
}
