//  Copyright (c) RXD Solutions. All rights reserved.


using System.Collections;
using System.Collections.Generic;
using RxdSolutions.FusionLink.Interface;
using sophis.portfolio;

namespace RxdSolutions.FusionLink.Services
{
    public class PositionService
    {
        public List<int> GetPositions(int folioId, PositionsToRequest positions)
        {
            var results = new List<int>();

            using var portfolio = CSMPortfolio.GetCSRPortfolio(folioId);

            if (portfolio is object)
            {
                if (!portfolio.IsLoaded())
                {
                    throw new PortfolioNotLoadedException();
                }

                GetPositionsFromPortfolio(portfolio, positions, results);

                var allChildren = new ArrayList();
                portfolio.GetChildren(allChildren);

                for (int i = 0; i < allChildren.Count; i++)
                {
                    var current = allChildren[i] as CSMPortfolio;

                    if (current is object)
                    {
                        GetPositionsFromPortfolio(current, positions, results);
                    }
                }

                return results;
            }
            else
            {
                throw new PortfolioNotFoundException();
            }
        }

        private void GetPositionsFromPortfolio(CSMPortfolio portfolio, PositionsToRequest positions, List<int> results)
        {
            int positionCount = portfolio.GetTreeViewPositionCount();
            for (int i = 0; i < positionCount; i++)
            {
                using (var position = portfolio.GetNthTreeViewPosition(i))
                {
                    if (position.GetIdentifier() > 0) //Exclude Virtual positions
                    {
                        switch (positions)
                        {
                            case PositionsToRequest.All:
                                results.Add(position.GetIdentifier());
                                break;

                            case PositionsToRequest.Open:
                                if (position.GetInstrumentCount() != 0)
                                {
                                    results.Add(position.GetIdentifier());
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
