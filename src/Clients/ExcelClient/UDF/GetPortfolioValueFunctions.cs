using ExcelDna.Integration;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetPortfolioValueFunctions
    {
        [ExcelFunction(Name = "GETPORTFOLIOVALUE",
                       Description = "Returns a portfolio cell value",
                       HelpTopic = "Get-Portfolio-Value")]
        public static object GetPortfolioValue(int portfolioId, string column)
        {
            return ExcelAsyncUtil.Observe(nameof(GetPortfolioValue), new object[] { portfolioId, column }, () => new PortfolioValueExcelObservable(portfolioId, column, AddIn.Client));
        }

        [ExcelFunction(Name = "PRV",
                       Description = "Returns a portfolio cell value",
                       HelpTopic = "Get-Portfolio-Value")]
        public static object PRV(int portfolioId, string column)
        {
            return GetPortfolioValue(portfolioId, column);
        }

        [ExcelFunction(Name = "GETPORTFOLIODATE",
                       Description = "Returns the Portfolio Date of FusionInvest",
                       HelpTopic = "Get-Portfolio-Date")]
        public static object GetPortfolioDate()
        {
            return ExcelAsyncUtil.Observe(nameof(GetPortfolioDate), null, () => new SystemPropertyExcelObservable(SystemProperty.PortfolioDate, AddIn.Client));
        }
    }
}
