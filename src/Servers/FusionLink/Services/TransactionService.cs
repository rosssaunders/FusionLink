//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RxdSolutions.FusionLink.Helpers;
using RxdSolutions.FusionLink.Interface;
using sophis.backoffice_kernel;
using sophis.instrument;
using sophis.portfolio;
using sophis.static_data;
using sophis.utils;

namespace RxdSolutions.FusionLink.Services
{
    public class TransactionService
    {
        private readonly PositionService positionService;

        public TransactionService(PositionService positionService)
        {
            this.positionService = positionService;
        }

        public List<Transaction> GetPositionTransactions(int positionId, DateTime startDate, DateTime endDate)
        {
            _userNameCacheLookup.Clear();

            using var position = CSMPosition.GetCSRPosition(positionId);

            if(position is object)
            {
                var transactions = new CSMTransactionVector();
                position.GetTransactions(transactions);

                return GetTradesForPosition(startDate, endDate, transactions);
            }
            else
            {
                throw new PositionNotFoundException();
            }
        }

        public List<Transaction> GetPortfolioTransactions(int portfolioId, DateTime startDate, DateTime endDate)
        {
            using var portfolioCheck = CSMPortfolio.GetCSRPortfolio(portfolioId);
            if (portfolioCheck is null)
                throw new PortfolioNotFoundException();

            if (!portfolioCheck.IsLoaded())
                throw new PortfolioNotLoadedException();

            _userNameCacheLookup.Clear();

            var results = new List<Transaction>();

            var sql = $"DATENEG >= TO_DATE('{startDate.ToString("yyyyMMdd")}', 'YYYYMMDD') AND DATENEG <= TO_DATE('{endDate.ToString("yyyyMMdd")}', 'YYYYMMDD')";

            using var extraction = new CSMExtraction(sql, "GetPortfolioTransactions");

            //Add the Fund as the Entry point to the extraction
            var entryPoints = new ArrayList();
            extraction.GetEntryPointsList(entryPoints);
            entryPoints.Add(portfolioId);
            extraction.Create();

            using var portfolio = CSMPortfolio.GetCSRPortfolio(portfolioId, extraction);

            if (portfolio is null)
                return results;

            var positionCount = portfolio.GetFlatViewPositionCount();
            for (var x = 0; x < positionCount; x++)
            {
                using var position = portfolio.GetNthFlatViewPosition(x);
                using var transactionVector = new CSMTransactionVector();
                position.GetTransactions(transactionVector);

                var models = GetTradesForPosition(startDate, endDate, transactionVector);

                results.AddRange(models);
            }

            return results;
        }

        private List<Transaction> GetTradesForPosition(DateTime startDate, DateTime endDate, CSMTransactionVector transactions)
        {
            var tradesInRange = transactions.OfType<CSMTransaction>().Where(x =>
            {
                var dt = (DateTime)x.GetTransactionDate().ToDateTime();
                return (dt >= startDate && dt <= endDate);
            });

            return ConvertToModel(tradesInRange);
        }

        private Dictionary<int, string> _userNameCacheLookup = new Dictionary<int, string>();

        private string GetUserName(int id)
        {
            if(!_userNameCacheLookup.ContainsKey(id))
            {
                if (id == 0)
                    _userNameCacheLookup.Add(0, "MANAGER");
                else
                {
                    using var user = new CSMUserRights((uint)id); //This will trigger a SQL call so we need to cache the results

                    if(user is null)
                        _userNameCacheLookup.Add(id, "UNKNOWN");
                    else
                    {
                        using var name = user.GetName();
                        _userNameCacheLookup.Add(id, name.StringValue);
                    }
                }
            }

            return _userNameCacheLookup[id];
        }

        private string GetThirdPartyName(int id)
        {
            using var entity = CSMThirdParty.GetCSRThirdParty(id);

            if (entity is null)
                return "";

            using var name = entity.GetName();
            return name.StringValue;
        }

        private string GetInstrumentReference(int id)
        {
            using var instrument = CSMInstrument.GetInstance(id);

            if (instrument is null)
                return "";

            using var name = instrument.GetReference();
            return name.StringValue;
        }

        private string GetCurrencyCode(int id)
        {
            using var name = new CMString();
            CSMCurrency.CurrencyToString(id, name);
            return name.StringValue;
        }

