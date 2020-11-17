using sophis.backoffice_kernel;
using sophis.instrument;
using sophis.market_data;
using sophis.static_data;
using sophis.utils;

namespace RxdSolutions.FusionLink.Model
{
    public class Currency
    {
        protected readonly int code;

        public Currency(int code)
        {
            this.code = code;
        }

        public string Name
        {
            get
            {
                using var currency = CSMCurrency.GetCSRCurrency(code);
                using var name = new CMString();
                currency.GetName(name);
                return name.StringValue;
            }
        }

        [MarketData]
        public double Last
        {
            get
            {
                return CSMCurrency.GetCSRCurrency(code).GetSpot();
            }
        }
    }
}
