//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Runtime.ExceptionServices;
using RxdSolutions.FusionLink.Helpers;
using sophis.portfolio;

namespace RxdSolutions.FusionLink.Provider
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