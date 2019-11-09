//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Linq;
using System.Collections.Generic;
using RxdSolutions.FusionLink.Helpers;
using RxdSolutions.FusionLink.Interface;
using sophis.backoffice_kernel;
using sophis.gui;
using sophis.instrument;
using sophis.market_data;
using sophis.portfolio;
using sophis.static_data;
using sophis.utils;

namespace RxdSolutions.FusionLink.Services
{
    public class TransactionService
    {
        public List<Transaction> GetTransactions(int positionId, DateTime startDate, DateTime endDate)
        {
            using (var position = CSMPosition.GetCSRPosition(positionId))
            using (var transactions = new CSMTransactionVector())
            {
                position.GetTransactions(transactions);

                var tradesInRange = transactions.OfType<CSMTransaction>().Where(x =>
                {
                    var dt = (DateTime)x.GetTransactionDate().GetDateTime();
                    return (dt >= startDate && dt <= endDate);
                });

                return ConvertToModel(tradesInRange);
            }
        }

        private string GetUserName(int id)
        {
            using (var decisionMaker = new CSMUserRights((uint)id))
            using (var name = decisionMaker.GetName())
            {
                return name.StringValue;
            }
        }

        private string GetThirdPartyName(int id)
        {
            using (var entity = new CSMThirdParty(id))
            using (var name = entity.GetName())
            {
                return name.StringValue;
            }
        }

        private string GetInstrumentReference(int id)
        {
            using (var entity = CSMInstrument.GetInstance(id))
            using (var name = entity.GetReference())
            {
                return name.StringValue;
            }
        }

        private string GetCurrencyCode(int id)
        {
            using (var name = new CMString())
            {
                CSMCurrency.CurrencyToString(id, name);
                return name.StringValue;
            }
        }

        private string GetQuotationType(eMAskQuotationType eMAskQuotationType)
        {
            switch(eMAskQuotationType)
            {
                case eMAskQuotationType.M_adLastQuotationValidValue:
                    return "Last Quotation Valid Value";

                case eMAskQuotationType.M_aqInAnotherCurrency:
                    return "In Another Currency";

                case eMAskQuotationType.M_aqInPercentage:
                    return "In Percentage";

                case eMAskQuotationType.M_aqInPercentWithAccrued:
                    return "Percent With Accrued";

                case eMAskQuotationType.M_aqInPrice:
                    return "In Price";

                case eMAskQuotationType.M_aqInPriceWithoutAccrued:
                    return "In Price Without Accrued";

                case eMAskQuotationType.M_aqInRate:
                    return "In Rate";

                case eMAskQuotationType.M_aqNotDefined:
                    return "Not Defined";

                case eMAskQuotationType.M_aqUncertainMode:
                    return "Uncertain Mode";

                default:
                    return "Unknown";
            }
        }

        private string GetPositionType(eMPositionType positionType)
        {
            switch (positionType)
            {
                case eMPositionType.M_pVirtualForValue:
                    return "Virtual For Value";
                
                case eMPositionType.M_pVirtualCashPerCurrency:
                    return "Virtual Cash Per Currency";
                
                case eMPositionType.M_pVirtualForNostroInterest:
                    return "Virtual For Nostro Interest";
                
                case eMPositionType.M_pVirtual:
                    return "Virtual";
                
                case eMPositionType.M_pVirtualMarginCall:
                    return "Virtual Margin Call";
                
                case eMPositionType.M_pContractForDifference:
                    return "Contract For Difference";
                
                case eMPositionType.M_pSecurityLoan:
                    return "Security Loan";
                
                case eMPositionType.M_pUseArbitrageSimulation:
                    return "Use Arbitrage Simulation";
                
                case eMPositionType.M_pUseLastSimulation:
                    return "Use Last Simulation";
                
                case eMPositionType.M_pUseTheoreticalSimulation:
                    return "Use Theoretical Simulation";
                
                case eMPositionType.M_pUseArbitrage:
                    return "Use Arbitrage";
                
                case eMPositionType.M_pUseLast:
                    return "Use Last";
                
                case eMPositionType.M_pUseTheoretical:
                    return "Use Theoretical";
                
                case eMPositionType.M_pSimulatedVirtualForex:
                    return "Simulated Virtual Forex";
                
                case eMPositionType.M_pVirtualForex:
                    return "Virtual Forex";
                
                case eMPositionType.M_pBrokerage:
                    return "Brokerage";
                
                case eMPositionType.M_pBasket:
                    return "Basket";
                
                case eMPositionType.M_pSimulation:
                    return "Simulation";
                
                case eMPositionType.M_pLended:
                    return "Lended";
                
                case eMPositionType.M_pArbitrage:
                    return "Arbitrage";
                
                case eMPositionType.M_pBlocked:
                    return "Blocked";
                
                case eMPositionType.M_pStandard:
                    return "Standard";
            }

            return "Unknown";
        }

