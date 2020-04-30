using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;
using sophis.static_data;
using sophis.utils;

namespace RxdSolutions.FusionLink.Model
{
    public class InterestRate : Instrument
    {
        public InterestRate(int code) : base(code)
        {
        }

        public int AverageRate
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAverageRate();
            }
        }

        public string CalendarCalculation
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                using var calendar = instrument.GetCalendarCalculation();
                using var name = new CMString();
                calendar.GetName(name);
                return name.StringValue;
            }
        }

        public int CalendarPlace
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCalendarPlace();
            }
        }

        public int CapitalizedRoundingDigits
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCapitalizedRoundingDigits();
            }
        }

        public string CapitalizedRoundingMethod
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCapitalizedRoundingMethod().SophisEnumToString();
            }
        }

        public int CodeForSpecificVolatility
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCodeForSpecificVolatility();
            }
        }

        public string DayCountBasisType
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDayCountBasisType().SophisEnumToString();
            }
        }

        public int DecimalCount
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDecimalCount();
            }
        }

        public object Expiry
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiry().ToDateTime();
            }
        }

        public int FixingPlace
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFixingPlace();
            }
        }

        public string IndexRate
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                using var name = new CMString();
                instrument.GetIndexRate(name);
                return name.StringValue;
            }
        }

        public int InflationIndex
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInflationIndex();
            }
        }

        public int InflationRule
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInflationRule();
            }
        }

        public string InterestRateAverageType
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInterestRateAverageType().SophisEnumToString();
            }
        }

        public string InterestRatePeriodicityType
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInterestRatePeriodicityType().SophisEnumToString();
            }
        }

        public string InterestRateType
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInterestRateType().SophisEnumToString();
            }
        }

        public double LiborMultiplier
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLiborMultiplier();
            }
        }

        public int MarketCode
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketCode();
            }
        }

        public string Maturity
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                using var maturity = instrument.GetMaturity();
                return maturity.fMaturity.ToString() + maturity.fType.ToString();
            }
        }

        public int SearchAtDayMinus
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSearchAtDayMinus();
            }
        }

        public int ShortIndex
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetShortIndex();
            }
        }

        public double Spread
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSpread();
            }
        }

        public string SwapPeriodicityType
        {
            get
            {
                using CSMInterestRate instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSwapPeriodicityType().SophisEnumToString();
            }
        }
    }
}
