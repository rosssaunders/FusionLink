////  Copyright (c) RXD Solutions. All rights reserved.


//using System;
//using System.Collections.Generic;
//using RxdSolutions.FusionLink.Helpers;
//using RxdSolutions.FusionLink.Properties;
//using sophis.commodity;
//using sophis.instrument;
//using sophis.static_data;
//using sophis.value;

//namespace RxdSolutions.FusionLink.Services
//{
//    internal static class InstrumentFields
//    {
//        private static readonly Dictionary<string, Func<CSMInstrument, object>> InstrumentFieldLookup = new Dictionary<string, Func<CSMInstrument, object>>();        //Core Fields

//        private static readonly Dictionary<string, Func<CSMEquity, object>> EquityFields = new Dictionary<string, Func<CSMEquity, object>>();                                           //A
//        private static readonly Dictionary<string, Func<CSMCapFloor, object>> CapFloorFields = new Dictionary<string, Func<CSMCapFloor, object>>();                                     //B
//        private static readonly Dictionary<string, Func<CSMCommission, object>> CommissionFields = new Dictionary<string, Func<CSMCommission, object>>();                               //C
//        private static readonly Dictionary<string, Func<CSMOption, object>> OptionFields = new Dictionary<string, Func<CSMOption, object>>();                                           //D
//        private static readonly Dictionary<string, Func<CSMForexSpot, object>> ForexFieldLookup = new Dictionary<string, Func<CSMForexSpot, object>>();                                 //E
//        private static readonly Dictionary<string, Func<CSMFuture, object>> FutureFieldLookup = new Dictionary<string, Func<CSMFuture, object>>();                                      //F
//        private static readonly Dictionary<string, Func<CSMContractForDifference, object>> CFDFieldLookup = new Dictionary<string, Func<CSMContractForDifference, object>>();           //G
//        private static readonly Dictionary<string, Func<CSMIssuer, object>> IssuerFieldLookup = new Dictionary<string, Func<CSMIssuer, object>>();                                      //H
//        private static readonly Dictionary<string, Func<CSMInstrument, object>> IndexesFieldLookup = new Dictionary<string, Func<CSMInstrument, object>>();                             //I
//        private static readonly Dictionary<string, Func<CSMNonDeliverableForexForward, object>> NDFFieldLookup = new Dictionary<string, Func<CSMNonDeliverableForexForward, object>>(); //K
//        //private static readonly Dictionary<string, Func<CSMLoanAndRepo, object>> LoansAndReposFieldLookup = new Dictionary<string, Func<CSMLoanAndRepo, object>>();                   //L
//        private static readonly Dictionary<string, Func<CSMBondBasket, object>> BondBasketFieldLookup = new Dictionary<string, Func<CSMBondBasket, object>>();                          //N
//        private static readonly Dictionary<string, Func<CSMOption, object>> ListedOptionFieldLookup = new Dictionary<string, Func<CSMOption, object>>();                                //M
//        private static readonly Dictionary<string, Func<CSMBond, object>> BondFieldLookup = new Dictionary<string, Func<CSMBond, object>>();                                            //O
//        private static readonly Dictionary<string, Func<CSMLoanAndRepo, object>> LoanAndRepoFieldLookup = new Dictionary<string, Func<CSMLoanAndRepo, object>>();                       //P
//        private static readonly Dictionary<string, Func<CSMCommodity, object>> CommodityFieldLookup = new Dictionary<string, Func<CSMCommodity, object>>();                             //Q
//        private static readonly Dictionary<string, Func<CSMInterestRate, object>> InterestRateFieldLookup = new Dictionary<string, Func<CSMInterestRate, object>>();                    //R
//        private static readonly Dictionary<string, Func<CSMSwap, object>> SwapFields = new Dictionary<string, Func<CSMSwap, object>>();                                                 //S
//        private static readonly Dictionary<string, Func<CSMDebtInstrument, object>> DebtInstrumentFields = new Dictionary<string, Func<CSMDebtInstrument, object>>();                   //T
//        private static readonly Dictionary<string, Func<CSMCommodityBasket, object>> CommodityBasketFields = new Dictionary<string, Func<CSMCommodityBasket, object>>();                //U
//        private static readonly Dictionary<string, Func<CSMOption, object>> SwappedOptionFields = new Dictionary<string, Func<CSMOption, object>>();                                    //W
//        private static readonly Dictionary<string, Func<CSMForexFuture, object>> ForexFutureLookupFields = new Dictionary<string, Func<CSMForexFuture, object>>();                      //X
//        private static readonly Dictionary<string, Func<CSMAmFund, object>> FundFields = new Dictionary<string, Func<CSMAmFund, object>>();                                             //Z

//        static InstrumentFields()
//        {
//            SetupInstruments();             //Core fields
//            SetupBonds();                   //O
//            SetupCommissions();             //C

//            SetupBondBasket();              //N

//            SetupEquities();                //A
//            SetupCapFloors();               //B
            
//            SetupOptions();                 //D
//            SetupForex();                   //E
//            SetupFutures();                 //F
//            SetupCFD();                     //G
//            SetupIssuer();                  //H
//            SetupIndexesAndBaskets();       //I
//            SetupNDF();                     //K
//            SetupLoansAndRepos();           //L
            
//            SetupListedOptions();           //M
            
//            SetupLoansOnStock();            //P
//            SetupCommodities();             //Q
//            SetupInterestRate();            //R
//            SetupSwaps();                   //S
//            SetupDebtInstruments();         //T
//            SetupCommoditiesIndexes();      //U
//            SetupSwappedOptions();          //W
//            SetupForexFuture();             //X
//            SetupFund();                    //Z
//        }

//        internal static object GetValue<T>(T instrument, string property) where T : CSMInstrument
//        {
//            var propertyUpper = property.ToUpper();

//            object FB()
//            {
//                return InstrumentFieldLookup.ContainsKey(propertyUpper) ? InstrumentFieldLookup[propertyUpper](instrument) : string.Format(Resources.InstrumentPropertyNotFoundMessage, property);
//            }

//            switch (instrument)
//            {
//                case CSMIssuer h: return IssuerFieldLookup.ContainsKey(propertyUpper) ? IssuerFieldLookup[propertyUpper](h) : FB();
//                case CSMNonDeliverableForexForward k: return NDFFieldLookup.ContainsKey(propertyUpper) ? NDFFieldLookup[propertyUpper](k) : FB();
//                case CSMCapFloor b: return CapFloorFields.ContainsKey(propertyUpper) ? CapFloorFields[propertyUpper](b) : FB();
//                case CSMCommission c: return CommissionFields.ContainsKey(propertyUpper) ? CommissionFields[propertyUpper](c) : FB();
//                case CSMOption d: return OptionFields.ContainsKey(propertyUpper) ? OptionFields[propertyUpper](d) : FB();
//                case CSMForexSpot e: return ForexFieldLookup.ContainsKey(propertyUpper) ? ForexFieldLookup[propertyUpper](e) : FB();
//                case CSMFuture f: return FutureFieldLookup.ContainsKey(propertyUpper) ? FutureFieldLookup[propertyUpper](f) : FB();
//                case CSMContractForDifference g: return CFDFieldLookup.ContainsKey(propertyUpper) ? CFDFieldLookup[propertyUpper](g) : FB();
//                case CSMLoanAndRepo l: return LoanAndRepoFieldLookup.ContainsKey(propertyUpper) ? LoanAndRepoFieldLookup[propertyUpper](l) : FB();
//                case CSMBondBasket n: return BondBasketFieldLookup.ContainsKey(propertyUpper) ? BondBasketFieldLookup[propertyUpper](n) : FB();
//                case CSMBond o: return BondFieldLookup.ContainsKey(propertyUpper) ? BondFieldLookup[propertyUpper](o) : FB();
//                case CSMCommodityBasket u: return CommodityBasketFields.ContainsKey(propertyUpper) ? CommodityBasketFields[propertyUpper](u) : FB();
//                case CSMCommodity q: return CommodityFieldLookup.ContainsKey(propertyUpper) ? CommodityFieldLookup[propertyUpper](q) : FB();
//                case CSMInterestRate r: return InterestRateFieldLookup.ContainsKey(propertyUpper) ? InterestRateFieldLookup[propertyUpper](r) : FB();
//                case CSMSwap s: return SwapFields.ContainsKey(propertyUpper) ? SwapFields[propertyUpper](s) : FB();
//                case CSMDebtInstrument t: return DebtInstrumentFields.ContainsKey(propertyUpper) ? DebtInstrumentFields[propertyUpper](t) : FB();
//                case CSMForexFuture x: return ForexFutureLookupFields.ContainsKey(propertyUpper) ? ForexFutureLookupFields[propertyUpper](x) : FB();
//                case CSMAmFund z: return FundFields.ContainsKey(propertyUpper) ? FundFields[propertyUpper](z) : FB();
//                case CSMEquity e: return EquityFields.ContainsKey(propertyUpper) ? EquityFields[propertyUpper](e) : FB();
//                case CSMInstrument i: return InstrumentFieldLookup.ContainsKey(propertyUpper) ? InstrumentFieldLookup[propertyUpper](i) : FB();

