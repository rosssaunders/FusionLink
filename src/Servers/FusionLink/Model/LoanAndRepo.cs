using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class LoanAndRepo : Instrument
    {
        public LoanAndRepo(int code) : base(code)
        {
        }

        public bool AllInRate
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAllInRate();
            }
        }

        public int AveragePriceComputationType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAveragePriceComputationType();
            }
        }

        public int BillingCurrency
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBillingCurrency();
            }
        }

        public int CollateralCode
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCollateralCode();
            }
        }

        public double CollateralQuantity
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCollateralQuantity();
            }
        }

        public double CollateralSpot
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCollateralSpot();
            }
        }

        public string CollateralType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCollateralType().SophisEnumToString();
            }
        }

        public string CommissionAccrualMode
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionAccrualMode().SophisEnumToString();
            }
        }

        public double CommissionAmount
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionAmount();
            }
        }

        public int CommissionCurrency
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionCurrency();
            }
        }

        public string CommissionMode
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionMode().SophisEnumToString();
            }
        }

        public double CommissionQuotity
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionQuotity();
            }
        }

        public string CommissionType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionType().SophisEnumToString(); 
            }
        }

        public double CouponCollateral
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCouponCollateral();
            }
        }

        public string CreationMethod
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCreationMethod().SophisEnumToString();
            }
        }

        public int CurrencyQuotationAvailable
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCurrencyQuotationAvailable();
            }
        }

        public string DayCountBasisType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDayCountBasisType().SophisEnumToString(); 
            }
        }

        public string DeliveryType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDeliveryType().SophisEnumToString();
            }
        }

        public double DividendRate
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDividendRate();
            }
        }

        public object EndDateInProduct
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetEndDateInProduct().ToDateTime();
            }
        }

        public object ExerciseDate
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExerciseDate().ToDateTime();
            }
        }

        public object Expiry
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiry().ToDateTime();
            }
        }

        public double FactorForRealised
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFactorForRealised();
            }
        }

        public int FloatingRateOnCollateral
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFloatingRateOnCollateral();
            }
        }

        public int FloatingRateOnCommission
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFloatingRateOnCommission();
            }
        }

        public string ForexOrder
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetForexOrder().SophisEnumToString();
            }
        }

        public double ForexValue
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetForexValue();
            }
        }

        public double HairCut
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHairCut();
            }
        }

        public double HairCutInProduct
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHairCutInProduct();
            }
        }

        public double Hedging
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHedging();
            }
        }

        public double HedgingInProduct
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHedgingInProduct();
            }
        }

        public double InterestOnCouponCollateral
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInterestOnCouponCollateral();
            }
        }

        public int IssuerCode
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuerCode();
            }
        }

        public string LoanAndRepoType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetLoanAndRepoType().SophisEnumToString(); 
            }
        }

        public double MarginOnCollateral
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarginOnCollateral();
            }
        }

        public int MarketCode
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketCode();
            }
        }

        public double MinimumAmount
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMinimumAmount();
            }
        }

        public int NotificationDelay
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotificationDelay();
            }
        }

        public string NotificationType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotificationType().SophisEnumToString(); 
            }
        }

        public double Notional
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotional();
            }
        }

        public int PaymentDate
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPaymentDate();
            }
        }

        public int PaymentType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPaymentType();
            }
        }

        public int Perimeter
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPerimeter();
            }
        }

        public string PnlType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPnlType().SophisEnumToString(); 
            }
        }

        public double Quotity
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public double RateOnCollateral
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRateOnCollateral();
            }
        }

        public int SettlementCurrency
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSettlementCurrency();
            }
        }

        public int StartDate
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStartDate();
            }
        }

        public int StartDateInProduct
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStartDateInProduct();
            }
        }

        public string StockLoanPricingType
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStockLoanPricingType().SophisEnumToString();
            }
        }

        public double TotalSpot
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetTotalSpot();
            }
        }

        public int UnderlyingCode
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnderlyingCode();
            }
        }

        public string UnrealizedMethod
        {
            get
            {
                using CSMLoanAndRepo instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnrealizedMethod().SophisEnumToString();
            }
        }
    }
}
