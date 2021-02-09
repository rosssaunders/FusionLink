//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using sophis.portfolio;
using sophis.tools;

namespace RxdSolutions.FusionLink.Listeners
{
    public class PortfolioActionListener : CSMPortfolioAction
    {
        public static event EventHandler<PortfolioChangedEventArgs> PortfolioChanged;

        public override void NotifyCreated(CSMPortfolio portfolio, CSMEventVector message)
        {
            PortfolioChanged?.Invoke(this, new PortfolioChangedEventArgs(portfolio.GetCode(), true));

            base.NotifyCreated(portfolio, message);
        }

        public override void NotifyDeleted(CSMPortfolio portfolio, CSMEventVector message)
        {
            PortfolioChanged?.Invoke(this, new PortfolioChangedEventArgs(portfolio.GetCode(), true));

            base.NotifyDeleted(portfolio, message);
        }

        public override void NotifyModified(CSMPortfolio portfolio, CSMEventVector message)
        {
            PortfolioChanged?.Invoke(this, new PortfolioChangedEventArgs(portfolio.GetCode(), true));

            base.NotifyModified(portfolio, message);
        }

        public override void NotifyTransferred(CSMPortfolio portfolio, CSMEventVector message)
        {
            PortfolioChanged?.Invoke(this, new PortfolioChangedEventArgs(portfolio.GetCode(), true));

            base.NotifyTransferred(portfolio, message);
        }
    }
}