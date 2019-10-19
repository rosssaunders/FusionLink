//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using sophis.portfolio;

namespace RxdSolutions.FusionLink.Listeners
{
    public class PortfolioEventListener : CSMPortfolioEvent
    {
        public static event EventHandler<PortfolioChangedEventArgs> PortfolioChanged;

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