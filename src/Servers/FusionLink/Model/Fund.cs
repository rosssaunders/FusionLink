using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;
using sophis.value;

namespace RxdSolutions.FusionLink.Model
{
    public class Fund : Instrument
    {
        public Fund(int code) : base(code)
        {

        }

        public int Administrator
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAdministrator();
            }
        }

        public int Entity
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetEntity();
            }
        }

        public int EODOffset
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetEODOffset();
            }
        }

        public int FactorModel
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFactorModel();
            }
        }

        public int FeesPortfolio
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFeesPortfolio();
            }
        }

        public int FundCashPortfolio
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFundCashPortfolio();
            }
        }

        public int InvestmentPortfolio
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInvestmentPortfolio();
            }
        }

        public int PortfolioUnderlying
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPortfolioUnderlying();
            }
        }

        public int SRPortfolio
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSRPortfolio();
            }
        }

        public int TradingPortfolio
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetTradingPortfolio();
            }
        }

        public double Tolerance
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetTolerance();
            }
        }

        public string ToleranceType
        {
            get
            {
                using CSMAmFund instrument = CSMInstrument.GetInstance(code);
                return instrument.GetToleranceType().SophisEnumToString();
            }
        }
    }
}
