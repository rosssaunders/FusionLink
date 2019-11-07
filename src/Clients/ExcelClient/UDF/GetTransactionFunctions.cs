//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetTransactionFunctions
    {
        [ExcelFunction(Name = "GETTRANSACTIONS",
                       Description = "Returns a list of transactions for a given position.",
                       HelpTopic = "Get-Transactions")]
        public static object GetTransactions(
            [ExcelArgument(Name = "positionId", Description = "The Id of the position")]int positionId,
            [ExcelArgument(Name = "start_date", Description = "The start date")]DateTime startDate,
            [ExcelArgument(Name = "end_Date", Description = "The end date")]DateTime endDate)
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
                    object[,] array = new object[results.Count + 1, 5];

                    array[0, 0] = "TransactionCode";
                    array[0, 1] = "TransactionDate";
                    array[0, 2] = "TransactionTime";
                    array[0, 3] = "TransactionType";
                    array[0, 4] = "Quantity";

                    for (int i = 0; i < results.Count; i++)
                    {
                        array[i + 1, 0] = results[i].TransactionCode;
                        array[i + 1, 1] = results[i].TransactionDate;
                        array[i + 1, 2] = results[i].TransactionTime;
                        array[i + 1, 3] = results[i].TransactionType;
                        array[i + 1, 4] = results[i].Quantity;
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
