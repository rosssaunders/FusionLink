//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetPortfolioPropertyFunctions
    {
        [ExcelFunction(Name = "GETPORTFOLIOPROPERTY",
               Description = "Returns a Portfolio Property",
               HelpTopic = "Get-Portfolio-Property")]
        public static object GetPortfolioProperty(
            [ExcelArgument(Name = "portfolio_Id", Description = "The portfolio id")]int portfolioId,
            [ExcelArgument(Name = "property", Description = "The portfolio property to subscribe to")]string property)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            if (!Enum.TryParse(property,true, out PortfolioProperty enteredValue))
            {
                return ExcelError.ExcelErrorValue; // #VALUE
            }

            return ExcelAsyncUtil.Observe(nameof(GetPortfolioProperty), new object[] { portfolioId, enteredValue }, () => new PortfolioPropertyExcelObservable(portfolioId, enteredValue, AddIn.Client));
        }
    }
}
