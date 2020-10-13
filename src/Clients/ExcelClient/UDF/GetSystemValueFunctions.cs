//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetSystemValueFunctions
    {
        [ExcelFunction(Name = "GETSYSTEMVALUE", 
                       Description = "Returns the requested System value from FusionInvest",
                       HelpTopic = "Get-System-Value")]
        public static object GetSystemValue(string property)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            if (!Enum.TryParse(property, true, out SystemProperty enteredValue))
            {
                return ExcelError.ExcelErrorValue; // #VALUE
            }

            return ExcelAsyncUtil.Observe(nameof(GetSystemValue), new object[] { enteredValue }, () => new SystemPropertyExcelObservable(enteredValue, AddIn.Client));
        }

        [ExcelFunction(Name = "GETPORTFOLIODATE",
                       Description = "Returns the Portfolio Date of FusionInvest",
                       HelpTopic = "Get-Portfolio-Date")]
        public static object GetPortfolioDate()
        {
            return ExcelAsyncUtil.Observe(nameof(GetPortfolioDate), null, () => new SystemPropertyExcelObservable(SystemProperty.PortfolioDate, AddIn.Client));
        }
    }
}