//                default: return FB();
//            }
//        }

//        private static void SetupInstruments()
//        {
//            InstrumentFieldLookup.Add("Code".ToUpper(), x => x.GetCode());
//            InstrumentFieldLookup.Add("Reference".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetReference));
//            InstrumentFieldLookup.Add("Name".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetName));
//            InstrumentFieldLookup.Add("Allotment".ToUpper(), x => x.GetAllotment().GetAllotmentName());
//            InstrumentFieldLookup.Add("Comment".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetComment));
//            InstrumentFieldLookup.Add("Market".ToUpper(), x => x.GetCSRMarket().GetMarketName());
//            InstrumentFieldLookup.Add("Currency".ToUpper(), x => x.GetCurrency().GetCurrencyCode());
//            InstrumentFieldLookup.Add("Type".ToUpper(), x => x.GetType_API().ToString());
//        }

//        private static void SetupForex()
//        {
//            ForexFieldLookup.Add("Accuracy".ToUpper(), x => x.GetAccuracy());
//            ForexFieldLookup.Add("Ask".ToUpper(), x => x.GetAsk());
//            ForexFieldLookup.Add("AskQuotationType".ToUpper(), x => x.GetAskQuotationType());
//            ForexFieldLookup.Add("Bid".ToUpper(), x => x.GetBid());
//            ForexFieldLookup.Add("Forex1".ToUpper(), x => x.GetForex1());
//            ForexFieldLookup.Add("Forex2".ToUpper(), x => x.GetForex2());
//            ForexFieldLookup.Add("FxPair".ToUpper(), x => x.GetFxPair());
//            ForexFieldLookup.Add("MarketQuotity".ToUpper(), x => x.GetMarketQuotity());
//            ForexFieldLookup.Add("MarketWay".ToUpper(), x => x.GetMarketWay());
//            ForexFieldLookup.Add("Quotity".ToUpper(), x => x.GetQuotity());
//            ForexFieldLookup.Add("SettlementCurrency".ToUpper(), x => x.GetSettlementCurrency());
//            ForexFieldLookup.Add("SpotDefaultForPurchase".ToUpper(), x => x.GetSpotDefaultForPurchase());
//            ForexFieldLookup.Add("ReportingBySettlementDate".ToUpper(), x => x.ReportingBySettlementDate());
//        }

//        private static void SetupBonds()
//        {
//            BondFieldLookup.Add("Notional".ToUpper(), x => x.GetNotional());
//            BondFieldLookup.Add("NotionalCurrency".ToUpper(), x => x.GetNotionalCurrency().GetCurrencyCode());
//            BondFieldLookup.Add("IssueDate".ToUpper(), x => x.GetIssueDate().ToDateTime());
//            BondFieldLookup.Add("Maturity".ToUpper(), x => x.GetMaturity().ToDateTime());
//            BondFieldLookup.Add("IssuerCode".ToUpper(), x => x.GetIssuerCode());
//            BondFieldLookup.Add("Seniority".ToUpper(), x => x.GetSeniority().GetSeniorityName());
//            BondFieldLookup.Add("GuarantorCode".ToUpper(), x => x.GetGuarantorCode());
//            BondFieldLookup.Add("ReferenceTreasuryCode".ToUpper(), x => x.GetReferenceTreasuryCode());
//            BondFieldLookup.Add("FixedRateFrequency".ToUpper(), x => x.GetMarketBondPeriodicityType().SophisEnumToString());
//            BondFieldLookup.Add("FixedRateBasis".ToUpper(), x => x.GetMarketAIDayCountBasisType().SophisEnumToString());
//            BondFieldLookup.Add("FixedRateMode".ToUpper(), x => x.GetYieldCalculationType().SophisEnumToString());
//            BondFieldLookup.Add("FixedRate".ToUpper(), x => x.GetNotionalRate());
//            BondFieldLookup.Add("CashRoundingRule".ToUpper(), x => x.GetRoundingRule().SophisEnumToString());
//            BondFieldLookup.Add("CashRoundingType".ToUpper(), x => x.GetRoundingType().SophisEnumToString());
//            BondFieldLookup.Add("CashRoundingAmount".ToUpper(), x => x.GetRoundingAmount().ConvertSophisNumber());
//            BondFieldLookup.Add("NumberOfSecurities".ToUpper(), x => x.GetInstrumentCount());
//            BondFieldLookup.Add("QuotationType".ToUpper(), x => x.GetQuotationType().SophisEnumToString());
//            BondFieldLookup.Add("RangeAccrual".ToUpper(), x => x.GetRangeAccrual() != null);
//            BondFieldLookup.Add("ExoticUnderlyingType".ToUpper(), x => x.GetExoticUnderlyingType().SophisEnumToString());
//            BondFieldLookup.Add("BankHolidaysRule".ToUpper(), x => x.GetHolidayAdjustmentType().SophisEnumToString());
//            BondFieldLookup.Add("SettlementShift".ToUpper(), x => x.GetPaymentShift());
//            BondFieldLookup.Add("OwnershipShift".ToUpper(), x => x.GetSettlementShift());
//            BondFieldLookup.Add("OwnershipType".ToUpper(), x => x.GetBondOwnershipType().SophisEnumToString());
//            BondFieldLookup.Add("FirstSettlementDate".ToUpper(), x => x.GetFirstSettlementDate().ToDateTime());
//            BondFieldLookup.Add("ComputationOnAdjustedDate".ToUpper(), x => x.GetMarketCalculationAIOnAdjustedDates());
//            BondFieldLookup.Add("CouponsOnAdjustedDate".ToUpper(), x => x.GetMarketBondCouponAdjusted());
//            BondFieldLookup.Add("FirstCouponDate".ToUpper(), x => x.GetFirstCouponDate().ToDateTime());
//            BondFieldLookup.Add("LastButOneCouponDate".ToUpper(), x => x.GetLastButOneCouponDate().ToDateTime());
//            BondFieldLookup.Add("StepStartDate".ToUpper(), x => x.GetRedemptionStepStartDate().ToDateTime());
//            BondFieldLookup.Add("StepEndDate".ToUpper(), x => x.GetRedemptionStepEndDate().ToDateTime());
//            BondFieldLookup.Add("StepType".ToUpper(), x => x.GetRedemptionStepType().SophisEnumToString());
//            BondFieldLookup.Add("StepValue".ToUpper(), x => x.GetRedemptionStepValue());
//            BondFieldLookup.Add("MinimumPiece".ToUpper(), x => x.GetMinTradingSize());
//            BondFieldLookup.Add("LowestIncrement".ToUpper(), x => x.GetTradingUnits());
//            BondFieldLookup.Add("CouponSettlementLag".ToUpper(), x => x.GetCouponSettlementLag());
//            BondFieldLookup.Add("CouponSettlementLagType".ToUpper(), x => x.GetCouponSettlementLagType().SophisEnumToString());
//            BondFieldLookup.Add("CouponSettlementCurrency".ToUpper(), x => x.GetDeliveryCurrency().GetCurrencyCode());
//            BondFieldLookup.Add("CalledDate".ToUpper(), x => x.GetCalledDate().ToDateTime());
//            BondFieldLookup.Add("CalledPrice".ToUpper(), x => x.GetCalledPrice());
//            BondFieldLookup.Add("NumberOfPartialRedemptions".ToUpper(), x => x.GetNumberOfPartialRedemptions());
//            BondFieldLookup.Add("PartialRedeemPercentage".ToUpper(), x => x.GetRedeemPercentage());
//            BondFieldLookup.Add("RedemptionDistribution".ToUpper(), x => x.GetPartialRedemptionType().GetPartialPaymentMethod());
//            BondFieldLookup.Add("CancellationType".ToUpper(), x => x.GetCancellationType().SophisEnumToString());
//        }

