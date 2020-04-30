using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{

    public class ForexFuture : Instrument
    {
        public ForexFuture(int code) : base(code)
        {
        }

        public string AskQuotationType
        {
            get
            {
                using CSMForexFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAskQuotationType().SophisEnumToString();
            }
        }

        public object Expiry
        {
            get
            {
                using CSMForexFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiry().ToDateTime();
            }
        }

        public int ExpiryCurrency
        {
            get
            {
                using CSMForexFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiryCurrency();
            }
        }

        public int ExpiryInProduct
        {
            get
            {
                using CSMForexFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiryInProduct();
            }
        }

        public double Quotity
        {
            get
            {
                using CSMForexFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public double SpotDefaultForPurchase
        {
            get
            {
                using CSMForexFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSpotDefaultForPurchase();
            }
        }

        public string UnrealizedMethod
        {
            get
            {
                using CSMForexFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnrealizedMethod().SophisEnumToString();
            }
        }
    }
}
