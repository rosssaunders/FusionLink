//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.misc;
using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    public class FusionCapitalGlobalFunctions : CSMGlobalFunctions, IGlobalFunctions
    {
        public event EventHandler<PortfolioCalculationEndedEventArgs> PortfolioCalculationEnded;

        public event EventHandler<PortfolioAdditionEndedEventArgs> PortfolioAdditionEnded;

        public override void EndPortfolioCalculation(CSMExtraction extraction, int folioId)
        {
            PortfolioCalculationEnded?.Invoke(this, new PortfolioCalculationEndedEventArgs(extraction, folioId, CSMPortfolioColumn.GetRefreshVersion(), this.fInPortfolioCalculation));

            base.EndPortfolioCalculation(extraction, folioId);
        }

        public override void EndPortfolioAddition(CSMExtraction extraction, int folioId)
        {
            PortfolioAdditionEnded?.Invoke(this, new PortfolioAdditionEndedEventArgs(extraction, folioId, CSMPortfolioColumn.GetRefreshVersion()));

            base.EndPortfolioAddition(extraction, folioId);
        }
    }
}