using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class DebtInstrument : Instrument
    {
        public DebtInstrument(int code) : base(code)
        {
        }

        public string AskQuotationType
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAskQuotationType().SophisEnumToString();
            }
        }

        public object DateDebut
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDateDebut().ToDateTime();
            }
        }

        public object DateFin
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDateFin().ToDateTime();
            }
        }

        public string DayCountBasisType
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDayCountBasisType().SophisEnumToString();
            }
        }

        public object Expiry
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiry().ToDateTime();
            }
        }

        public int FloatingRate
        {
            get
            {
                //TODO
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFloatingRate();
            }
        }

        public double InstrumentSpread
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInstrumentSpread();
            }
        }

        public object IssueDate
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssueDate().ToDateTime();
            }
        }

        public int IssuerCode
        {
            get
            {
                //TODO
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuerCode();
            }
        }

        public double Margin
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMargin();
            }
        }

        public string MarketAIDayCountBasisType
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketAIDayCountBasisType().SophisEnumToString();
            }
        }

        public string MarketBondPeriodicityType
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketBondPeriodicityType().SophisEnumToString();
            }
        }

        public bool MarketCalculationYTMOnAdjustedDates
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketCalculationYTMOnAdjustedDates();
            }
        }

        public bool MarketCalculationYTMOnSettlementDate
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketCalculationYTMOnSettlementDate();
            }
        }

        public string MarketCSDayCountBasisType
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketCSDayCountBasisType().SophisEnumToString();
            }
        }

        public string MarketYTMDayCountBasisType
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketYTMDayCountBasisType().SophisEnumToString();
            }
        }

        public string MarketYTMYieldCalculationType
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketYTMYieldCalculationType().SophisEnumToString();
            }
        }

        public double Notional
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotional();
            }
        }

        public double NotionalRate
        {

            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotionalRate();
            }
        }

        public bool PaymentCouponsAtTheEndOfTheDay
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPaymentCouponsAtTheEndOfTheDay();
            }
        }

        public double Quotity
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public double Rate
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRate();
            }
        }

        public double Spread
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSpread();
            }
        }

        public string SpreadType
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSpreadType().SophisEnumToString();
            }
        }

        public object StartDate
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStartDate().ToDateTime();
            }
        }

        public string YieldCalculationType
        {
            get
            {
                using CSMDebtInstrument instrument = CSMInstrument.GetInstance(code);
                return instrument.GetYieldCalculationType().SophisEnumToString();
            }
        }
    }
}