        private string GetPaymentMethod(int v)
        {
            return v.ToString();
        }

        private string GetPaymentCurrencyType(CSMTransaction.eMPaymentCurrencyType eMPaymentCurrencyType)
        {
            switch (eMPaymentCurrencyType)
            {
                case CSMTransaction.eMPaymentCurrencyType.M_pcPence:
                    return "Pence";
                
                case CSMTransaction.eMPaymentCurrencyType.M_pcSettlement:
                    return "Settlement";
                
                case CSMTransaction.eMPaymentCurrencyType.M_pcUnderlying:
                    return "Underlying";
            }

            return "Unknown";
        }

        private string GetForexCertaintyType(CSMTransaction.eMForexCertaintyType eMForexCertaintyType)
        {
            switch (eMForexCertaintyType)
            {
                case CSMTransaction.eMForexCertaintyType.M_fcUncertain:
                    return "Uncertain";
                
                case CSMTransaction.eMForexCertaintyType.M_fcCertain:
                    return "Certain";
            }

            return "Unknown";
        }

        private string GetDeliveryType(eMBODeliveryType eMBODeliveryType)
        {
            switch (eMBODeliveryType)
            {
                case eMBODeliveryType.M_bdtNA:
                    return "NA";
                
                case eMBODeliveryType.M_bdtFOP:
                    return "FOP";
                
                case eMBODeliveryType.M_bdtDVP:
                    return "DVP";
                
                case eMBODeliveryType.M_bdtAll:
                    return "All";
            }

            return "Unknown";
        }

        private string GetCreationKind(eMTransactionOriginType eMTransactionOriginType)
        {
            switch (eMTransactionOriginType)
            {
                case eMTransactionOriginType.M_toElectronic:
                    return "Electronic";
                
                case eMTransactionOriginType.M_toAutomatic:
                    return "Automatic";
                
                case eMTransactionOriginType.M_toManual:
                    return "Manual";
            }

            return "Unknown";
        }

