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
