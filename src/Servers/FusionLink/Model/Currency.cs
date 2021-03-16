using System.Data;
using RxdSolutions.FusionLink.Helpers;
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

#if SOPHIS713
                using var name = new CMString();
                currency.GetName(name);
#endif

#if SOPHIS2021
                using var name = currency.GetName();
#endif

                return name.StringValue;
            }
        }

        public bool InverseRic
        {
            get
            {
                using var currency = CSMCurrency.GetCSRCurrency(code);
                return currency.InverseRic(); ;
            }
        }

        public DataTable Holidays
        {
            get
            {
                using var currency = CSMCurrency.GetCSRCurrency(code);

                var dt = new DataTable()
                {
                    TableName = "BankHolidays"
                };
                dt.Columns.Add("Date");

                var numBankHolidays = currency.GetBankHolidayDayCount();
                for(var i = 0; i < numBankHolidays; i++)
                {
                    dt.Rows.Add(currency.GetNthBankHolidayDay(i).ToDateTime());
                }

                return dt;
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
