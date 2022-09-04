using System;

namespace H.Necessaire.Analytics
{
    public class ConsumerNetworkTraceFilter : SortFilterBase
    {
        static readonly string[] validSortNames = new string[] {
            nameof(ConsumerNetworkTrace.ConsumerIdentityID),
            nameof(ConsumerNetworkTrace.ConsumerDisplayName),
            nameof(ConsumerNetworkTrace.LatestVisit),
            nameof(ConsumerNetworkTrace.OldestVisit),
        };

        public Guid?[] ConsumerIdentityIDs { get; set; }
        public string[] ConsumerDisplayNames { get; set; }
        public DateTime? FromInclusive { get; set; }
        public DateTime? ToInclusive { get; set; }
        public string[] NetworkServiceProviders { get; set; }
        public string[] IpAddresses { get; set; }
        public string[] Countries { get; set; }
        public string[] Cities { get; set; }

        protected override string[] ValidSortNames => validSortNames;

        public ConsumerNetworkTracePageFilter ToPageFilter(PageFilter pageFilter = null)
        {
            return
                new ConsumerNetworkTracePageFilter
                {
                    PageFilter = pageFilter,
                    ConsumerIdentityIDs = ConsumerIdentityIDs,
                    ConsumerDisplayNames = ConsumerDisplayNames,
                    FromInclusive = FromInclusive,
                    ToInclusive = ToInclusive,
                    NetworkServiceProviders = NetworkServiceProviders,
                    IpAddresses = IpAddresses,
                    Countries = Countries,
                    Cities = Cities,
                    SortFilters = SortFilters,
                };
        }
    }
}