//        private static void SetupListedOptions()
//        {
//            //Nothing to do here. Use the CSMOptions.
//        }

//        private static void SetupLoansOnStock()
//        {
//        }

//        private static void SetupCommodities()
//        {
//            CommodityFieldLookup.Add("QuotationTick".ToUpper(), x => x.GetQuotationTick());
//            CommodityFieldLookup.Add("RoundingAvg".ToUpper(), x => x.GetRoundingAvg());
//            CommodityFieldLookup.Add("UnitOfTrading".ToUpper(), x => x.GetQuotity());
//        }

//        private static void SetupCommoditiesIndexes()
//        {
//            //Nothing to do here
//        }

//        private static void SetupSwappedOptions()
//        {
//            //Use CSMOptions.
//        }

//        private static void SetupFutures()
//        {
//            FutureFieldLookup.Add("Basis".ToUpper(), x => x.GetBasis());
//            FutureFieldLookup.Add("Cost".ToUpper(), x => x.GetClearingFees());
//            FutureFieldLookup.Add("ConversionRatioInPrice".ToUpper(), x => x.GetConversionRatioInPrice());
//            FutureFieldLookup.Add("ConversionRatioInProduct".ToUpper(), x => x.GetConversionRatioInProduct());
//            FutureFieldLookup.Add("DeliveryDate".ToUpper(), x => x.GetDeliveryDate().ToDateTime());
//            FutureFieldLookup.Add("DeliveryType".ToUpper(), x => x.GetDeliveryType().SophisEnumToString());
//            FutureFieldLookup.Add("Maturity".ToUpper(), x => x.GetExpiry().ToDateTime());
//            FutureFieldLookup.Add("FinalSettlement".ToUpper(), x => x.GetFinalSettlement().ToDateTime());
//            FutureFieldLookup.Add("FRAEffectiveDate".ToUpper(), x => x.GetFRAEffectiveDate().ToDateTime());
//            FutureFieldLookup.Add("FRAEndDate".ToUpper(), x => x.GetFRAEndDate().ToDateTime());
//            FutureFieldLookup.Add("Market".ToUpper(), x => x.GetListedMarketId().GetListedMarketName());
//            FutureFieldLookup.Add("MarketCode".ToUpper(), x => x.GetListedMarketId());
//            FutureFieldLookup.Add("Notional".ToUpper(), x => x.GetNotional());
//            FutureFieldLookup.Add("PointValue".ToUpper(), x => x.GetQuotity());
//            FutureFieldLookup.Add("ContractSize".ToUpper(), x => x.GetQuotity());
//            FutureFieldLookup.Add("UnderlyingCode".ToUpper(), x => x.GetUnderlyingCode());
//        }

//        private static void SetupLoansAndRepos()
//        {
//            //TODO
//            LoanAndRepoFieldLookup.Add("AllInRate".ToUpper(), x => x.GetAllInRate());
//            LoanAndRepoFieldLookup.Add("Allotment".ToUpper(), x => x.GetAllotment());
//            LoanAndRepoFieldLookup.Add("AveragePriceComputationType".ToUpper(), x => x.GetAveragePriceComputationType());
//            LoanAndRepoFieldLookup.Add("BillingCurrency".ToUpper(), x => x.GetBillingCurrency());
//            LoanAndRepoFieldLookup.Add("CollateralCode".ToUpper(), x => x.GetCollateralCode());
//            LoanAndRepoFieldLookup.Add("CollateralQuantity".ToUpper(), x => x.GetCollateralQuantity());
//            LoanAndRepoFieldLookup.Add("CollateralSpot".ToUpper(), x => x.GetCollateralSpot());
//            LoanAndRepoFieldLookup.Add("CollateralType".ToUpper(), x => x.GetCollateralType());
//            LoanAndRepoFieldLookup.Add("Comment".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetComment));
//            LoanAndRepoFieldLookup.Add("CommissionAccrualMode".ToUpper(), x => x.GetCommissionAccrualMode());
//            LoanAndRepoFieldLookup.Add("CommissionAmount".ToUpper(), x => x.GetCommissionAmount());
//            LoanAndRepoFieldLookup.Add("CommissionCurrency".ToUpper(), x => x.GetCommissionCurrency());
//            LoanAndRepoFieldLookup.Add("CommissionMode".ToUpper(), x => x.GetCommissionMode());
//            LoanAndRepoFieldLookup.Add("CommissionQuotity".ToUpper(), x => x.GetCommissionQuotity());
//            LoanAndRepoFieldLookup.Add("CommissionType".ToUpper(), x => x.GetCommissionType());
//            LoanAndRepoFieldLookup.Add("CouponCollateral".ToUpper(), x => x.GetCouponCollateral());
//            LoanAndRepoFieldLookup.Add("CreationMethod".ToUpper(), x => x.GetCreationMethod());
//            LoanAndRepoFieldLookup.Add("CurrencyQuotationAvailable".ToUpper(), x => x.GetCurrencyQuotationAvailable());
//            LoanAndRepoFieldLookup.Add("DayCountBasisType".ToUpper(), x => x.GetDayCountBasisType());
//            LoanAndRepoFieldLookup.Add("DeliveryType".ToUpper(), x => x.GetDeliveryType());
//            LoanAndRepoFieldLookup.Add("DividendRate".ToUpper(), x => x.GetDividendRate());
//            LoanAndRepoFieldLookup.Add("EndDateInProduct".ToUpper(), x => x.GetEndDateInProduct());
//            LoanAndRepoFieldLookup.Add("ExerciseDate".ToUpper(), x => x.GetExerciseDate());
//            LoanAndRepoFieldLookup.Add("Expiry".ToUpper(), x => x.GetExpiry());
//            LoanAndRepoFieldLookup.Add("FactorForRealised".ToUpper(), x => x.GetFactorForRealised());
//            LoanAndRepoFieldLookup.Add("FloatingRateOnCollateral".ToUpper(), x => x.GetFloatingRateOnCollateral());
//            LoanAndRepoFieldLookup.Add("FloatingRateOnCommission".ToUpper(), x => x.GetFloatingRateOnCommission());
//            LoanAndRepoFieldLookup.Add("ForexOrder".ToUpper(), x => x.GetForexOrder());
//            LoanAndRepoFieldLookup.Add("ForexValue".ToUpper(), x => x.GetForexValue());
//            LoanAndRepoFieldLookup.Add("HairCut".ToUpper(), x => x.GetHairCut());
//            LoanAndRepoFieldLookup.Add("HairCutInProduct".ToUpper(), x => x.GetHairCutInProduct());
//            LoanAndRepoFieldLookup.Add("Hedging".ToUpper(), x => x.GetHedging());
//            LoanAndRepoFieldLookup.Add("HedgingInProduct".ToUpper(), x => x.GetHedgingInProduct());
//            LoanAndRepoFieldLookup.Add("InterestOnCouponCollateral".ToUpper(), x => x.GetInterestOnCouponCollateral());
//            LoanAndRepoFieldLookup.Add("IssuerCode".ToUpper(), x => x.GetIssuerCode());
//            LoanAndRepoFieldLookup.Add("LoanAndRepoType".ToUpper(), x => x.GetLoanAndRepoType());
//            LoanAndRepoFieldLookup.Add("MarginOnCollateral".ToUpper(), x => x.GetMarginOnCollateral());
//            LoanAndRepoFieldLookup.Add("MarketCode".ToUpper(), x => x.GetMarketCode());
//            LoanAndRepoFieldLookup.Add("MinimumAmount".ToUpper(), x => x.GetMinimumAmount());
//            LoanAndRepoFieldLookup.Add("Name".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetName));
//            LoanAndRepoFieldLookup.Add("NotificationDelay".ToUpper(), x => x.GetNotificationDelay());
//            LoanAndRepoFieldLookup.Add("NotificationType".ToUpper(), x => x.GetNotificationType());
//            LoanAndRepoFieldLookup.Add("Notional".ToUpper(), x => x.GetNotional());
//            LoanAndRepoFieldLookup.Add("PaymentDate".ToUpper(), x => x.GetPaymentDate());
//            LoanAndRepoFieldLookup.Add("PaymentType".ToUpper(), x => x.GetPaymentType());
//            LoanAndRepoFieldLookup.Add("Perimeter".ToUpper(), x => x.GetPerimeter());
//            LoanAndRepoFieldLookup.Add("PnlType".ToUpper(), x => x.GetPnlType());
//            LoanAndRepoFieldLookup.Add("Quotity".ToUpper(), x => x.GetQuotity());
//            LoanAndRepoFieldLookup.Add("RateOnCollateral".ToUpper(), x => x.GetRateOnCollateral());
//            LoanAndRepoFieldLookup.Add("SettlementCurrency".ToUpper(), x => x.GetSettlementCurrency());
//            LoanAndRepoFieldLookup.Add("StartDate".ToUpper(), x => x.GetStartDate());
//            LoanAndRepoFieldLookup.Add("StartDateInProduct".ToUpper(), x => x.GetStartDateInProduct());
//            LoanAndRepoFieldLookup.Add("StockLoanPricingType".ToUpper(), x => x.GetStockLoanPricingType());
//            LoanAndRepoFieldLookup.Add("TotalSpot".ToUpper(), x => x.GetTotalSpot());
//            LoanAndRepoFieldLookup.Add("UnderlyingCode".ToUpper(), x => x.GetUnderlyingCode());
//            LoanAndRepoFieldLookup.Add("UnrealizedMethod".ToUpper(), x => x.GetUnrealizedMethod());
//        }

