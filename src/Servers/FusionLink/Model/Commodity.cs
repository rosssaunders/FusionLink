using sophis.commodity;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class Commodity : Instrument
    {
        public Commodity(int code) : base(code)
        {
        }

        public double QuotationTick
        {
            get
            {
                using CSMCommodity instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotationTick();
            }
        }

        public int RoundingAvg
        {
            get
            {
                using CSMCommodity instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRoundingAvg();
            }
        }

        public double UnitOfTrading
        {
            get
            {
                using CSMCommodity instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }
    }
}
