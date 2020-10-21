using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;
using sophis.market_data;
using sophis.static_data;
using Sophis.Utils;
using sophisTools;

namespace RxdSolutions.FusionLink.Model
{
    public class Equity : Instrument
    {
        public Equity(int code) : base(code)
        {
        }

        public double Beta
        {
            get
            {
                using CSMEquity equity = CSMInstrument.GetInstance(code);
                return equity.GetBeta();
            }
        }

        public double TradingUnits
        {
            get
            {
                using CSMEquity equity = CSMInstrument.GetInstance(code);
                return equity.GetTradingUnits();
            }
        }

        public object AccountingReferenceDate
        {
            get
            {
                using CSMEquity equity = CSMInstrument.GetInstance(code);
                return equity.GetAccountingReferenceDate().ToDateTime();
            }
        }

        public double SharesOutstanding
        {
            get
            {
                using CSMEquity equity = CSMInstrument.GetInstance(code);
                return equity.GetInstrumentCount();
            }
        }

        public int IssuerCode
        {
            get
            {
                using CSMEquity equity = CSMInstrument.GetInstance(code);
                return equity.GetIssuerCode();
            }
        }

        public DataTable RepoMargins
        {
            get
            {
                using CSMEquity equity = CSMInstrument.GetInstance(code);
                var dt = new DataTable(nameof(RepoMargins));
                dt.Columns.Add("Maturity");
                dt.Columns.Add("Bid");
                dt.Columns.Add("Ask");
                dt.Columns.Add("Cost");

                for (int i = 0; i <= equity.GetRepoMarginCount(); i++)
                {
                    var rmObj = new SSMRepoMargin();
                    if(equity.GetNthRepoMargin(i, rmObj))
                    {
                        var maturity = sophis.value.CSMAmDateUtils.DateToString(rmObj.fMaturity);

                        dt.Rows.Add(
                            maturity,
                            rmObj.fBid,
                            rmObj.fAsk,
                            rmObj.fCost);
                    }
                }

                return dt;
            }
        }
    }
}
