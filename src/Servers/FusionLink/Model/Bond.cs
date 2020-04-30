using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{

    public class Bond : Instrument
    {
        public Bond(int code) : base(code)
        {
        }

        public double Notional
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotional();
            }
        }

        public string NotionalCurrency
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotionalCurrency().GetCurrencyCode();
            }
        }

        public object IssueDate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssueDate().ToDateTime();
            }
        }

        public object Maturity
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMaturity().ToDateTime();
            }
        }

        public int IssuerCode
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuerCode();
            }
        }

        public string Seniority
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSeniority().GetSeniorityName();
            }
        }

        public int GuarantorCode
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetGuarantorCode();
            }
        }

        public int ReferenceTreasuryCode
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetReferenceTreasuryCode();
            }
        }

        public string FixedRateFrequency
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketBondPeriodicityType().SophisEnumToString();
            }
        }

        public string FixedRateBasis
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketAIDayCountBasisType().SophisEnumToString();
            }
        }

        public string FixedRateMode
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetYieldCalculationType().SophisEnumToString();
            }
        }

        public double FixedRate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotionalRate();
            }
        }

        public string CashRoundingRule
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRoundingRule().SophisEnumToString();
            }
        }

        public string CashRoundingType
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRoundingType().SophisEnumToString();
            }
        }

        public object CashRoundingAmount
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRoundingAmount().ConvertSophisNumber();
            }
        }

        public double NumberOfSecurities
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInstrumentCount();
            }
        }

        public string QuotationType
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotationType().SophisEnumToString();
            }
        }

        public bool RangeAccrual
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRangeAccrual() != null;
            }
        }

        public string ExoticUnderlyingType
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExoticUnderlyingType().SophisEnumToString();
            }
        }

        public string BankHolidaysRule
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHolidayAdjustmentType().SophisEnumToString();
            }
        }

        public int SettlementShift
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPaymentShift();
            }
        }

        public int OwnershipShift
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSettlementShift();
            }
        }

        public string OwnershipType
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBondOwnershipType().SophisEnumToString();
            }
        }

        public object FirstSettlementDate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFirstSettlementDate().ToDateTime();
            }
        }

        public bool ComputationOnAdjustedDate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketCalculationAIOnAdjustedDates();
            }
        }

        public bool CouponsOnAdjustedDate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketBondCouponAdjusted();
            }
        }

        public object FirstCouponDate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFirstCouponDate().ToDateTime();
            }
        }

        public object LastButOneCouponDate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLastButOneCouponDate().ToDateTime();
            }
        }

        public object StepStartDate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRedemptionStepStartDate().ToDateTime();
            }
        }

        public object StepEndDate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRedemptionStepEndDate().ToDateTime();
            }
        }

        public string StepType
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRedemptionStepType().SophisEnumToString();
            }
        }

        public double StepValue
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRedemptionStepValue();
            }
        }

        public double MinimumPiece
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMinTradingSize();
            }
        }

        public double LowestIncrement
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetTradingUnits();
            }
        }

        public int CouponSettlementLag
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCouponSettlementLag();
            }
        }

        public string CouponSettlementLagType
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCouponSettlementLagType().SophisEnumToString();
            }
        }

        public string CouponSettlementCurrency
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDeliveryCurrency().GetCurrencyCode();
            }
        }

        public object CalledDate
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCalledDate().ToDateTime();
            }
        }

        public double CalledPrice
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCalledPrice();
            }
        }

        public int NumberOfPartialRedemptions
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNumberOfPartialRedemptions();
            }
        }

        public double PartialRedeemPercentage
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRedeemPercentage();
            }
        }

        public string RedemptionDistribution
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPartialRedemptionType().GetPartialPaymentMethod();
            }
        }

        public string CancellationType
        {
            get
            {
                using CSMBond instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCancellationType().SophisEnumToString();
            }
        }
    }
}
