//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.ExcelClient.Properties;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetPriceHistoryFunction
    {
        [ExcelFunction(Name = "GETPRICEHISTORY", 
                       Description = "Returns the price history for the given Instrument reference.", 
                       HelpTopic = "Get-Price-History")]
        public static object GetPriceHistory([ExcelArgument(Name = "instrument_id", Description = "The instrument Reference (either Reference or Sicovam)")]object reference,
                                             [ExcelArgument(Name = "start_date", Description = "Start Date")]DateTime startDate,
                                             [ExcelArgument(Name = "end_date", Description = "End Date")]DateTime endDate)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            if (startDate == ExcelStaticData.ExcelMinDate)
                startDate = DateTime.MinValue;

            if (endDate == ExcelStaticData.ExcelMinDate)
                endDate = DateTime.MaxValue;

            if(startDate > endDate)
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
                    prices = AddIn.Client.GetInstrumentPriceHistory(Convert.ToInt32(reference), startDate, endDate);
                }
                else
                {
                    prices = AddIn.Client.GetInstrumentPriceHistory((string)reference, startDate, endDate);
                }

                if (prices.Count == 0)
                {
                    return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.ExcelEmptyRange);
                }
                else
                {
                    object[,] array = ConvertPriceHistoryToExcel(prices);

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
        public static object GetCurrencyPriceHistory([ExcelArgument(Name = "instrument_id", Description = "The currency reference (either Reference or Currency Id)")]object reference,
                                             [ExcelArgument(Name = "start_date", Description = "Start Date")]DateTime startDate,
                                             [ExcelArgument(Name = "end_date", Description = "End Date")]DateTime endDate)
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
                    object[,] array = ConvertPriceHistoryToExcel(prices);

                    return ExcelRangeResizer.TransformToExcelRange(array);
                }
            }
            catch (Exception ex)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(ex.Message));
            }
        }

        private static object[,] ConvertPriceHistoryToExcel(List<PriceHistory> prices)
        {
            object[,] array = new object[prices.Count + 1, 9];

            array[0, 0] = "Date";
            array[0, 1] = "First";
            array[0, 2] = "High";
            array[0, 3] = "Low";
            array[0, 4] = "Last";
            array[0, 5] = "Theoretical";
            array[0, 6] = "Bid";
            array[0, 7] = "Ask";
            array[0, 8] = "Volume";

            for (int i = 0; i < prices.Count; i++)
            {
                array[i + 1, 0] = prices[i].Date;
                array[i + 1, 1] = prices[i].First ?? 0;
                array[i + 1, 2] = prices[i].High ?? 0;
                array[i + 1, 3] = prices[i].Low ?? 0;
                array[i + 1, 4] = prices[i].Last ?? 0;
                array[i + 1, 5] = prices[i].Theoretical ?? 0;
                array[i + 1, 6] = prices[i].Bid ?? 0;
                array[i + 1, 7] = prices[i].Ask ?? 0;
                array[i + 1, 8] = prices[i].Volume ?? 0;
            }

            return array;
        }
    }
}
