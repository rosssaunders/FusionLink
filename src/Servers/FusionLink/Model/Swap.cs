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
    public class Swap : Instrument
    {
        public Swap(int code) : base(code)
        {
        }

        public string AskQuotationType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAskQuotationType().SophisEnumToString();
            }
        }

        public int CalculationAgent
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCalculationAgent();
            }
        }

        public int CodeForSpecificVolatility
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCodeForSpecificVolatility();
            }
        }

        public string CurrencyCode
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCurrencyCode().GetCurrencyCode();
            }
        }

        public string DTCCConvention
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                using var convention = instrument.GetDTCCConvention();
                return convention.StringValue;
            }
        }

        public object EndDate
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetEndDate().ToDateTime();
            }
        }

        public object Expiry
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiry().ToDateTime();
            }
        }

        public int Family
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFamily();
            }
        }

        public string FamilyName
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFamily().GetYieldCurveName();
            }
        }

        public double FamilySpread
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFamilySpread();
            }
        }

        public string HolidayAdjustmentType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHolidayAdjustmentType().SophisEnumToString();
            }
        }

        public double InstrumentSpread
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInstrumentSpread();
            }
        }

        public int InterestRateSwapFloatingRate
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInterestRateSwapFloatingRate();
            }
        }

        public object IssueDate
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssueDate().ToDateTime();
            }
        }

        public int IssuerCode
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuerCode();
            }
        }

        public int MarketCode
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketCode();
            }
        }

        public string ModelName
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                using var model = new CMString();
                instrument.GetModelName(model);
                return model.StringValue;
            }
        }

        public double Notional
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotional();
            }
        }

        public string NotionalExchangeType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotionalExchangeType().SophisEnumToString();
            }
        }

        public double NotionalInProduct
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotionalInProduct();
            }
        }

        public string PaymentGapType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPaymentGapType().SophisEnumToString();
            }
        }

        public double Quotity
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public string RollingType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRollingType().SophisEnumToString();
            }
        }

        public int Seniority
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSeniority();
            }
        }

        public int SettlementAtDPlus
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSettlementAtDPlus();
            }
        }

        public string SettlementCurrency
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSettlementCurrency().GetCurrencyCode();
            }
        }

        public string SpreadType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSpreadType().SophisEnumToString();
            }
        }

        public object StartDate
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStartDate().ToDateTime();
            }
        }

        public int StrikeUnit
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStrikeUnit();
            }
        }

        public int Unit
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnit();
            }
        }

        public string VolatilityDependencyType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetVolatilityDependencyType().SophisEnumToString();
            }
        }

        object GetFixedRate(CSMLeg leg)
        {
            CSMFixedLeg fl = leg; return fl is object ? (double?)fl.GetSettlementCurrencyType() : null;
        }

        public int ReceivingLegFixedRate
        {
            get
            {
                //TODO
                return 0;
                //".ToUpper(), x => GetFixedRate(x.GetLeg(0)));
            }
        }


        public int PayingLegFixedRate 
        {
            get 
            {
                //TODO
                return 0;
                //".ToUpper(), x => GetFixedRate(x.GetLeg(1)));
            }
        }

        public string ReceivingLegFixedSettlement
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetTypeOf().SophisEnumToString();
            }
        }

        public string ReceivingLegPayoff
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetTypeOf().SophisEnumToString();
            }
        }

        public int ReceivingLegUnderlyingCode
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetUnderlyingCode();
            }
        }

        public string ReceivingLegAdjustmentMethod
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetAdjustmentMethod().SophisEnumToString();
            }
        }

        public double ReceivingLegAutocallTrigger
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetAutocallTrigger();
            }
        }

        public int ReceivingLegBrokenDate
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetBrokenDate();
            }
        }

        public string ReceivingLegBrokenDateReference
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetBrokenDateReference().SophisEnumToString();
            }
        }

        public string ReceivingLegCurrency
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetCurrencyCode().GetCurrencyCode();
            }
        }

        public string ReceivingLegDayCountBasisType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetDayCountBasisType().SophisEnumToString();
            }
        }

        public string ReceivingLegDeliveryCurrency
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetDeliveryCurrency().GetCurrencyCode();
            }
        }

        public int ReceivingLegFloatingRate
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetFloatingRate();
            }
        }

        public int ReceivingLegForexFixing
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetForexFixing();
            }
        }

        public int ReceivingLegForexFixingLag
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetForexFixingLag();
            }
        }

        public string ReceivingLegForexOrder
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(0).GetForexOrder().SophisEnumToString();
            }
        }

        public string ReceivingLegPaymentRule
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                using var paymentRule = instrument.GetLeg(0).GetPaymentRule();
                return paymentRule.StringValue;
            }
        }

        public string PayingLegPayoff
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetTypeOf().SophisEnumToString();
            }
        }

        public int PayingLegUnderlyingCode
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetUnderlyingCode();
            }
        }

        public string PayingLegAdjustmentMethod
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetAdjustmentMethod().SophisEnumToString();
            }
        }

        public double PayingLegAutocallTrigger
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetAutocallTrigger();
            }
        }

        public int PayingLegBrokenDate
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetBrokenDate();
            }
        }

        public string PayingLegBrokenDateReference
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetBrokenDateReference().SophisEnumToString();
            }
        }

        public string PayingLegCurrency
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetCurrencyCode().GetCurrencyCode();
            }
        }

        public string PayingLegDayCountBasisType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetDayCountBasisType().SophisEnumToString();
            }
        }

        public string PayingLegDeliveryCurrency
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetDeliveryCurrency().GetCurrencyCode();
            }
        }

        public int PayingLegFloatingRate
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetFloatingRate();
            }
        }

        public int PayingLegForexFixing
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetForexFixing();
            }
        }

        public int PayingLegForexFixingLag
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetForexFixingLag();
            }
        }

        public string PayingLegForexOrder
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetForexOrder().SophisEnumToString();
            }
        }

        public string PayingLegPeriodicityType
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLeg(1).GetPeriodicityType().SophisEnumToString();
            }
        }

        public string PayingLegPaymentRule
        {
            get
            {
                using CSMSwap instrument = CSMInstrument.GetInstance(code);
                using var paymentRule = instrument.GetLeg(1).GetPaymentRule();
                return paymentRule.StringValue;
            }
        }
    }
}
