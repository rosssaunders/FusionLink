using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class Future : Instrument
    {
        public Future(int code) : base(code)
        {
        }

        public double Basis
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBasis();
            }
        }

        public double Cost
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetClearingFees();
            }
        }

        public double ConversionRatioInPrice
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetConversionRatioInPrice();
            }
        }

        public double ConversionRatioInProduct
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetConversionRatioInProduct();
            }
        }

        public object DeliveryDate
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDeliveryDate().ToDateTime();
            }
        }

        public string DeliveryType
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDeliveryType().SophisEnumToString();
            }
        }

        public object Maturity
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiry().ToDateTime();
            }
        }

        public object FinalSettlement
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFinalSettlement().ToDateTime();
            }
        }

        public object FRAEffectiveDate
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFRAEffectiveDate().ToDateTime();
            }
        }

        public object FRAEndDate
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFRAEndDate().ToDateTime();
            }
        }

        public override string Market
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetListedMarketId().GetListedMarketName();
            }
        }

        public int MarketCode
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetListedMarketId();
            }
        }

        public double Notional
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotional();
            }
        }

        public double PointValue
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public double ContractSize
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public int UnderlyingCode
        {
            get
            {
                using CSMFuture instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnderlyingCode();
            }
        }
    }
}