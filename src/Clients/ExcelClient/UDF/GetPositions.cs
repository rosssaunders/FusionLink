//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
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
        public static object GetPositions([ExcelArgument(Name = "portfolio_id", Description = "The Portfolio Id")]int portfolioId,
                                          [ExcelArgument(Name = "include_all_positions", Description = "Include all positions")] bool includeAll = false)
        {
            if (AddIn.Client.State != System.ServiceModel.CommunicationState.Opened)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.NotConnectedMessage));
            }

            try
            {
                var results = AddIn.Client.GetPositions(portfolioId, includeAll ? PositionsToRequest.All : PositionsToRequest.Open);
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
            catch (PortfolioNotFoundException)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.PortfolioNotFoundMessage));
            }
            catch (PortfolioNotLoadedException)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.PortfolioNotLoadedMessage));
            }
            catch(Exception ex)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(ex.Message));
            }
        }
    }
}
