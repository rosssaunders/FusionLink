using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class Commission : Instrument
    {
        public Commission(int code) : base(code)
        {
        }

        public string BusinessEvent
        {
            get
            {
                using CSMCommission instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBEType().GetBusinessEventType();
            }
        }

        public int DepositoryCode
        {
            get
            {
                using CSMCommission instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBrokerCode();
            }
        }

        public string Depository
        {
            get
            {
                using CSMCommission instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBrokerCode().GetThirdPartyReference();
            }
        }

        public int Rate
        {
            get
            {
                using CSMCommission instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFloatingRate();
            }
        }
    }
}
