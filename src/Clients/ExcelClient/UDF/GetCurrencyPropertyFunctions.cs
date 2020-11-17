//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.ExcelClient.Properties;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetCurrencyPropertyFunctions
    {
        [ExcelFunction(Name = "GETCURRENCYPROPERTY",
                       Description = "Returns an Currency Property",
                       HelpTopic = "Get-Currency-Property")]
        public static object GetCurrencyProperty(
            [ExcelArgument(Name = "currency", Description = "The instrument id or reference")]object instrument,
            [ExcelArgument(Name = "property", Description = "The instrument property to subscribe to")]string property)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            return ExcelAsyncUtil.Observe(nameof(GetCurrencyProperty), new object[] { instrument, property }, () => new CurrencyPropertyExcelObservable(instrument, property, AddIn.Client));
        }

        [ExcelFunction(Name = "GETCURRENCYSET",
               Description = "Returns an Currency Set",
               HelpTopic = "Get-Currency-Set")]
        public static object GetCurrencySet(
            [ExcelArgument(Name = "currency", Description = "The currency reference (either Reference or Currency Id)")] object currency,
            [ExcelArgument(Name = "property", Description = "The property")] string property)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            if (AddIn.Client.State != System.ServiceModel.CommunicationState.Opened)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.NotConnectedMessage));
            }

            try
            {
                DataTable results;
                if (currency is double)
                {
                    results = AddIn.Client.GetCurrencySet(Convert.ToInt32(currency), property);
                }
                else
                {
                    results = AddIn.Client.GetCurrencySet((string)currency, property);
                }

                if (results.Rows.Count == 0)
                {
                    return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.ExcelEmptyRange);
                }
                else
                {
                    object[,] array = DataHelper.ConvertDataTableToExcel(results);

                    return ExcelRangeResizer.TransformToExcelRange(array);
                }
            }
            catch (Exception ex)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(ex.Message));
            }
        }

        [ExcelFunction(Name = "GETCURRENCYPRICEHISTORY",
                       Description = "Returns the price history for the given currency reference.",
                       HelpTopic = "Get-Currency-Price-History")]
        public static object GetCurrencyPriceHistory([ExcelArgument(Name = "instrument_id", Description = "The currency reference (either Reference or Currency Id)")] object reference,
                                             [ExcelArgument(Name = "start_date", Description = "Start Date")] DateTime startDate,
                                             [ExcelArgument(Name = "end_date", Description = "End Date")] DateTime endDate)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            if (startDate == ExcelStaticData.ExcelMinDate)
                startDate = DateTime.MinValue;

            if (endDate == ExcelStaticData.ExcelMinDate)
                endDate = DateTime.MaxValue;

            if (startDate > endDate)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.StartDateGreaterThanEndDateMessage));
            }

            if (AddIn.Client.State != System.ServiceModel.CommunicationState.Opened)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.NotConnectedMessage));
            }

            try
            {
                List<PriceHistory> prices;
                if (reference is double)
                {
                    prices = AddIn.Client.GetCurrencyPriceHistory(Convert.ToInt32(reference), startDate, endDate);
                }
                else
                {
                    prices = AddIn.Client.GetCurrencyPriceHistory((string)reference, startDate, endDate);
                }

                if (prices.Count == 0)
                {
                    return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.ExcelEmptyRange);
                }
                else
                {
                    object[,] array = DataHelper.ConvertPriceHistoryToExcel(prices);

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
