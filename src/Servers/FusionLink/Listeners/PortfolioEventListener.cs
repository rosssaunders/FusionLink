//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    public class PortfolioEventListener : CSMPortfolioEvent, IPortfolioListener
    {
        public event EventHandler<PortfolioChangedEventArgs> PortfolioChanged;

        public override bool HasBeenCreated(CSMPortfolio portfolio)
        {
            return base.HasBeenCreated(portfolio);
        }

        public override void HasBeenDeleted(int code)
        {
            PortfolioChanged?.Invoke(this, new PortfolioChangedEventArgs(code, false));

            base.HasBeenDeleted(code);
        }

        public override void HasBeenModified(CSMPortfolio portfolio)
        {
            PortfolioChanged?.Invoke(this, new PortfolioChangedEventArgs(portfolio.GetCode(), false));

            base.HasBeenModified(portfolio);
        }

        public override void HasBeenTransferred(CSMPortfolio portfolio)
        {
            PortfolioChanged?.Invoke(this, new PortfolioChangedEventArgs(portfolio.GetCode(), false));

            base.HasBeenTransferred(portfolio);
        }
    }
}