//        private static void SetupCommissions()
//        {
//            CommissionFields.Add("BusinessEvent".ToUpper(), x => x.GetBEType().GetBusinessEventType());
//            CommissionFields.Add("DepositoryCode".ToUpper(), x => x.GetBrokerCode());
//            CommissionFields.Add("Depository".ToUpper(), x => x.GetBrokerCode().GetThirdPartyReference());
//            CommissionFields.Add("Rate".ToUpper(), x => x.GetFloatingRate());
//        }

//        private static void SetupFund()
//        {
//            FundFields.Add("Administrator".ToUpper(), x => x.GetAdministrator());
//            FundFields.Add("Entity".ToUpper(), x => x.GetEntity());
//            FundFields.Add("EODOffset".ToUpper(), x => x.GetEODOffset());
//            FundFields.Add("FactorModel".ToUpper(), x => x.GetFactorModel());
//            FundFields.Add("FeesPortfolio".ToUpper(), x => x.GetFeesPortfolio());
//            FundFields.Add("FundCashPortfolio".ToUpper(), x => x.GetFundCashPortfolio());
//            FundFields.Add("InvestmentPortfolio".ToUpper(), x => x.GetInvestmentPortfolio());
//            FundFields.Add("PortfolioUnderlying".ToUpper(), x => x.GetPortfolioUnderlying());
//            FundFields.Add("SRPortfolio".ToUpper(), x => x.GetSRPortfolio());
//            FundFields.Add("TradingPortfolio".ToUpper(), x => x.GetTradingPortfolio());
//            FundFields.Add("Tolerance".ToUpper(), x => x.GetTolerance());
//            FundFields.Add("ToleranceType".ToUpper(), x => x.GetToleranceType());
//        }

//        private static void SetupCommodityBasket()
//        {
//            CommodityBasketFields.Add("CommodityType", x => x.GetCommodityType());
//        }

//        private static void SetupDebtInstruments()
//        {
//            DebtInstrumentFields.Add("AskQuotationType".ToUpper(), x => x.GetAskQuotationType());
//            DebtInstrumentFields.Add("DateDebut".ToUpper(), x => x.GetDateDebut());
//            DebtInstrumentFields.Add("DateFin".ToUpper(), x => x.GetDateFin());
//            DebtInstrumentFields.Add("DayCountBasisType".ToUpper(), x => x.GetDayCountBasisType());
//            DebtInstrumentFields.Add("Expiry".ToUpper(), x => x.GetExpiry());
//            DebtInstrumentFields.Add("FloatingRate".ToUpper(), x => x.GetFloatingRate());
//            DebtInstrumentFields.Add("InstrumentSpread".ToUpper(), x => x.GetInstrumentSpread());
//            DebtInstrumentFields.Add("IssueDate".ToUpper(), x => x.GetIssueDate());
//            DebtInstrumentFields.Add("IssuerCode".ToUpper(), x => x.GetIssuerCode());
//            DebtInstrumentFields.Add("Margin".ToUpper(), x => x.GetMargin());
//            DebtInstrumentFields.Add("MarketAIDayCountBasisType".ToUpper(), x => x.GetMarketAIDayCountBasisType());
//            DebtInstrumentFields.Add("MarketBondPeriodicityType".ToUpper(), x => x.GetMarketBondPeriodicityType());
//            DebtInstrumentFields.Add("MarketCalculationYTMOnAdjustedDates".ToUpper(), x => x.GetMarketCalculationYTMOnAdjustedDates());
//            DebtInstrumentFields.Add("MarketCalculationYTMOnSettlementDate".ToUpper(), x => x.GetMarketCalculationYTMOnSettlementDate());
//            DebtInstrumentFields.Add("MarketCSDayCountBasisType".ToUpper(), x => x.GetMarketCSDayCountBasisType());
//            DebtInstrumentFields.Add("MarketYTMDayCountBasisType".ToUpper(), x => x.GetMarketYTMDayCountBasisType());
//            DebtInstrumentFields.Add("MarketYTMYieldCalculationType".ToUpper(), x => x.GetMarketYTMYieldCalculationType());
//            DebtInstrumentFields.Add("Notional".ToUpper(), x => x.GetNotional());
//            DebtInstrumentFields.Add("NotionalRate".ToUpper(), x => x.GetNotionalRate());
//            DebtInstrumentFields.Add("PaymentCouponsAtTheEndOfTheDay".ToUpper(), x => x.GetPaymentCouponsAtTheEndOfTheDay());
//            DebtInstrumentFields.Add("Quotity".ToUpper(), x => x.GetQuotity());
//            DebtInstrumentFields.Add("Rate".ToUpper(), x => x.GetRate());
//            DebtInstrumentFields.Add("Spread".ToUpper(), x => x.GetSpread());
//            DebtInstrumentFields.Add("SpreadType".ToUpper(), x => x.GetSpreadType());
//            DebtInstrumentFields.Add("StartDate".ToUpper(), x => x.GetStartDate());
//            DebtInstrumentFields.Add("YieldCalculationType".ToUpper(), x => x.GetYieldCalculationType());
//        }

//        private static void SetupNDF()
//        {
//            //Nothing to do here
//        }

