//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Runtime.ExceptionServices;
using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    internal class PortfolioDateValue : SystemValue
    {
        [HandleProcessCorruptedStateExceptions]
        public override object GetValue()
        {
            try
            {
                return CSMPortfolio.GetPortfolioDate().GetDateTime();
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}