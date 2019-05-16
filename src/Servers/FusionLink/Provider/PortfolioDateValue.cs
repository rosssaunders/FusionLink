//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using RxdSolutions.FusionLink.Properties;
using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    internal class PortfolioDateValue : SystemValue
    {
        public override object GetValue()
        {
            return CSMPortfolio.GetPortfolioDate().GetDateTime();
        }
    }
}