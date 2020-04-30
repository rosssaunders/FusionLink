using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;
using sophis.utils;

namespace RxdSolutions.FusionLink.Model
{

    public class Option : Instrument
    {
        public Option(int code) : base(code)
        {

        }

        public int UnderlyingCode
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnderlyingCode();
            }
        }

        public string Delivery
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDeliveryType().SophisEnumToString();
            }
        }

        public string PaymentCurrency
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStrikeCurrency().GetCurrencyCode();
            }
        }

        public string Quotation
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSettlementCurrency().GetCurrencyCode();
            }
        }

        public string QuotationUnit
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAskQuotationType().SophisEnumToString();
            }
        }

        public int CalculationAgent
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCalculationAgent();
            }
        }


        //public int FixingType{ get { using CSMOption instrument = CSMInstrument.GetInstance(code); return instrument.GetFixingType(); } } --THIS RETURNS A NUMBER THAT ISNT USEFUL
        //public int GuarrantedFX{ get { using CSMOption instrument = CSMInstrument.GetInstance(code); return instrument.GetFX);

        public double GuaranteedFX
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetGuarrantedForexSpot();
            }
        }

        public int AlreadyDownCount
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAlreadyDownCount();
            }
        }

        public string AverageType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAverageType().SophisEnumToString();
            }
        }

        public int BarrierCount
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBarrierCount();
            }
        }

        public object CalledDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCalledDate().ToDateTime();
            }
        }

        public double CalledPrice
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCalledPrice();
            }
        }

        public bool CalledPriceInPercent
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCalledPriceInPercent();
            }
        }

        public double CallValueInTree
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCallValueInTree();
            }
        }

        public double ConversionRatioInPrice
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetConversionRatioInPrice();
            }
        }

        public double ConversionRatioInProduct
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetConversionRatioInProduct();
            }
        }

        public double ConversionRatioInShares
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetConversionRatioInShares();
            }
        }

        public double ConversionValueInTree
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetConversionValueInTree();
            }
        }

        public string CouponFrequencyForCB
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCouponFrequencyForCB().SophisEnumToString();
            }
        }

        public double CreditSpread
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCreditSpread();
            }
        }

        public bool CreditSpreadForDerivative
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCreditSpreadForDerivative();
            }
        }

        public string CreditSpreadValuationTypeForCB
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCreditSpreadValuationTypeForCB().SophisEnumToString();
            }
        }

        public string DayCountBasisTypeForCB
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDayCountBasisTypeForCB().SophisEnumToString();
            }
        }

        public string DerivativeType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDerivativeType().SophisEnumToString();
            }
        }

        public int DigitalBasisType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDigitalBasisType();
            }
        }

        public object DigitalStartDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDigitalStartDate().ToDateTime();
            }
        }

        public string DividendInTaxCreditType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDividendInTaxCreditType().SophisEnumToString();
            }
        }

        public object ExerciseEndDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExerciseEndDate().ToDateTime();
            }
        }

        public int ExercisePeriodCount
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExercisePeriodCount();
            }
        }

        public object ExerciseStartDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExerciseStartDate().ToDateTime();
            }
        }

        public string ExerciseType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExerciseType().SophisEnumToString();
            }
        }

        public object Expiry
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiry().ToDateTime();
            }
        }

        public int ExpiryForRho
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiryForRho();
            }
        }

        public object FinalSettlement
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFinalSettlement().ToDateTime();
            }
        }

        public double FixedVolatility
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFixedVolatility();
            }
        }

        public int FloatingRate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFloatingRate();
            }
        }

        public string GamaType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetGamaType().SophisEnumToString();
            }
        }

        public int GapBetween2Fixings
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetGapBetween2Fixings();
            }
        }

        public double GuaranteedForexSpot
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetGuarrantedForexSpot();
            }
        }

        public string HolidayAdjustmentType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHolidayAdjustmentType().SophisEnumToString();
            }
        }

        public string ImpliedCreditSpreadValuationTypeForCB
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetImpliedCreditSpreadValuationTypeForCB().SophisEnumToString();
            }
        }

        public bool IsInAmount
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIsInAmount();
            }
        }

        public object IssueDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssueDate().ToDateTime();
            }
        }

        public double IssuePrice
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuePrice();
            }
        }

        public int IssuerCode
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuerCode();
            }
        }

        public string ModelName
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                using var model = new CMString();
                instrument.GetModelName(model);
                return model.StringValue;
            }
        }
        
        public bool NMUFlag
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNMUFlag();
            }
        }

        public double Notional
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotional();
            }
        }

        public double NotionalInProduct
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotionalInProduct();
            }
        }

        public int NumberOfFoliages
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNumberOfFoliages();
            }
        }

        public double NumberSharesPhysicallyDelivered
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNumberSharesPhysicallyDelivered();
            }
        }

        public string OptimisationType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetOptimisationType().SophisEnumToString();
            }
        }

        public string OptionFlagType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetOptionFlagType().SophisEnumToString();
            }
        }

        public object OptionStartDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetOptionStartDate().ToDateTime();
            }
        }

        public object OptionStartRelativeDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetOptionStartRelativeDate().ToDateTime();
            }
        }

        public double OverVolatility
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetOverVolatility();
            }
        }

        public double Parity
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetParity();
            }
        }

        public double ParityStockInTree
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetParityStockInTree();
            }
        }

        public int PayOffClauseCount
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPayOffClauseCount();
            }
        }

        public string PayOffFormula
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                using var formula = new CMString();
                instrument.GetPayOffFormula(formula);
                return formula.StringValue;
            }
        }

        public double Proportion
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetProportion();
            }
        }

        public double Quotity
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public string Seniority
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSeniority().GetSeniorityName();
            }
        }

        public string SmileInDeltaGamma
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSmileInDeltaGamma().SophisEnumToString();
            }
        }

        public int SophisOptionModel
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSophisOptionModel();
            }
        }

        public double Spot
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSpot();
            }
        }

        public string SpreadType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSpreadType().SophisEnumToString();
            }
        }

        public object StartDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStartDate().ToDateTime();
            }
        }

        public int SwaptionFamily
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSwaptionFamily();
            }
        }

        public int SwaptionIndex
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSwaptionIndex();
            }
        }

        public string SwaptionInterestBasis
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSwaptionInterestBasis().SophisEnumToString();
            }
        }

        public string SwaptionInterestMode
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSwaptionInterestMode().SophisEnumToString();
            }
        }

        public double SwaptionNotional
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSwaptionNotional();
            }
        }

        public object SwaptionSwapEndDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSwaptionSwapEndDate().ToDateTime();
            }
        }

        public object SwaptionSwapStartDate
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSwaptionSwapStartDate().ToDateTime();
            }
        }

        public double TradingUnits
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetTradingUnits();
            }
        }

        public int Unit
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnit();
            }
        }

        public bool Validation
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetValidation();
            }
        }

        public string VolatilityType
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetVolatilityType().SophisEnumToString();
            }
        }

        public string YieldCalculationTypeForCB
        {
            get
            {
                using CSMOption instrument = CSMInstrument.GetInstance(code);
                return instrument.GetYieldCalculationTypeForCB().SophisEnumToString();
            }
        }
    }
}