//        private static void SetupForexFuture()
//        {
//            ForexFutureLookupFields.Add("AskQuotationType".ToUpper(), x => x.GetAskQuotationType());
//            ForexFutureLookupFields.Add("Expiry".ToUpper(), x => x.GetExpiry());
//            ForexFutureLookupFields.Add("ExpiryCurrency".ToUpper(), x => x.GetExpiryCurrency());
//            ForexFutureLookupFields.Add("ExpiryInProduct".ToUpper(), x => x.GetExpiryInProduct());
//            ForexFutureLookupFields.Add("Quotity".ToUpper(), x => x.GetQuotity());
//            ForexFutureLookupFields.Add("SpotDefaultForPurchase".ToUpper(), x => x.GetSpotDefaultForPurchase());
//            ForexFutureLookupFields.Add("UnrealizedMethod".ToUpper(), x => x.GetUnrealizedMethod());
//        }

//        private static void SetupIndexesAndBaskets()
//        {

//        }

//        private static void SetupIssuer()
//        {
//            IssuerFieldLookup.Add("IndexCode".ToUpper(), x => x.GetIndexCode());
//            IssuerFieldLookup.Add("IssuerCode".ToUpper(), x => x.GetIssuerCode());
//            IssuerFieldLookup.Add("IssuerIndustry".ToUpper(), x => x.GetIssuerIndustry());
//            IssuerFieldLookup.Add("IndustryCode".ToUpper(), x => x.GetIssuerIndustryCode());
//            IssuerFieldLookup.Add("ReferenceEntity".ToUpper(), x => x.GetParentReferenceEntity());
//            IssuerFieldLookup.Add("GetQuotity".ToUpper(), x => x.GetQuotity());
//            IssuerFieldLookup.Add("ReferenceEquity".ToUpper(), x => x.GetReferenceEquity());
//        }

//        private static void SetupInterestRate()
//        {
//            InterestRateFieldLookup.Add("AverageRate".ToUpper(), x => x.GetAverageRate());
//            InterestRateFieldLookup.Add("CalendarCalculation".ToUpper(), x => x.GetCalendarCalculation());
//            InterestRateFieldLookup.Add("CalendarPlace".ToUpper(), x => x.GetCalendarPlace());
//            InterestRateFieldLookup.Add("CapitalizedRoundingDigits".ToUpper(), x => x.GetCapitalizedRoundingDigits());
//            InterestRateFieldLookup.Add("CapitalizedRoundingMethod".ToUpper(), x => x.GetCapitalizedRoundingMethod());
//            InterestRateFieldLookup.Add("CodeForSpecificVolatility".ToUpper(), x => x.GetCodeForSpecificVolatility());
//            InterestRateFieldLookup.Add("DayCountBasisType".ToUpper(), x => x.GetDayCountBasisType());
//            InterestRateFieldLookup.Add("DecimalCount".ToUpper(), x => x.GetDecimalCount());
//            InterestRateFieldLookup.Add("Expiry".ToUpper(), x => x.GetExpiry());
//            InterestRateFieldLookup.Add("FixingPlace".ToUpper(), x => x.GetFixingPlace());
//            InterestRateFieldLookup.Add("IndexRate".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetIndexRate));
//            InterestRateFieldLookup.Add("InflationIndex".ToUpper(), x => x.GetInflationIndex());
//            InterestRateFieldLookup.Add("InflationRule".ToUpper(), x => x.GetInflationRule());
//            InterestRateFieldLookup.Add("InterestRateAverageType".ToUpper(), x => x.GetInterestRateAverageType());
//            InterestRateFieldLookup.Add("InterestRatePeriodicityType".ToUpper(), x => x.GetInterestRatePeriodicityType());
//            InterestRateFieldLookup.Add("InterestRateType".ToUpper(), x => x.GetInterestRateType());
//            InterestRateFieldLookup.Add("LiborMultiplier".ToUpper(), x => x.GetLiborMultiplier());
//            InterestRateFieldLookup.Add("MarketCode".ToUpper(), x => x.GetMarketCode());
//            InterestRateFieldLookup.Add("Maturity".ToUpper(), x => x.GetMaturity());
//            InterestRateFieldLookup.Add("SearchAtDayMinus".ToUpper(), x => x.GetSearchAtDayMinus());
//            InterestRateFieldLookup.Add("ShortIndex".ToUpper(), x => x.GetShortIndex());
//            InterestRateFieldLookup.Add("Spread".ToUpper(), x => x.GetSpread());
//            InterestRateFieldLookup.Add("SwapPeriodicityType".ToUpper(), x => x.GetSwapPeriodicityType());
//        }

//        private static void SetupBondBasket()
//        {
//            //Nothing to do
//        }

//        private static void SetupCFD()
//        {
//            CFDFieldLookup.Add("AllInRate".ToUpper(), x => x.GetAllInRate());
//            CFDFieldLookup.Add("AskQuotationType".ToUpper(), x => x.GetAskQuotationType());
//            CFDFieldLookup.Add("AveragePriceComputationType".ToUpper(), x => x.GetAveragePriceComputationType());
//            CFDFieldLookup.Add("BillingCurrency".ToUpper(), x => x.GetBillingCurrency());
//            CFDFieldLookup.Add("CfdCalculation".ToUpper(), x => x.GetCfdCalculation());
//            CFDFieldLookup.Add("CollateralCode".ToUpper(), x => x.GetCollateralCode());
//            CFDFieldLookup.Add("CollateralQuantity".ToUpper(), x => x.GetCollateralQuantity());
//            CFDFieldLookup.Add("CollateralSpot".ToUpper(), x => x.GetCollateralSpot());
//            CFDFieldLookup.Add("CollateralType".ToUpper(), x => x.GetCollateralType());
//            CFDFieldLookup.Add("CommissionAmount".ToUpper(), x => x.GetCommissionAmount());
//            CFDFieldLookup.Add("CommissionCurrency".ToUpper(), x => x.GetCommissionCurrency());
//            CFDFieldLookup.Add("CommissionQuotity".ToUpper(), x => x.GetCommissionQuotity());
//            CFDFieldLookup.Add("CommissionType".ToUpper(), x => x.GetCommissionType());
//            CFDFieldLookup.Add("CouponCollateral".ToUpper(), x => x.GetCouponCollateral());
//            CFDFieldLookup.Add("CreationMethod".ToUpper(), x => x.GetCreationMethod());
//            CFDFieldLookup.Add("CurrencyQuotationAvailable".ToUpper(), x => x.GetCurrencyQuotationAvailable());
//            CFDFieldLookup.Add("DayCountBasisType".ToUpper(), x => x.GetDayCountBasisType());
//            CFDFieldLookup.Add("DividendRate".ToUpper(), x => x.GetDividendRate());
//            CFDFieldLookup.Add("EndDateInProduct".ToUpper(), x => x.GetEndDateInProduct());
//            CFDFieldLookup.Add("Expiry".ToUpper(), x => x.GetExpiry());
//            CFDFieldLookup.Add("FactorForRealised".ToUpper(), x => x.GetFactorForRealised());
//            CFDFieldLookup.Add("FloatingRateOnCollateral".ToUpper(), x => x.GetFloatingRateOnCollateral());
//            CFDFieldLookup.Add("FloatingRateOnCommission".ToUpper(), x => x.GetFloatingRateOnCommission());
//            CFDFieldLookup.Add("ForexOrder".ToUpper(), x => x.GetForexOrder());
//            CFDFieldLookup.Add("ForexValue".ToUpper(), x => x.GetForexValue());
//            CFDFieldLookup.Add("HairCut".ToUpper(), x => x.GetHairCut());
//            CFDFieldLookup.Add("Hedging".ToUpper(), x => x.GetHedging());
//            CFDFieldLookup.Add("HedgingInProduct".ToUpper(), x => x.GetHedgingInProduct());
//            CFDFieldLookup.Add("InterestOnCouponCollateral".ToUpper(), x => x.GetInterestOnCouponCollateral());
//            CFDFieldLookup.Add("IssuerCode".ToUpper(), x => x.GetIssuerCode());
//            CFDFieldLookup.Add("MarginOnCollateral".ToUpper(), x => x.GetMarginOnCollateral());
//            CFDFieldLookup.Add("MarketCode".ToUpper(), x => x.GetMarketCode());
//            CFDFieldLookup.Add("MinimumAmount".ToUpper(), x => x.GetMinimumAmount());
//            CFDFieldLookup.Add("NotificationDelay".ToUpper(), x => x.GetNotificationDelay());
//            CFDFieldLookup.Add("Notional".ToUpper(), x => x.GetNotional());
//            CFDFieldLookup.Add("PaymentDate".ToUpper(), x => x.GetPaymentDate());
//            CFDFieldLookup.Add("PaymentType".ToUpper(), x => x.GetPaymentType());
//            CFDFieldLookup.Add("Perimeter".ToUpper(), x => x.GetPerimeter());
//            CFDFieldLookup.Add("PnlType".ToUpper(), x => x.GetPnlType());
//            CFDFieldLookup.Add("Quotity".ToUpper(), x => x.GetQuotity());
//            CFDFieldLookup.Add("RateOnCollateral".ToUpper(), x => x.GetRateOnCollateral());
//            CFDFieldLookup.Add("SettlementCurrency".ToUpper(), x => x.GetSettlementCurrency());
//            CFDFieldLookup.Add("SpotDefaultForPurchase".ToUpper(), x => x.GetSpotDefaultForPurchase());
//            CFDFieldLookup.Add("StartDate".ToUpper(), x => x.GetStartDate());
//            CFDFieldLookup.Add("StartDateInProduct".ToUpper(), x => x.GetStartDateInProduct());
//            CFDFieldLookup.Add("StockLoanPricingType".ToUpper(), x => x.GetStockLoanPricingType());
//            CFDFieldLookup.Add("TotalSpot".ToUpper(), x => x.GetTotalSpot());
//            CFDFieldLookup.Add("UnderlyingCode".ToUpper(), x => x.GetUnderlyingCode());
//            CFDFieldLookup.Add("UnrealizedMethod".ToUpper(), x => x.GetUnrealizedMethod());
//        }

