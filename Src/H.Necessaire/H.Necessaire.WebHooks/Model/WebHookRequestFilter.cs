using System;

namespace H.Necessaire.WebHooks
{
    public class WebHookRequestFilter : SortFilterBase, IPageFilter
    {
        static readonly string[] validSortNames = new string[] { nameof(IWebHookRequest.ID), nameof(IWebHookRequest.HappenedAt), nameof(IWebHookRequest.Source) };

        public Guid[] IDs { get; set; }
        public string[] Sources { get; set; }
        public string[] HandlingHosts { get; set; }
        public DateTime? FromInclusive { get; set; }
        public DateTime? ToInclusive { get; set; }

        public PageFilter PageFilter { get; set; }

        protected override string[] ValidSortNames => validSortNames;
    }
}
