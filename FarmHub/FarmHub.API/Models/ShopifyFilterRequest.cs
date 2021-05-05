using System;
using FarmHub.Domain.Services.ShopifyReportGenerator;

namespace FarmHub.API.Models
{
    public class ShopifyFilterRequest
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public ShopifyReportType ReportType { get; set; }
        public string[]? Tags { get; set; }
        public long?[]? ProductIds { get; set; }
        public string[]? PromoCodes { get; set; }
        public string? FileName { get; set; }
        public int? OrderIdStart { get; set; }
        public int? OrderIdEnd { get; set; }
        public bool? TaggedOnly { get; set; } = true;
    }
}