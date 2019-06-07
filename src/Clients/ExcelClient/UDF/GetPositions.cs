using System.Collections.Generic;
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
                return Resources.NotConnectedMessage;
            }

            List<int> positionIds;
            try
            {
                positionIds = AddIn.Client.GetPositions(portfolioId, includeAll ? PositionsToRequest.All : PositionsToRequest.Open);
            }
            catch (PortfolioNotLoadedException)
            {
                return Resources.PortfolioNotLoadedMessage;
            }

            object[,] array = new object[positionIds.Count, 1];
            for (int i = 0; i < positionIds.Count; i++)
            {
                array[i, 0] = positionIds[i];
            }

            return ExcelRangeResizer.TransformToExcelRange(array);
        }
    }
}
