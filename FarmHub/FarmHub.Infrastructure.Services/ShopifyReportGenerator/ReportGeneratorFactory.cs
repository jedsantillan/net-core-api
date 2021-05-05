using System.Dynamic;

namespace FarmHub.Domain.Services.ShopifyReportGenerator
{
    public enum ShopifyReportType
    {
        MasterList = 1,
        RidersComm,
        Totals
    }

    public interface IReportGeneratorFactory
    {
        public IReportGenerator<ExpandoObject> Create(ShopifyReportType type);
    }

    public class ReportGeneratorFactory : IReportGeneratorFactory
    {
        public IReportGenerator<ExpandoObject> Create(ShopifyReportType type)
        {
            return type switch
            {
                ShopifyReportType.Totals => new TotalsReportGenerator(),
                ShopifyReportType.RidersComm => new RidersCommReportGenerator(),
                _ => new MasterListReportGenerator()
            };
        }
    }
}