//        private static void SetupCapFloors()
//        {
//            CapFloorFields.Add("BrokenDate".ToUpper(), x => x.GetBrokenDate().ToDateTime());
//            CapFloorFields.Add("DateReference".ToUpper(), x => x.GetBrokenDateReference().SophisEnumToString());
//            CapFloorFields.Add("GetBSVolatility".ToUpper(), x => x.GetBSVolatility());
//            CapFloorFields.Add("FloorType".ToUpper(), x => x.GetCapFloorType().SophisEnumToString());
//            CapFloorFields.Add("SpecificVolatility".ToUpper(), x => x.GetCodeForSpecificVolatility());
//            CapFloorFields.Add("ForPayment".ToUpper(), x => x.GetCSRCalendarForPayment().GetName());
//            CapFloorFields.Add("ForRolling".ToUpper(), x => x.GetCSRCalendarForRolling().GetName());
//            CapFloorFields.Add("MetaModel".ToUpper(), x => x.GetCurrentMetaModel().GetName());
//            CapFloorFields.Add("DataDate".ToUpper(), x => x.GetDataDate().ToDateTime());
//            CapFloorFields.Add("DatesAdjustment".ToUpper(), x => x.GetDatesAdjustment().SophisEnumToString());
//            CapFloorFields.Add("Basis".ToUpper(), x => x.GetDayCountBasisType().SophisEnumToString());
//            CapFloorFields.Add("EndDate".ToUpper(), x => x.GetEndDate().ToDateTime());
//            CapFloorFields.Add("StartDate".ToUpper(), x => x.GetStartDate().ToDateTime());
//            CapFloorFields.Add("Timing".ToUpper(), x => x.GetFixingDateRef().SophisEnumToString());
//            CapFloorFields.Add("FixingOffset".ToUpper(), x => x.GetFixingOffset());
//            CapFloorFields.Add("Notional".ToUpper(), x => x.GetNotional());
//            CapFloorFields.Add("FloatingRate".ToUpper(), x => x.GetFloatingRate());
//            CapFloorFields.Add("AdjustmentType".ToUpper(), x => x.GetHolidayAdjustmentType().SophisEnumToString());
//            CapFloorFields.Add("VolatilityAccuracy".ToUpper(), x => x.GetImpliedVolatilityAccuracy());
//            CapFloorFields.Add("IndexCode".ToUpper(), x => x.GetInflationIndexCode());
//            CapFloorFields.Add("GetJplus".ToUpper(), x => x.GetJplus());
//            CapFloorFields.Add("InPercent".ToUpper(), x => x.GetMarketPriceInPercent());
//            CapFloorFields.Add("Settlement".ToUpper(), x => x.GetPaymentMethod().SophisEnumToString());
//            CapFloorFields.Add("PayoffType".ToUpper(), x => x.GetPayoffType().StringValue);
//            CapFloorFields.Add("PeriodicPayment".ToUpper(), x => x.GetPeriodicPayment().SophisEnumToString());
//            CapFloorFields.Add("QuotationType".ToUpper(), x => x.GetQuotationType().SophisEnumToString());
//            CapFloorFields.Add("DecimalNumber".ToUpper(), x => x.GetRoundingDecimalNumber());
//            CapFloorFields.Add("MethodType".ToUpper(), x => x.GetRoundingMethodType().SophisEnumToString());
//            CapFloorFields.Add("UnderlyingCategory".ToUpper(), x => x.GetUnderlyingCategory().SophisEnumToString());
//            CapFloorFields.Add("UnderlyingCode".ToUpper(), x => x.GetUnderlyingCode());
//            CapFloorFields.Add("DependencyType".ToUpper(), x => x.GetVolatilityDependencyType().SophisEnumToString());
//            CapFloorFields.Add("Frequency".ToUpper(), x => x.GetPeriodicPayment().SophisEnumToString());
//            CapFloorFields.Add("Strike".ToUpper(), x => x.GetLeg().GetStrikeInProduct());
//        }

//        private static void SetupOptions()
//        {
//            OptionFields.Add("UnderlyingCode".ToUpper(), x => x.GetUnderlyingCode());
//            OptionFields.Add("Delivery".ToUpper(), x => x.GetDeliveryType().SophisEnumToString());
//            OptionFields.Add("PaymentCurrency".ToUpper(), x => x.GetStrikeCurrency().GetCurrencyCode());
//            OptionFields.Add("Quotation".ToUpper(), x => x.GetSettlementCurrency().GetCurrencyCode());
//            OptionFields.Add("QuotationUnit".ToUpper(), x => x.GetAskQuotationType().SophisEnumToString());
//            OptionFields.Add("CalculationAgent".ToUpper(), x => x.GetCalculationAgent());

//            //OptionFields.Add("FixingType".ToUpper(), x => x.GetFixingType()); --THIS RETURNS A NUMBER THAT ISNT USEFUL
//            //OptionFields.Add("GuarrantedFX".ToUpper(), x => x.GetFX);
//            OptionFields.Add("GuarrantedFX".ToUpper(), x => x.GetGuarrantedForexSpot());