        private string GetQuotationType(eMAskQuotationType eMAskQuotationType)
        {
            return eMAskQuotationType switch
            {
                eMAskQuotationType.M_adLastQuotationValidValue => "Last Quotation Valid Value",

                eMAskQuotationType.M_aqInAnotherCurrency => "In Another Currency",

                eMAskQuotationType.M_aqInPercentage => "In Percentage",

                eMAskQuotationType.M_aqInPercentWithAccrued => "Percent With Accrued",

                eMAskQuotationType.M_aqInPrice => "In Price",

                eMAskQuotationType.M_aqInPriceWithoutAccrued => "In Price Without Accrued",

                eMAskQuotationType.M_aqInRate => "In Rate",

                eMAskQuotationType.M_aqNotDefined => "Not Defined",

                eMAskQuotationType.M_aqUncertainMode => "Uncertain Mode",

                _ => "Unknown",
            };
        }

        private string GetPositionType(eMPositionType positionType)
        {
            return positionType switch
            {
                eMPositionType.M_pVirtualForValue => "Virtual For Value",

                eMPositionType.M_pVirtualCashPerCurrency => "Virtual Cash Per Currency",

                eMPositionType.M_pVirtualForNostroInterest => "Virtual For Nostro Interest",

                eMPositionType.M_pVirtual => "Virtual",

                eMPositionType.M_pVirtualMarginCall => "Virtual Margin Call",

                eMPositionType.M_pContractForDifference => "Contract For Difference",

                eMPositionType.M_pSecurityLoan => "Security Loan",

                eMPositionType.M_pUseArbitrageSimulation => "Use Arbitrage Simulation",

                eMPositionType.M_pUseLastSimulation => "Use Last Simulation",

                eMPositionType.M_pUseTheoreticalSimulation => "Use Theoretical Simulation",

                eMPositionType.M_pUseArbitrage => "Use Arbitrage",

                eMPositionType.M_pUseLast => "Use Last",

                eMPositionType.M_pUseTheoretical => "Use Theoretical",

                eMPositionType.M_pSimulatedVirtualForex => "Simulated Virtual Forex",

                eMPositionType.M_pVirtualForex => "Virtual Forex",

                eMPositionType.M_pBrokerage => "Brokerage",

                eMPositionType.M_pBasket => "Basket",

                eMPositionType.M_pSimulation => "Simulation",

                eMPositionType.M_pLended => "Lended",

                eMPositionType.M_pArbitrage => "Arbitrage",

                eMPositionType.M_pBlocked => "Blocked",

                eMPositionType.M_pStandard => "Standard",

                _ => "Unknown",
            };
        }

        private string GetPaymentMethod(int v)
        {
            return v.ToString();
        }

        private string GetPaymentCurrencyType(CSMTransaction.eMPaymentCurrencyType eMPaymentCurrencyType)
        {
            return eMPaymentCurrencyType switch
            {
                CSMTransaction.eMPaymentCurrencyType.M_pcPence => "Pence",

                CSMTransaction.eMPaymentCurrencyType.M_pcSettlement => "Settlement",

                CSMTransaction.eMPaymentCurrencyType.M_pcUnderlying => "Underlying",

                _ => "Unknown",
            };
        }

        private string GetForexCertaintyType(CSMTransaction.eMForexCertaintyType eMForexCertaintyType)
        {
            return eMForexCertaintyType switch
            {
                CSMTransaction.eMForexCertaintyType.M_fcUncertain => "Uncertain",

                CSMTransaction.eMForexCertaintyType.M_fcCertain => "Certain",

                _ => "Unknown",
            };
        }

        private string GetDeliveryType(eMBODeliveryType eMBODeliveryType)
        {
            return eMBODeliveryType switch
            {
                eMBODeliveryType.M_bdtNA => "NA",

                eMBODeliveryType.M_bdtFOP => "FOP",

                eMBODeliveryType.M_bdtDVP => "DVP",

                eMBODeliveryType.M_bdtAll => "All",

                _ => "Unknown",
            };
        }

        private string GetCreationKind(eMTransactionOriginType eMTransactionOriginType)
        {
            return eMTransactionOriginType switch
            {
                eMTransactionOriginType.M_toElectronic => "Electronic",

                eMTransactionOriginType.M_toAutomatic => "Automatic",

                eMTransactionOriginType.M_toManual => "Manual",

                _ => "Unknown",
            };
        }

        private string GetStatus(eMBackOfficeType backOfficeType)
        {
            using var s = new CSMKernelStatus((int)backOfficeType);
            using var name = s.GetName();
            return name.StringValue;
        }

        private string GetBusinessEvent(eMTransactionType transactionType)
        {
            using var be = CSMBusinessEvent.GetBusinessEventById((int)transactionType);
            using var name = be.GetName();
            return name.StringValue;
        }

