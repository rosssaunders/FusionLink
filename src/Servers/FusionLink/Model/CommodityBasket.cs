using RxdSolutions.FusionLink.Helpers;
using sophis.commodity;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class CommodityBasket : Instrument
    {
        public CommodityBasket(int code) : base(code)
        {
        }

        public string CommodityType
        {
            get
            {
                using CSMCommodityBasket instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommodityType().SophisEnumToString();
            }
        }
    }
}
