//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Diagnostics;
using sophis.instrument;
using sophis.portfolio;
using sophis.utils;
using sophis.value;

namespace RxdSolutions.FusionLink
{
    public class FusionInvestGlobalFunctions : CSMAmGlobalFunctions, IGlobalFunctions
    {
        public event EventHandler<PortfolioCalculationEndedEventArgs> PortfolioCalculationEnded;

        public event EventHandler<PortfolioAdditionEndedEventArgs> PortfolioAdditionEnded;

        public override void EndPortfolioCalculation(CSMExtraction extraction, int folioId)
        {
            PortfolioCalculationEnded?.Invoke(this, new PortfolioCalculationEndedEventArgs(extraction, folioId, CSMPortfolioColumn.GetRefreshVersion(), this.fInPortfolioCalculation, this.m_Mode, this.fLoadingPortfolio));

            base.EndPortfolioCalculation(extraction, folioId);
        }

        public override void EndPortfolioAddition(CSMExtraction extraction, int folioId)
        {
            PortfolioAdditionEnded?.Invoke(this, new PortfolioAdditionEndedEventArgs(extraction, folioId, CSMPortfolioColumn.GetRefreshVersion(), this.m_Mode, this.fLoadingPortfolio));

            base.EndPortfolioAddition(extraction, folioId);
        }
    }
}