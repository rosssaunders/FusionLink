using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class ContractForDifference : Instrument
    {
        public ContractForDifference(int code) : base(code)
        {
        }

        public bool AllInRate
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAllInRate();
            }
        }

        public string AskQuotationType
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAskQuotationType().SophisEnumToString();
            }
        }

        public int AveragePriceComputationType
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAveragePriceComputationType();
            }
        }

        public int BillingCurrency
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBillingCurrency();
            }
        }

        public string CfdCalculation
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCfdCalculation().SophisEnumToString();
            }
        }

        public int CollateralCode
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCollateralCode();
            }
        }

        public double CollateralQuantity
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCollateralQuantity();
            }
        }

        public double CollateralSpot
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCollateralSpot();
            }
        }

        public string CollateralType
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCollateralType().SophisEnumToString();
            }
        }

        public double CommissionAmount
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionAmount();
            }
        }

        public int CommissionCurrency
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionCurrency();
            }
        }

        public double CommissionQuotity
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionQuotity();
            }
        }

        public string CommissionType
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCommissionType().SophisEnumToString();
            }
        }

        public double CouponCollateral
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCouponCollateral();
            }
        }

        public string CreationMethod
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCreationMethod().SophisEnumToString();
            }
        }

        public int CurrencyQuotationAvailable
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCurrencyQuotationAvailable();
            }
        }

        public string DayCountBasisType
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDayCountBasisType().SophisEnumToString();
            }
        }

        public double DividendRate
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetDividendRate();
            }
        }

        public object EndDateInProduct
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetEndDateInProduct().ToDateTime();
            }
        }

        public object Expiry
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetExpiry().ToDateTime();
            }
        }

        public double FactorForRealised
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFactorForRealised();
            }
        }

        public int FloatingRateOnCollateral
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFloatingRateOnCollateral();
            }
        }

        public int FloatingRateOnCommission
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetFloatingRateOnCommission();
            }
        }

        public string ForexOrder
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetForexOrder().SophisEnumToString();
            }
        }

        public double ForexValue
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetForexValue();
            }
        }

        public double HairCut
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHairCut();
            }
        }

        public double Hedging
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHedging();
            }
        }

        public double HedgingInProduct
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetHedgingInProduct();
            }
        }

        public double InterestOnCouponCollateral
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInterestOnCouponCollateral();
            }
        }

        public int IssuerCode
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuerCode();
            }
        }

        public double MarginOnCollateral
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarginOnCollateral();
            }
        }

        public int MarketCode
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMarketCode();
            }
        }

        public double MinimumAmount
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetMinimumAmount();
            }
        }

        public int NotificationDelay
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotificationDelay();
            }
        }

        public double Notional
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetNotional();
            }
        }

        public object PaymentDate
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPaymentDate().ToDateTime();
            }
        }

        public int PaymentType
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPaymentType();
            }
        }

        public int Perimeter
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPerimeter();
            }
        }

        public string PnlType
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetPnlType().SophisEnumToString();
            }
        }

        public double Quotity
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public double RateOnCollateral
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetRateOnCollateral();
            }
        }

        public int SettlementCurrency
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSettlementCurrency();
            }
        }

        public double SpotDefaultForPurchase
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetSpotDefaultForPurchase();
            }
        }

        public object StartDate
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStartDate().ToDateTime();
            }
        }

        public object StartDateInProduct
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStartDateInProduct().ToDateTime();
            }
        }

        public string StockLoanPricingType
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetStockLoanPricingType().SophisEnumToString();
            }
        }

        public double TotalSpot
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetTotalSpot();
            }
        }

        public int UnderlyingCode
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnderlyingCode();
            }
        }

        public string UnrealizedMethod
        {
            get
            {
                using CSMContractForDifference instrument = CSMInstrument.GetInstance(code);
                return instrument.GetUnrealizedMethod().SophisEnumToString();
            }
        }
    }
}