        private List<Transaction> ConvertToModel(IEnumerable<CSMTransaction> transactions)
        {
            var modelTransactions = new List<Transaction>();
            foreach (CSMTransaction t in transactions)
            {
                var model = new Transaction();

                model.AccountancyDate = t.GetAccountancyDate().GetDateTime();
                model.AccountingBook = t.GetAccountingBook();
                model.AccruedAmount = t.GetAccruedAmount();
                model.AccruedAmount2 = t.GetAccruedAmount2();
                model.AccruedCoupon = t.GetAccruedCoupon();
                model.AccruedCouponDate = t.GetAccruedCouponDate().GetDateTime();
                model.Adjustment = t.GetAdjustment();

                model.AskQuotationType = GetQuotationType(t.GetAskQuotationType());
                model.BackOfficeInfos = t.GetBackOfficeInfos();
                model.BackOfficeRef = t.GetBackOfficeRef();

                using (var s = new CSMKernelStatus((int)t.GetBackOfficeType()))
                using (var name = s.GetName())
                {
                    model.BackOfficeType = name.ToString();
                }

                model.BasketInstrumentRef = t.GetBasketInstrumentRef();
                model.BasketInternalCode = t.GetBasketInternalCode();
                model.BasketQuantity = t.GetBasketQuantity();
                model.BenchmarkCode = t.GetBenchmarkCode();
                model.BlockTrade = t.GetBlockTrade();
                model.Broker = GetThirdPartyName(t.GetBroker());
                model.BrokerFees = t.GetBrokerFees();
                model.CashDepositary = GetThirdPartyName(t.GetCashDepositary());
                model.ClearingExceptionParty = t.GetClearingExceptionParty();
                model.ClearingHouse = t.GetClearingHouse();
                model.ClearingMember = t.GetClearingMember();
                model.Comment = t.GetComment();
                model.Commission = t.GetCommission();
                model.CommissionDate = t.GetCommissionDate().GetDateTime();
                model.ComponentCode = t.GetComponentCode();
                model.CompressionResult = t.GetCompressionResult();
                model.Counterparty = GetThirdPartyName(t.GetCounterparty());
                model.Counterparty2 = GetThirdPartyName(t.GetCounterparty2());
                model.CounterpartyFees = t.GetCounterpartyFees();
                model.CreationKind = GetCreationKind(t.GetCreationKind());
                model.CrossedReference = t.GetCrossedReference();
                model.DecisionMaker = GetUserName(t.GetDecisionMaker());
                model.DeliveryDate = t.GetDeliveryDate().GetDateTime();
                model.DeliveryType = GetDeliveryType(t.GetDeliveryType());
                model.Depositary = GetThirdPartyName(t.GetDepositary());
                model.DepositaryOfCounterparty = GetThirdPartyName(t.GetDepositaryOfCounterparty());
                model.DestinationTable = t.GetDestinationTable();
                model.Entity = GetThirdPartyName(t.GetEntity());
                model.ExecutionVenue = GetThirdPartyName(t.GetExecutionVenue());
                model.FolioCode = t.GetFolioCode();
                model.ForceLoad = t.GetForceLoad();
                model.ForexCertaintyType = GetForexCertaintyType(t.GetForexCertaintyType());
                model.ForexSpot = t.GetForexSpot();
                model.ForwardFixingDate = t.GetForwardFixingDate().GetDateTime();
                model.GrossAmount = t.GetGrossAmount();
                model.InitialMargin = t.GetInitialMargin();
                model.Instrument = GetInstrumentReference(t.GetInstrumentCode());
                model.InstrumentCode = t.GetInstrumentCode();
                model.InvestmentStrategyId = t.GetInvestmentStrategyId();
                model.LostroCashId = t.GetLostroCashId();
                model.LostroPhysicalId = t.GetLostroPhysicalId();
                model.MarketFees = t.GetMarketFees();
                model.MirroringReference = t.GetMirroringReference();
                model.MirrorRule = t.GetMirrorRule();
                model.NetAmount = t.GetNetAmount();
                model.NostroCashId = t.GetNostroCashId();
                model.NostroPhysicalId = t.GetNostroPhysicalId();
                model.Notional = t.GetNotional();
                model.Operator = GetUserName(t.GetOperator());
                model.OrderId = t.GetOrderId();
                model.OrderReference = t.GetOrderReference();
                model.OtherTradeRepository = GetThirdPartyName(t.GetOtherTradeRepository());
                model.PariPassuDate = t.GetPariPassuDate().GetDateTime();
                model.PaymentCurrencyType = GetPaymentCurrencyType(t.GetPaymentCurrencyType());
                model.PaymentMethod = GetPaymentMethod(t.GetPaymentMethod());
                model.PoolFactor = t.GetPoolFactor();
                model.Position = t.GetPositionID();
                model.PositionType = GetPositionType(t.GetPositionType());
                model.PsetId = t.GetPsetId();
                model.Quantity = t.GetQuantity();
                model.Reference = t.GetReference();
                model.ReportingCtpy = GetThirdPartyName(t.GetReportingCtpy());
                model.SettlementCurrency = GetCurrencyCode(t.GetSettlementCurrency());
                model.SettlementDate = (DateTime)t.GetSettlementDate().GetDateTime();
                model.SettlementMethod = t.GetSettlementMethod().ToString();
                model.Spot = t.GetSpot();
                model.SophisOrderId = t.GetSophisOrderId();
                model.TradeRepository = GetThirdPartyName(t.GetTradeRepository());
                model.TradeYtm = t.GetTradeYtm();
                model.TransactionCode = t.GetTransactionCode();
                model.TransactionDate = (DateTime)t.GetTransactionDate().GetDateTime();
                model.TransactionTime = TimeSpan.FromSeconds(t.GetTransactionTime());

                using (var be = CSMBusinessEvent.GetBusinessEventById((int)t.GetTransactionType()))
                using (var name = be.GetName())
                    model.TransactionType = name.StringValue;
                
                model.TypeSpotCurrency = GetCurrencyCode(t.GetTypeSpotCurrency());

                modelTransactions.Add(model);
            }

            return modelTransactions;
        }
    }
}