//            OptionFields.Add("AlreadyDownCount".ToUpper(), x => x.GetAlreadyDownCount());
//            OptionFields.Add("AverageType".ToUpper(), x => x.GetAverageType().SophisEnumToString());
//            OptionFields.Add("BarrierCount".ToUpper(), x => x.GetBarrierCount());
//            OptionFields.Add("CalledDate".ToUpper(), x => x.GetCalledDate().ToDateTime());
//            OptionFields.Add("CalledPrice".ToUpper(), x => x.GetCalledPrice());
//            OptionFields.Add("CalledPriceInPercent".ToUpper(), x => x.GetCalledPriceInPercent());
//            OptionFields.Add("CallValueInTree".ToUpper(), x => x.GetCallValueInTree());
//            OptionFields.Add("ConversionRatioInPrice".ToUpper(), x => x.GetConversionRatioInPrice());
//            OptionFields.Add("ConversionRatioInProduct".ToUpper(), x => x.GetConversionRatioInProduct());
//            OptionFields.Add("ConversionRatioInShares".ToUpper(), x => x.GetConversionRatioInShares());
//            OptionFields.Add("ConversionValueInTree".ToUpper(), x => x.GetConversionValueInTree());
//            OptionFields.Add("CouponFrequencyForCB".ToUpper(), x => x.GetCouponFrequencyForCB().SophisEnumToString());
//            OptionFields.Add("CreditSpread".ToUpper(), x => x.GetCreditSpread());
//            OptionFields.Add("CreditSpreadForDerivative".ToUpper(), x => x.GetCreditSpreadForDerivative());
//            OptionFields.Add("CreditSpreadValuationTypeForCB".ToUpper(), x => x.GetCreditSpreadValuationTypeForCB().SophisEnumToString());
//            OptionFields.Add("DayCountBasisTypeForCB".ToUpper(), x => x.GetDayCountBasisTypeForCB().SophisEnumToString());
//            OptionFields.Add("DerivativeType".ToUpper(), x => x.GetDerivativeType().SophisEnumToString());
//            OptionFields.Add("DigitalBasisType".ToUpper(), x => x.GetDigitalBasisType());
//            OptionFields.Add("DigitalStartDate".ToUpper(), x => x.GetDigitalStartDate().ToDateTime());
//            OptionFields.Add("DividendInTaxCreditType".ToUpper(), x => x.GetDividendInTaxCreditType().SophisEnumToString());
//            OptionFields.Add("ExerciseEndDate".ToUpper(), x => x.GetExerciseEndDate().ToDateTime());
//            OptionFields.Add("ExercisePeriodCount".ToUpper(), x => x.GetExercisePeriodCount());
//            OptionFields.Add("ExerciseStartDate".ToUpper(), x => x.GetExerciseStartDate().ToDateTime());
//            OptionFields.Add("ExerciseType".ToUpper(), x => x.GetExerciseType().SophisEnumToString());
//            OptionFields.Add("Expiry".ToUpper(), x => x.GetExpiry().ToDateTime());
//            OptionFields.Add("ExpiryForRho".ToUpper(), x => x.GetExpiryForRho());
//            OptionFields.Add("FinalSettlement".ToUpper(), x => x.GetFinalSettlement().ToDateTime());
//            OptionFields.Add("FixedVolatility".ToUpper(), x => x.GetFixedVolatility());
//            OptionFields.Add("FloatingRate".ToUpper(), x => x.GetFloatingRate());
//            OptionFields.Add("GamaType".ToUpper(), x => x.GetGamaType().SophisEnumToString());
//            OptionFields.Add("GapBetween2Fixings".ToUpper(), x => x.GetGapBetween2Fixings());
//            OptionFields.Add("GuarrantedForexSpot".ToUpper(), x => x.GetGuarrantedForexSpot());
//            OptionFields.Add("HolidayAdjustmentType".ToUpper(), x => x.GetHolidayAdjustmentType().SophisEnumToString());
//            OptionFields.Add("ImpliedCreditSpreadValuationTypeForCB".ToUpper(), x => x.GetImpliedCreditSpreadValuationTypeForCB().SophisEnumToString());
//            OptionFields.Add("IsInAmount".ToUpper(), x => x.GetIsInAmount());
//            OptionFields.Add("IssueDate".ToUpper(), x => x.GetIssueDate().ToDateTime());
//            OptionFields.Add("IssuePrice".ToUpper(), x => x.GetIssuePrice());
//            OptionFields.Add("IssuerCode".ToUpper(), x => x.GetIssuerCode());
//            OptionFields.Add("ModelName".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetModelName));
//            OptionFields.Add("NMUFlag".ToUpper(), x => x.GetNMUFlag());
//            OptionFields.Add("Notional".ToUpper(), x => x.GetNotional());
//            OptionFields.Add("NotionalInProduct".ToUpper(), x => x.GetNotionalInProduct());
//            OptionFields.Add("NumberOfFoliages".ToUpper(), x => x.GetNumberOfFoliages());
//            OptionFields.Add("NumberSharesPhysicallyDelivered".ToUpper(), x => x.GetNumberSharesPhysicallyDelivered());
//            OptionFields.Add("OptimisationType".ToUpper(), x => x.GetOptimisationType().SophisEnumToString());
//            OptionFields.Add("OptionFlagType".ToUpper(), x => x.GetOptionFlagType().SophisEnumToString());
//            OptionFields.Add("OptionStartDate".ToUpper(), x => x.GetOptionStartDate().ToDateTime());
//            OptionFields.Add("OptionStartRelativeDate".ToUpper(), x => x.GetOptionStartRelativeDate().ToDateTime());
//            OptionFields.Add("OverVolatility".ToUpper(), x => x.GetOverVolatility());
//            OptionFields.Add("Parity".ToUpper(), x => x.GetParity());
//            OptionFields.Add("ParityStockInTree".ToUpper(), x => x.GetParityStockInTree());
//            OptionFields.Add("PayOffClauseCount".ToUpper(), x => x.GetPayOffClauseCount());
//            OptionFields.Add("PayOffFormula".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetPayOffFormula));
//            OptionFields.Add("Proportion".ToUpper(), x => x.GetProportion());
//            OptionFields.Add("Quotity".ToUpper(), x => x.GetQuotity());
//            OptionFields.Add("Seniority".ToUpper(), x => x.GetSeniority().GetSeniorityName());
//            OptionFields.Add("SmileInDeltaGamma".ToUpper(), x => x.GetSmileInDeltaGamma().SophisEnumToString());
//            OptionFields.Add("SophisOptionModel".ToUpper(), x => x.GetSophisOptionModel());
//            OptionFields.Add("Spot".ToUpper(), x => x.GetSpot());
//            OptionFields.Add("SpreadType".ToUpper(), x => x.GetSpreadType().SophisEnumToString());
//            OptionFields.Add("StartDate".ToUpper(), x => x.GetStartDate().ToDateTime());
//            OptionFields.Add("SwaptionFamily".ToUpper(), x => x.GetSwaptionFamily());
//            OptionFields.Add("SwaptionIndex".ToUpper(), x => x.GetSwaptionIndex());
//            OptionFields.Add("SwaptionInterestBasis".ToUpper(), x => x.GetSwaptionInterestBasis().SophisEnumToString());
//            OptionFields.Add("SwaptionInterestMode".ToUpper(), x => x.GetSwaptionInterestMode().SophisEnumToString());
//            OptionFields.Add("SwaptionNotional".ToUpper(), x => x.GetSwaptionNotional());
//            OptionFields.Add("SwaptionSwapEndDate".ToUpper(), x => x.GetSwaptionSwapEndDate().ToDateTime());
//            OptionFields.Add("SwaptionSwapStartDate".ToUpper(), x => x.GetSwaptionSwapStartDate().ToDateTime());
//            OptionFields.Add("TradingUnits".ToUpper(), x => x.GetTradingUnits());
//            OptionFields.Add("Unit".ToUpper(), x => x.GetUnit());
//            OptionFields.Add("Validation".ToUpper(), x => x.GetValidation());
//            OptionFields.Add("VolatilityType".ToUpper(), x => x.GetVolatilityType().SophisEnumToString());
//            OptionFields.Add("YieldCalculationTypeForCB".ToUpper(), x => x.GetYieldCalculationTypeForCB().SophisEnumToString());
//        }

//        private static void SetupEquities()
//        {
//            EquityFields.Add("Beta".ToUpper(), x => x.GetBeta());
//            EquityFields.Add("TradingUnits".ToUpper(), x => x.GetTradingUnits());
//            EquityFields.Add("AccountingReferenceDate".ToUpper(), x => x.GetAccountingReferenceDate().ToDateTime());
//            EquityFields.Add("SharesOutstanding".ToUpper(), x => x.GetInstrumentCount());
//            EquityFields.Add("IssuerCode".ToUpper(), x => x.GetIssuerCode());
//        }

