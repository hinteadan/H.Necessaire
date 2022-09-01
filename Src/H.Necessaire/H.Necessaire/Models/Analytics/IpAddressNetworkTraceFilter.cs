using System;

namespace H.Necessaire.Analytics
{
    public class IpAddressNetworkTraceFilter : SortFilterBase
    {
        static readonly string[] validSortNames = new string[] {
            nameof(IpAddressNetworkTrace.IpAddress),
            nameof(IpAddressNetworkTrace.LatestVisit),
            nameof(IpAddressNetworkTrace.OldestVisit),
        };

        public string[] IpAddresses { get; set; }
        public DateTime? FromInclusive { get; set; }
        public DateTime? ToInclusive { get; set; }
        public string[] NetworkServiceProviders { get; set; }
        public Guid?[] ConsumerIdentityIDs { get; set; }
        public string[] Countries { get; set; }
        public string[] Cities { get; set; }

        protected override string[] ValidSortNames => validSortNames;

        public IpAddressNetworkTracePageFilter ToPageFilter(PageFilter pageFilter = null)
        {
            return
                new IpAddressNetworkTracePageFilter
                {
                    PageFilter = pageFilter,
                    IpAddresses = IpAddresses,
                    FromInclusive = FromInclusive,
                    ToInclusive = ToInclusive,
                    NetworkServiceProviders = NetworkServiceProviders,
                    ConsumerIdentityIDs = ConsumerIdentityIDs,
                    Countries = Countries,
                    Cities = Cities,
                    SortFilters = SortFilters,
                };
        }
    }
}
