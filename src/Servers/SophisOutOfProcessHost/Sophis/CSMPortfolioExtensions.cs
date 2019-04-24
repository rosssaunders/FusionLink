using sophis.utils;

namespace sophis.portfolio
{
    public static class CSMPortfolioExtensions
    {
        public static string GetName(this CSMPortfolio portfolio)
        {
            using (CMString name = new CMString())
            {
                portfolio.GetName(name);
                return name.GetString();
            }
        }
    }
}