//        private static void SetupSwaps()
//        {
//            SwapFields.Add("QuotationUnit".ToUpper(), x => x.GetAskQuotationType().SophisEnumToString());
//            SwapFields.Add("CalculationAgent".ToUpper(), x => x.GetCalculationAgent());
//            SwapFields.Add("CodeForSpecificVolatility".ToUpper(), x => x.GetCodeForSpecificVolatility());
//            SwapFields.Add("Comment".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetComment));
//            SwapFields.Add("CurrencyCode".ToUpper(), x => x.GetCurrencyCode().GetCurrencyCode());
//            SwapFields.Add("DTCCConvention".ToUpper(), x => x.GetDTCCConvention().StringValue);
//            SwapFields.Add("EndDate".ToUpper(), x => x.GetEndDate().ToDateTime());
//            SwapFields.Add("Expiry".ToUpper(), x => x.GetExpiry().ToDateTime());
//            SwapFields.Add("Family".ToUpper(), x => x.GetFamily());
//            SwapFields.Add("FamilyName".ToUpper(), x => x.GetFamily().GetYieldCurveName());
//            SwapFields.Add("FamilySpread".ToUpper(), x => x.GetFamilySpread());
//            SwapFields.Add("HolidayAdjustmentType".ToUpper(), x => x.GetHolidayAdjustmentType().SophisEnumToString());
//            SwapFields.Add("InstrumentSpread".ToUpper(), x => x.GetInstrumentSpread());
//            SwapFields.Add("InterestRateSwapFloatingRate".ToUpper(), x => x.GetInterestRateSwapFloatingRate());
//            SwapFields.Add("IssueDate".ToUpper(), x => x.GetIssueDate().ToDateTime());
//            SwapFields.Add("IssuerCode".ToUpper(), x => x.GetIssuerCode());
//            SwapFields.Add("MarketCode".ToUpper(), x => x.GetMarketCode());
//            SwapFields.Add("ModelName".ToUpper(), x => DataTypeExtensions.GetStringFromMethod(x.GetModelName));
//            SwapFields.Add("Notional".ToUpper(), x => x.GetNotional());
//            SwapFields.Add("NotionalExchangeType".ToUpper(), x => x.GetNotionalExchangeType().SophisEnumToString());
//            SwapFields.Add("NotionalInProduct".ToUpper(), x => x.GetNotionalInProduct());
//            SwapFields.Add("PaymentGapType".ToUpper(), x => x.GetPaymentGapType().SophisEnumToString());
//            SwapFields.Add("Quotity".ToUpper(), x => x.GetQuotity());
//            SwapFields.Add("RollingType".ToUpper(), x => x.GetRollingType().SophisEnumToString());
//            SwapFields.Add("Seniority".ToUpper(), x => x.GetSeniority());
//            SwapFields.Add("SettlementAtDPlus".ToUpper(), x => x.GetSettlementAtDPlus());
//            SwapFields.Add("SettlementCurrency".ToUpper(), x => x.GetSettlementCurrency().GetCurrencyCode());
//            SwapFields.Add("SpreadType".ToUpper(), x => x.GetSpreadType().SophisEnumToString());
//            SwapFields.Add("StartDate".ToUpper(), x => x.GetStartDate().ToDateTime());
//            SwapFields.Add("StrikeUnit".ToUpper(), x => x.GetStrikeUnit());
//            SwapFields.Add("Unit".ToUpper(), x => x.GetUnit());
//            SwapFields.Add("VolatilityDependencyType".ToUpper(), x => x.GetVolatilityDependencyType().SophisEnumToString());

//            object GetFixedRate(CSMLeg leg)
//            {
//                CSMFixedLeg fl = leg; return fl is object ? (double?)fl.GetSettlementCurrencyType() : null;
//            }

//            SwapFields.Add("ReceivingLegFixedRate".ToUpper(), x => GetFixedRate(x.GetLeg(0)));
//            SwapFields.Add("PayingLegFixedRate".ToUpper(), x => GetFixedRate(x.GetLeg(1)));

//            SwapFields.Add("ReceivingLegFixedSettlement".ToUpper(), x => x.GetLeg(0).GetTypeOf().SophisEnumToString());
            
//            SwapFields.Add("ReceivingLegPayoff".ToUpper(), x => x.GetLeg(0).GetTypeOf().SophisEnumToString());
//            SwapFields.Add("ReceivingLegUnderlyingCode".ToUpper(), x => x.GetLeg(0).GetUnderlyingCode());
//            SwapFields.Add("ReceivingLegAdjustmentMethod".ToUpper(), x => x.GetLeg(0).GetAdjustmentMethod());
//            SwapFields.Add("ReceivingLegAutocallTrigger".ToUpper(), x => x.GetLeg(0).GetAutocallTrigger());
//            SwapFields.Add("ReceivingLegBrokenDate".ToUpper(), x => x.GetLeg(0).GetBrokenDate());
//            SwapFields.Add("ReceivingLegBrokenDateReference".ToUpper(), x => x.GetLeg(0).GetBrokenDateReference().SophisEnumToString());
//            SwapFields.Add("ReceivingLegCurrency".ToUpper(), x => x.GetLeg(0).GetCurrencyCode().GetCurrencyCode());
//            SwapFields.Add("ReceivingLegDayCountBasisType".ToUpper(), x => x.GetLeg(0).GetDayCountBasisType().SophisEnumToString());
//            SwapFields.Add("ReceivingLegDeliveryCurrency".ToUpper(), x => x.GetLeg(0).GetDeliveryCurrency().GetCurrencyCode());
//            SwapFields.Add("ReceivingLegFloatingRate".ToUpper(), x => x.GetLeg(0).GetFloatingRate());
//            SwapFields.Add("ReceivingLegForexFixing".ToUpper(), x => x.GetLeg(0).GetForexFixing());
//            SwapFields.Add("ReceivingLegForexFixingLag".ToUpper(), x => x.GetLeg(0).GetForexFixingLag());
//            SwapFields.Add("ReceivingLegForexOrder".ToUpper(), x => x.GetLeg(0).GetForexOrder().SophisEnumToString());
//            SwapFields.Add("ReceivingLegPaymentRule".ToUpper(), x => x.GetLeg(0).GetPaymentRule().StringValue);
            
//            SwapFields.Add("PayingLegPayoff".ToUpper(), x => x.GetLeg(1).GetTypeOf().SophisEnumToString());
//            SwapFields.Add("PayingLegUnderlyingCode".ToUpper(), x => x.GetLeg(1).GetUnderlyingCode());
//            SwapFields.Add("PayingLegAdjustmentMethod".ToUpper(), x => x.GetLeg(1).GetAdjustmentMethod());
//            SwapFields.Add("PayingLegAutocallTrigger".ToUpper(), x => x.GetLeg(1).GetAutocallTrigger());
//            SwapFields.Add("PayingLegBrokenDate".ToUpper(), x => x.GetLeg(1).GetBrokenDate());
//            SwapFields.Add("PayingLegBrokenDateReference".ToUpper(), x => x.GetLeg(1).GetBrokenDateReference().SophisEnumToString());
//            SwapFields.Add("PayingLegCurrency".ToUpper(), x => x.GetLeg(1).GetCurrencyCode().GetCurrencyCode());
//            SwapFields.Add("PayingLegDayCountBasisType".ToUpper(), x => x.GetLeg(1).GetDayCountBasisType().SophisEnumToString());
//            SwapFields.Add("PayingLegDeliveryCurrency".ToUpper(), x => x.GetLeg(1).GetDeliveryCurrency().GetCurrencyCode());
//            SwapFields.Add("PayingLegFloatingRate".ToUpper(), x => x.GetLeg(1).GetFloatingRate());
//            SwapFields.Add("PayingLegForexFixing".ToUpper(), x => x.GetLeg(1).GetForexFixing());
//            SwapFields.Add("PayingLegForexFixingLag".ToUpper(), x => x.GetLeg(1).GetForexFixingLag());
//            SwapFields.Add("PayingLegForexOrder".ToUpper(), x => x.GetLeg(1).GetForexOrder().SophisEnumToString());
//            SwapFields.Add("PayingLegPeriodicityType".ToUpper(), x => x.GetLeg(1).GetPeriodicityType());

//            SwapFields.Add("PayingLegPaymentRule".ToUpper(), x => x.GetLeg(1).GetPaymentRule().StringValue);
//        }
//    }
//}
