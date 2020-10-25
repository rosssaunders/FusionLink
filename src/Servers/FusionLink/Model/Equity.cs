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

        public DataTable VolatilityCallAsk => GetVolatility(eMVolatilityCurveType.M_vcCallAsk);

        public DataTable VolatilityCallBid => GetVolatility(eMVolatilityCurveType.M_vcCallBid);

        public DataTable VolatilityCallManagement => GetVolatility(eMVolatilityCurveType.M_vcCallManagement);

        public DataTable VolatilityCallMarket => GetVolatility(eMVolatilityCurveType.M_vcCallMarket);

        public DataTable VolatilityCallResult => GetVolatility(eMVolatilityCurveType.M_vcCallResult);

        public DataTable VolatilityEndOfList => GetVolatility(eMVolatilityCurveType.M_vcEndOfList);

        public DataTable VolatilityPutAsk => GetVolatility(eMVolatilityCurveType.M_vcPutAsk);

        public DataTable VolatilityPutBid => GetVolatility(eMVolatilityCurveType.M_vcPutBid);

        public DataTable VolatilityPutManagement => GetVolatility(eMVolatilityCurveType.M_vcPutManagement);

        public DataTable VolatilityPutMarket => GetVolatility(eMVolatilityCurveType.M_vcPutMarket);

        public DataTable VolatilityPutResult => GetVolatility(eMVolatilityCurveType.M_vcPutResult);

        private DataTable GetVolatility(eMVolatilityCurveType curveType)
        {
            using CSMEquity equity = CSMInstrument.GetInstance(code);
            var dt = new DataTable("Volatility");

            dt.Columns.Add("Maturity");
            dt.Columns.Add("Stike");
            dt.Columns.Add("Volatility");

            using (var volSurface = equity.GetVolatilitySurface())
            using (var atmCurve = volSurface.GetAtmVolatilityCurve(curveType))
            using (var smiles = atmCurve.GetSmiles())
            {
                //using var atmPoint
                for (var i = 0; i < atmCurve.GetPointCount(); i++)
                {
                    using var atmPoint = new SSMVolatilityPoint();
                    if (atmCurve.GetNthPoint(i, atmPoint))
                    {
                        using var smile = atmPoint.fSmile;

                        var maturity = DateToString(atmPoint.fMaturity, atmPoint.fType);

                        dt.Rows.Add(
                            maturity,
                            "ATM",
                            atmPoint.fVolatility);

                        if (smile is object)
                        {
                            for (var j = 0; j < smile.GetPointCount(); j++)
                            {
                                using var ssmPoint = new SSMSmilePoint();
                                if (smile.GetNthPoint(j, ssmPoint))
                                {
                                    dt.Rows.Add(
                                        maturity,
                                        ssmPoint.fStrike,
                                        ssmPoint.fVolatility);
                                }
                            }
                        }
                    }
                }
            }

            return dt;
        }

        private string DateToString(int maturity, eMVolatilityPointType type)
        {
            string text = "";
            if (type != eMVolatilityPointType.M_vpAbsoluteDate)
            {
                text += maturity;
                if (type == eMVolatilityPointType.M_vpDay)
                {
                    text += "d";
                }
                else if (type == eMVolatilityPointType.M_vpWeek)
                {
                    text += "w";
                }
                else if (type == eMVolatilityPointType.M_vpMonth)
                {
                    text += "m";
                }
                else
                {
                    text += "y";
                }
            }
            else
            {
                int date = CSMMarketData.GetCurrentMarketData().GetDate();
                using var csmday = new CSMDay(maturity + date);
                var dateTime = new DateTime((int)csmday.fYear, (int)csmday.fMonth, (int)csmday.fDay);
                text = dateTime.ToShortDateString();
            }
            return text;
        }
    }
}
