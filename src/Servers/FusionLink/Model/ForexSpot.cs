using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class ForexSpot : Instrument
    {
        public ForexSpot(int code) : base(code)
        { }

        public int Accuracy
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAccuracy();
            }
        }

        public double Ask
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAsk();
            }
        }

        public string AskQuotationType
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAskQuotationType().SophisEnumToString();
            }
        }

        public double Bid
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBid();
            }
        }

        public int Forex1
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetForex1();
            }
        }

        public int Forex2
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetForex2();
            }
        }

        public int FxPair
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                using var fxPair = instrument.GetFxPair();
                return fxPair.fSico;
            }
        }

        public double MarketQuotity
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketQuotity();
            }
        }

        public int MarketWay
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketWay();
            }
        }

        public double Quotity
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public int SettlementCurrency
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSettlementCurrency();
            }
        }

        public double SpotDefaultForPurchase
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSpotDefaultForPurchase();
            }
        }

        public bool ReportingBySettlementDate
        {
            get
            {
                using CSMForexSpot instrument = CSMInstrument.GetInstance(code);
                return instrument.ReportingBySettlementDate();
            }
        }
    }
}