        private List<Transaction> ConvertToModel(IEnumerable<CSMTransaction> transactions)
        {
            var modelTransactions = new List<Transaction>();
            foreach (CSMTransaction t in transactions)
            {
                Transaction model = ConvertToModel(t);

                modelTransactions.Add(model);
            }

            return modelTransactions;
        }

        private Transaction ConvertToModel(CSMTransaction t)
        {
            var model = new Transaction();

            model.AccountancyDate = t.GetAccountancyDate().ToDateTime();
            model.AccountingBook = t.GetAccountingBook();
            model.AccruedAmount = t.GetAccruedAmount();
            model.AccruedAmount2 = t.GetAccruedAmount2();
            model.AccruedCoupon = t.GetAccruedCoupon();
            model.AccruedCouponDate = t.GetAccruedCouponDate().ToDateTime();
            model.Adjustment = t.GetAdjustment();
            model.AskQuotationType = GetQuotationType(t.GetAskQuotationType());
            model.BackOfficeInfos = t.GetBackOfficeInfos();
            model.BackOfficeRef = t.GetBackOfficeRef();
            model.BackOfficeType = GetStatus(t.GetBackOfficeType());
            model.BasketInstrumentRef = t.GetBasketInstrumentRef();
            model.BasketInternalCode = t.GetBasketInternalCode();
            model.BasketQuantity = t.GetBasketQuantity();
            model.BenchmarkCode = t.GetBenchmarkCode();
            model.BlockTrade = t.GetBlockTrade();
            model.Broker = GetThirdPartyName(t.GetBroker());
            model.BrokerFees = t.GetBrokerFees();
            model.CashDepositary = GetThirdPartyName(t.GetCashDepositary());
            model.ClearingExceptionParty = GetThirdPartyName(t.GetClearingExceptionParty());
            model.ClearingHouse = GetThirdPartyName(t.GetClearingHouse());
            model.ClearingMember = GetThirdPartyName(t.GetClearingMember());
            model.Comment = t.GetComment();
            model.Commission = t.GetCommission();
            model.CommissionDate = t.GetCommissionDate().ToDateTime();
            model.ComponentCode = t.GetComponentCode();
            model.CompressionResult = t.GetCompressionResult();
            model.Counterparty = GetThirdPartyName(t.GetCounterparty());
            model.Counterparty2 = GetThirdPartyName(t.GetCounterparty2());
            model.CounterpartyFees = t.GetCounterpartyFees();
            model.CreationKind = GetCreationKind(t.GetCreationKind());
            model.CrossedReference = t.GetCrossedReference();
            model.DecisionMaker = GetUserName(t.GetDecisionMaker());
            model.DeliveryDate = t.GetDeliveryDate().ToDateTime();
            model.DeliveryType = GetDeliveryType(t.GetDeliveryType());
            model.Depositary = GetThirdPartyName(t.GetDepositary());
            model.DepositaryOfCounterparty = GetThirdPartyName(t.GetDepositaryOfCounterparty());
            model.Entity = GetThirdPartyName(t.GetEntity());
            model.ExecutionVenue = GetThirdPartyName(t.GetExecutionVenue());
            model.FolioCode = t.GetFolioCode();
            model.ForceLoad = t.GetForceLoad();
            model.ForexCertaintyType = GetForexCertaintyType(t.GetForexCertaintyType());
            model.ForexSpot = t.GetForexSpot();
            model.ForwardFixingDate = t.GetForwardFixingDate().ToDateTime();
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
            model.PariPassuDate = t.GetPariPassuDate().ToDateTime();
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
            model.SettlementDate = (DateTime)t.GetSettlementDate().ToDateTime();
            model.SettlementMethod = t.GetSettlementMethod().ToString();
            model.Spot = t.GetSpot();
            model.SophisOrderId = t.GetSophisOrderId();
            model.TradeRepository = GetThirdPartyName(t.GetTradeRepository());
            model.TradeYtm = t.GetTradeYtm();
            model.TransactionCode = t.GetTransactionCode();
            model.TransactionDate = (DateTime)t.GetTransactionDate().ToDateTime();
            model.TransactionTime = TimeSpan.FromSeconds(t.GetTransactionTime());
            model.TransactionType = GetBusinessEvent(t.GetTransactionType());
            model.TypeSpotCurrency = GetCurrencyCode(t.GetTypeSpotCurrency());

            return model;
        }
    }
}
