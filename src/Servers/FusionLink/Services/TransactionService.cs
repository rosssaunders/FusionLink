//  Copyright (c) RXD Solutions. All rights reserved.


using System;
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

                return ConvertToModel(transactions);
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

        private List<Transaction> ConvertToModel(CSMTransactionVector transactions)
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
                model.AskQuotationType = t.GetAskQuotationType().ToString();
                model.BackOfficeInfos = t.GetBackOfficeInfos();
                model.BackOfficeRef = t.GetBackOfficeRef();
                model.BackOfficeType = t.GetBackOfficeType().ToString();
                model.BasketInstrumentRef = t.GetBasketInstrumentRef();
                model.BasketInternalCode = t.GetBasketInternalCode();
                model.BasketQuantity = t.GetBasketQuantity();
                model.BenchmarkCode = t.GetBenchmarkCode();
                model.BlockTrade = t.GetBlockTrade();
                model.Broker = t.GetBroker();
                model.BrokerFees = t.GetBrokerFees();
                model.CashDepositary = t.GetCashDepositary();
                model.ClearingExceptionParty = t.GetClearingExceptionParty();
                model.ClearingHouse = t.GetClearingHouse();
                model.ClearingMember = t.GetClearingMember();
                model.Comment = t.GetComment();
                model.Commission = t.GetCommission();
                model.CommissionDate = t.GetCommissionDate().GetDateTime();
                model.ComponentCode = t.GetComponentCode();
                model.CompressionResult = t.GetCompressionResult();
                model.Counterparty = t.GetCounterparty();
                model.Counterparty2 = t.GetCounterparty2();
                model.CounterpartyFees = t.GetCounterpartyFees();
                model.CreationKind = t.GetCreationKind().ToString();
                model.CrossedReference = t.GetCrossedReference();
                model.DecisionMaker = GetUserName(t.GetDecisionMaker());
                model.DeliveryDate = t.GetDeliveryDate().GetDateTime();
                model.DeliveryType = t.GetDeliveryType().ToString();
                model.Depositary = GetThirdPartyName(t.GetDepositary());
                model.DepositaryOfCounterparty = GetThirdPartyName(t.GetDepositaryOfCounterparty());
                model.DestinationTable = t.GetDestinationTable();
                model.Entity = GetThirdPartyName(t.GetEntity());
                model.ExecutionVenue = GetThirdPartyName(t.GetExecutionVenue());
                model.FolioCode = t.GetFolioCode();
                model.ForceLoad = t.GetForceLoad();
                model.ForexCertaintyType = t.GetForexCertaintyType().ToString();
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
                model.PaymentCurrencyType = t.GetPaymentCurrencyType().ToString();
                model.PaymentMethod = t.GetPaymentMethod().ToString();
                model.PoolFactor = t.GetPoolFactor();
                model.Position = t.GetPositionID();
                model.PositionType = t.GetPositionType().ToString();
                model.PsetId = t.GetPsetId();
                model.Quantity = t.GetQuantity();
                model.Reference = t.GetReference();
                model.ReportingCtpy = GetThirdPartyName(t.GetReportingCtpy());
                model.SettlementCurrency = GetCurrencyCode(t.GetSettlementCurrency());
                model.SettlementDate = (DateTime)t.GetSettlementDate().GetDateTime();
                model.SettlementMethod = t.GetSettlementMethod().ToString();
                model.SophisOrderId = t.GetSophisOrderId();
                model.TradeRepository = GetThirdPartyName(t.GetTradeRepository());
                model.TradeYtm = t.GetTradeYtm();
                model.TransactionCode = t.GetTransactionCode();
                model.TransactionDate = (DateTime)t.GetTransactionDate().GetDateTime();
                model.TransactionTime = t.GetTransactionTime();
                model.TransactionType = t.GetTransactionType().ToString();
                model.TypeSpotCurrency = GetCurrencyCode(t.GetTypeSpotCurrency());

                modelTransactions.Add(model);
            }

            return modelTransactions;
        }
    }
}
