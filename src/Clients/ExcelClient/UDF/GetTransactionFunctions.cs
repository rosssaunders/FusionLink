//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetTransactionFunctions
    {
        private static Dictionary<string, Func<Transaction, object>> fieldLookup = new Dictionary<string, Func<Transaction, object>>();

        private static List<string> defaultFields = new List<string>();

        static GetTransactionFunctions()
        {
            defaultFields.Add("TransactionCode");
            defaultFields.Add("InstrumentCode");
            defaultFields.Add("Instrument");
            defaultFields.Add("TransactionDate");
            defaultFields.Add("TransactionTime");
            defaultFields.Add("Operator");
            defaultFields.Add("TransactionType");
            defaultFields.Add("Entity");
            defaultFields.Add("Depositary");
            defaultFields.Add("Broker");
            defaultFields.Add("Counterparty");
            defaultFields.Add("BrokerFees");
            defaultFields.Add("CounterpartyFees");
            defaultFields.Add("MarketFees");
            defaultFields.Add("Quantity");
            defaultFields.Add("Spot");
            defaultFields.Add("AskQuotationType");
            defaultFields.Add("GrossAmount");
            defaultFields.Add("NetAmount");
            defaultFields.Add("Comment");
            defaultFields.Add("BackOfficeInfos");
            defaultFields.Add("BackOfficeType");


            fieldLookup.Add("AccountancyDate", x => x.AccountancyDate);
            fieldLookup.Add("AccountingBook", x => x.AccountingBook);
            fieldLookup.Add("AccruedAmount", x => x.AccruedAmount);
            fieldLookup.Add("AccruedAmount2", x => x.AccruedAmount2);
            fieldLookup.Add("AccruedCoupon", x => x.AccruedCoupon);
            fieldLookup.Add("AccruedCouponDate", x => x.AccruedCouponDate);
            fieldLookup.Add("Adjustment", x => x.Adjustment);
            fieldLookup.Add("AskQuotationType", x => x.AskQuotationType);
            fieldLookup.Add("BackOfficeInfos", x => x.BackOfficeInfos);
            fieldLookup.Add("BackOfficeRef", x => x.BackOfficeRef);
            fieldLookup.Add("BackOfficeType", x => x.BackOfficeType);
            fieldLookup.Add("BasketInstrumentRef", x => x.BasketInstrumentRef);
            fieldLookup.Add("BasketInternalCode", x => x.BasketInternalCode);
            fieldLookup.Add("BasketQuantity", x => x.BasketQuantity);
            fieldLookup.Add("BenchmarkCode", x => x.BenchmarkCode);
            fieldLookup.Add("BlockTrade", x => x.BlockTrade);
            fieldLookup.Add("Broker", x => x.Broker);
            fieldLookup.Add("BrokerFees", x => x.BrokerFees);
            fieldLookup.Add("CashDepositary", x => x.CashDepositary);
            fieldLookup.Add("ClearingExceptionParty", x => x.ClearingExceptionParty);
            fieldLookup.Add("ClearingHouse", x => x.ClearingHouse);
            fieldLookup.Add("ClearingMember", x => x.ClearingMember);
            fieldLookup.Add("Comment", x => x.Comment);
            fieldLookup.Add("Commission", x => x.Commission);
            fieldLookup.Add("CommissionDate", x => x.CommissionDate);
            fieldLookup.Add("ComponentCode", x => x.ComponentCode);
            fieldLookup.Add("CompressionResult", x => x.CompressionResult);
            fieldLookup.Add("Counterparty", x => x.Counterparty);
            fieldLookup.Add("Counterparty2", x => x.Counterparty2);
            fieldLookup.Add("CounterpartyFees", x => x.CounterpartyFees);
            fieldLookup.Add("CreationKind", x => x.CreationKind);
            fieldLookup.Add("CrossedReference", x => x.CrossedReference);
            fieldLookup.Add("DecisionMaker", x => x.DecisionMaker);
            fieldLookup.Add("DeliveryDate", x => x.DeliveryDate);
            fieldLookup.Add("DeliveryType", x => x.DeliveryType);
            fieldLookup.Add("Depositary", x => x.Depositary);
            fieldLookup.Add("DepositaryOfCounterparty", x => x.DepositaryOfCounterparty);
            fieldLookup.Add("DestinationTable", x => x.DestinationTable);
            fieldLookup.Add("Entity", x => x.Entity);
            fieldLookup.Add("ExecutionVenue", x => x.ExecutionVenue);
            fieldLookup.Add("FolioCode", x => x.FolioCode);
            fieldLookup.Add("ForceLoad", x => x.ForceLoad);
            fieldLookup.Add("ForexCertaintyType", x => x.ForexCertaintyType);
            fieldLookup.Add("ForexSpot", x => x.ForexSpot);
            fieldLookup.Add("ForwardFixingDate", x => x.ForwardFixingDate);
            fieldLookup.Add("GrossAmount", x => x.GrossAmount);
            fieldLookup.Add("InitialMargin", x => x.InitialMargin);
            fieldLookup.Add("Instrument", x => x.Instrument);
            fieldLookup.Add("InstrumentCode", x => x.InstrumentCode);
            fieldLookup.Add("InvestmentStrategyId", x => x.InvestmentStrategyId);
            fieldLookup.Add("LostroCashId", x => x.LostroCashId);
            fieldLookup.Add("LostroPhysicalId", x => x.LostroPhysicalId);
            fieldLookup.Add("MarketFees", x => x.MarketFees);
            fieldLookup.Add("MirroringEnabler", x => x.MirroringEnabler);
            fieldLookup.Add("MirroringReference", x => x.MirroringReference);
            fieldLookup.Add("MirrorRule", x => x.MirrorRule);
            fieldLookup.Add("NetAmount", x => x.NetAmount);
            fieldLookup.Add("NostroCashId", x => x.NostroCashId);
            fieldLookup.Add("NostroPhysicalId", x => x.NostroPhysicalId);
            fieldLookup.Add("Notional", x => x.Notional);
            fieldLookup.Add("Operator", x => x.Operator);
            fieldLookup.Add("OrderId", x => x.OrderId);
            fieldLookup.Add("OrderReference", x => x.OrderReference);
            fieldLookup.Add("OtherTradeRepository", x => x.OtherTradeRepository);
            fieldLookup.Add("PariPassuDate", x => x.PariPassuDate);
            fieldLookup.Add("PaymentCurrencyType", x => x.PaymentCurrencyType);
            fieldLookup.Add("PaymentMethod", x => x.PaymentMethod);
            fieldLookup.Add("PoolFactor", x => x.PoolFactor);
            fieldLookup.Add("Position", x => x.Position);
            fieldLookup.Add("PositionType", x => x.PositionType);
            fieldLookup.Add("PsetId", x => x.PsetId);
            fieldLookup.Add("Quantity", x => x.Quantity);
            fieldLookup.Add("Reference", x => x.Reference);
            fieldLookup.Add("ReportingCtpy", x => x.ReportingCtpy);
            fieldLookup.Add("SettlementCurrency", x => x.SettlementCurrency);
            fieldLookup.Add("SettlementDate", x => x.SettlementDate);
            fieldLookup.Add("SettlementMethod", x => x.SettlementMethod);
            fieldLookup.Add("SophisOrderId", x => x.SophisOrderId);
            fieldLookup.Add("Spot", x => x.Spot);
            fieldLookup.Add("TradeRepository", x => x.TradeRepository);
            fieldLookup.Add("TradeYtm", x => x.TradeYtm);
            fieldLookup.Add("TransactionCode", x => x.TransactionCode);
            fieldLookup.Add("TransactionDate", x => x.TransactionDate);
            fieldLookup.Add("TransactionTime", x => x.TransactionTime.TotalSeconds / 86400d);
            fieldLookup.Add("TransactionType", x => x.TransactionType);
            fieldLookup.Add("TypeSpotCurrency", x => x.TypeSpotCurrency);
        }

        [ExcelFunction(Name = "GETTRANSACTIONS",
                       Description = "Returns a list of transactions for a given position.",
                       HelpTopic = "Get-Transactions")]
        public static object GetTransactions(
            [ExcelArgument(Name = "positionId", Description = "The Id of the position")]int positionId,
            [ExcelArgument(Name = "start_date", Description = "The start date")]DateTime startDate,
            [ExcelArgument(Name = "end_Date", Description = "The end date")]DateTime endDate,
            [ExcelArgument(Name = "extra_fields", Description = "Addition fields to display", AllowReference = false)]object[,] extraFields)
        {
            if (AddIn.Client.State != System.ServiceModel.CommunicationState.Opened)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.NotConnectedMessage));
            }

            try
            {
                var results = AddIn.Client.GetTransactions(positionId, startDate, endDate);
               
                if (results.Count == 0)
                {
                    return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.ExcelEmptyRange);
                }
                else
                {
                    var extraFieldsLength = 0;
                    if(!(extraFields[0, 0] == ExcelMissing.Value))
                    {
                        extraFieldsLength = extraFields.Length;
                    }

                    object[,] array = new object[results.Count + 1, defaultFields.Count + extraFields.Length];

                    var k = 0;
                    foreach (var fld in defaultFields)
                    {
                        array[0, k++] = fld;
                    }
                    
                    if(extraFieldsLength > 0)
                        foreach(var ef in extraFields)
                        {
                            array[0, k++] = ef;
                        }

                    for (int i = 0; i < results.Count; i++)
                    {
                        var t = results[i];
                        int j = 0;

                        foreach (var fld in defaultFields)
                        {
                            array[i + 1, j++] = fieldLookup[fld](t); ;
                        }

                        if (extraFieldsLength > 0)
                            foreach (var ef in extraFields)
                            {
                                if(fieldLookup.ContainsKey(ef.ToString()))
                                {
                                    array[i + 1, j++] = fieldLookup[ef.ToString()](t);
                                }
                                else
                                {
                                    array[i + 1, j++] = "Unknown field";
                                }
                            }
                    }

                    return ExcelRangeResizer.TransformToExcelRange(array);
                }
            }
            catch (Exception ex)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(ex.Message));
            }
        }
    }
}
