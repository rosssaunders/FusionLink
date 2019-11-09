//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public class Transaction
    {
        [DataMember]
        public object AccountancyDate { get; set; }
        
        [DataMember]
        public int AccountingBook { get; set; }
        
        [DataMember]
        public double AccruedAmount { get; set; }
        
        [DataMember]
        public double AccruedAmount2 { get; set; }
        
        [DataMember]
        public double AccruedCoupon { get; set; }
        
        [DataMember]
        public object AccruedCouponDate { get; set; }
        
        [DataMember]
        public int Adjustment { get; set; }
        
        [DataMember]
        public string AskQuotationType { get; set; }
        
        [DataMember] 
        public string BackOfficeInfos { get; set; }
        
        [DataMember] 
        public int BackOfficeRef { get; set; }
        
        [DataMember] 
        public string BackOfficeType { get; set; }
        
        [DataMember] 
        public int BasketInstrumentRef { get; set; }
        
        [DataMember] 
        public long BasketInternalCode { get; set; }
        
        [DataMember] 
        public int BasketQuantity { get; set; }
        
        [DataMember] 
        public int BenchmarkCode { get; set; }
        
        [DataMember] 
        public short BlockTrade { get; set; }
        
        [DataMember] 
        public string Broker { get; set; }
        
        [DataMember] 
        public double BrokerFees { get; set; }
        
        [DataMember] 
        public string CashDepositary { get; set; }
        
        [DataMember] 
        public int ClearingExceptionParty { get; set; }
        
        [DataMember] 
        public int ClearingHouse { get; set; }
        
        [DataMember] 
        public int ClearingMember { get; set; }
        
        [DataMember] 
        public string Comment { get; set; }
        
        [DataMember] 
        public double Commission { get; set; }
        
        [DataMember] 
        public object CommissionDate { get; set; }
        
        [DataMember] 
        public int ComponentCode { get; set; }
        
        [DataMember] 
        public short CompressionResult { get; set; }
        
        [DataMember] 
        public string Counterparty { get; set; }
        
        [DataMember] 
        public string Counterparty2 { get; set; }
        
        [DataMember] 
        public double CounterpartyFees { get; set; }
        
        [DataMember] 
        public string CreationKind { get; set; }
        
        [DataMember] 
        public long CrossedReference { get; set; }
        
        [DataMember] 
        public string DecisionMaker { get; set; }
        
        [DataMember] 
        public object DeliveryDate { get; set; }
        
        [DataMember] 
        public string DeliveryType { get; set; }
        
        [DataMember] 
        public string Depositary { get; set; }
        
        [DataMember] 
        public string DepositaryOfCounterparty { get; set; }
        
        [DataMember] 
        public string DestinationTable { get; set; }
        
        [DataMember] 
        public string Entity { get; set; }
        
        [DataMember] 
        public string ExecutionVenue { get; set; }
        
        [DataMember] 
        public int FolioCode { get; set; }
        
        [DataMember] 
        public int ForceLoad { get; set; }
        
        [DataMember] 
        public string ForexCertaintyType { get; set; }
        
        [DataMember] 
        public double ForexSpot { get; set; }
        
        [DataMember] 
        public object ForwardFixingDate { get; set; }
        
        [DataMember] 
        public double GrossAmount { get; set; }
        
        [DataMember] 
        public double InitialMargin { get; set; }
        
        [DataMember] 
        public string Instrument { get; set; }
        
        [DataMember] 
        public int InstrumentCode { get; set; }
        
        [DataMember] 
        public int InvestmentStrategyId { get; set; }
        
        [DataMember] 
        public int LostroCashId { get; set; }
        
        [DataMember] 
        public int LostroPhysicalId { get; set; }
        
        [DataMember] 
        public double MarketFees { get; set; }
        
        [DataMember] 
        public string MirroringEnabler { get; set; }
        
        [DataMember] 
        public long MirroringReference { get; set; }
        
        [DataMember] 
        public int MirrorRule { get; set; }
        
        [DataMember] 
        public double NetAmount { get; set; }
        
        [DataMember] 
        public int NostroCashId { get; set; }
        
        [DataMember] 
        public int NostroPhysicalId { get; set; }
        
        [DataMember] 
        public double Notional { get; set; }
        
        [DataMember] 
        public string Operator { get; set; }
        
        [DataMember] 
        public int OrderId { get; set; }
        
        [DataMember] 
        public string OrderReference { get; set; }
        
        [DataMember] 
        public string OtherTradeRepository { get; set; }
        
        [DataMember] 
        public object PariPassuDate { get; set; }
        
        [DataMember] 
        public string PaymentCurrencyType { get; set; }
        
        [DataMember] 
        public string PaymentMethod { get; set; }
        
        [DataMember] 
        public double PoolFactor { get; set; }
        
        [DataMember] 
        public int Position { get; set; }
        
        [DataMember] 
        public string PositionType { get; set; }
        
        [DataMember] 
        public int PsetId { get; set; }
        
        [DataMember] 
        public double Quantity { get; set; }
        
        [DataMember] 
        public long Reference { get; set; }
        
        [DataMember] 
        public string ReportingCtpy { get; set; }
        
        [DataMember] 
        public string SettlementCurrency { get; set; }
        
        [DataMember] 
        public DateTime SettlementDate { get; set; }
        
        [DataMember] 
        public string SettlementMethod { get; set; }
        
        [DataMember] 
        public int SophisOrderId { get; set; }

        [DataMember]
        public double Spot { get; set; }

        [DataMember] 
        public string TradeRepository { get; set; }
        
        [DataMember] 
        public double TradeYtm { get; set; }
        
        [DataMember] 
        public long TransactionCode { get; set; }
        
        [DataMember] 
        public DateTime TransactionDate { get; set; }
        
        [DataMember] 
        public TimeSpan TransactionTime { get; set; }
        
        [DataMember] 
        public string TransactionType { get; set; }
        
        [DataMember] 
        public string TypeSpotCurrency { get; set; }
    }
}
