using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class CapFloor : Instrument
    {
        public CapFloor(int code) : base(code)
        {
        }

        public object BrokenDate
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBrokenDate().ToDateTime();
            }
        }

        public string DateReference
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBrokenDateReference().SophisEnumToString();
            }
        }

        public double GetBSVolatility
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBSVolatility();
            }
        }

        public string FloorType
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCapFloorType().SophisEnumToString();
            }
        }

        public int SpecificVolatility
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCodeForSpecificVolatility();
            }
        }

        public string ForPayment
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCSRCalendarForPayment().GetName();
            }
        }

        public string ForRolling
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCSRCalendarForRolling().GetName();
            }
        }

        public string MetaModel
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);

#if SOPHIS713
                return instrument.GetCurrentMetaModel().GetName();
#endif

#if SOPHIS2021
                return instrument.GetCurrentPricer().GetName();
#endif
            }
        }

        public object DataDate
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDataDate().ToDateTime();
            }
        }

        public string DatesAdjustment
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDatesAdjustment().SophisEnumToString();
            }
        }

        public string Basis
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDayCountBasisType().SophisEnumToString();
            }
        }

        public object EndDate
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetEndDate().ToDateTime();
            }
        }

        public object StartDate
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStartDate().ToDateTime();
            }
        }

        public string Timing
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFixingDateRef().SophisEnumToString();
            }
        }

        public int FixingOffset
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFixingOffset();
            }
        }

        public double Notional
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotional();
            }
        }

        public int FloatingRate
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFloatingRate();
            }
        }

        public string AdjustmentType
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHolidayAdjustmentType().SophisEnumToString();
            }
        }

        public double VolatilityAccuracy
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetImpliedVolatilityAccuracy();
            }
        }

        public int IndexCode
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInflationIndexCode();
            }
        }

        public int GetJplus
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetJplus();
            }
        }

        public double MarketPriceInPercent
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketPriceInPercent();
            }
        }

        public string Settlement
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPaymentMethod().SophisEnumToString();
            }
        }

        public string PayoffType
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                using var payoffType = instrument.GetPayoffType();
                return payoffType.StringValue;
            }
        }

        public string PeriodicPayment
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPeriodicPayment().SophisEnumToString();
            }
        }

        public string QuotationType
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotationType().SophisEnumToString();
            }
        }

        public int RoundingDecimalNumber
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRoundingDecimalNumber();
            }
        }

        public string MethodType
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRoundingMethodType().SophisEnumToString();
            }
        }

        public string UnderlyingCategory
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnderlyingCategory().SophisEnumToString();
            }
        }

        public int UnderlyingCode
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnderlyingCode();
            }
        }

        public string DependencyType
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetVolatilityDependencyType().SophisEnumToString();
            }
        }

        public string Frequency
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPeriodicPayment().SophisEnumToString();
            }
        }

        public double Strike
        {
            get
            {
                using CSMCapFloor instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg().GetStrikeInProduct();
            }
        }
    }
}
