//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using System.Linq;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.ExcelClient.Properties;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetPositionsFunction
    {
        [ExcelFunction(Name = "GETPOSITIONS", 
                       Description = "Returns a list of position ids of the given portfolio. By default only includes open positions.", 
                       HelpTopic = "Get-Positions")]
        public static object GetPositions([ExcelArgument(Name = "portfolio_id", Description = "The Portfolio Id")]object[] portfolioId,
                                          [ExcelArgument(Name = "include_all_positions", Description = "Include all positions")] bool includeAll = false)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            if (AddIn.Client.State != System.ServiceModel.CommunicationState.Opened)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.NotConnectedMessage));
            }

            try
            {
                if (portfolioId.Length == 0)
                    return ExcelError.ExcelErrorNA;

                if (portfolioId[0] == ExcelMissing.Value)
                    return ExcelError.ExcelErrorNA;

                var intIds = new HashSet<int>();
                foreach (var id in portfolioId)
                {
                    try
                    {
                        intIds.Add(Convert.ToInt32(id));
                    }
                    catch
                    {
                        return ExcelError.ExcelErrorNA;
                    }
                }

                var results = new HashSet<int>();
                foreach (var id in intIds)
                {
                    var resultsForPortfolio = AddIn.Client.GetPositions(id, includeAll ? PositionsToRequest.All : PositionsToRequest.Open);

                    foreach(var r in resultsForPortfolio)
                        results.Add(r);
                }

                int[] positionIds = results.OrderBy(x => x).ToArray();

                if (positionIds.Length == 0)
                {
                    return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.ExcelEmptyRange);
                }
                else
                {
                    object[,] array = new object[positionIds.Length, 1];
                    for (int i = 0; i < positionIds.Length; i++)
                    {
                        array[i, 0] = positionIds[i];
                    }

                    return ExcelRangeResizer.TransformToExcelRange(array);
                }
            }
            catch(Exception ex)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(ex.Message));
            }
        }
    }
}
