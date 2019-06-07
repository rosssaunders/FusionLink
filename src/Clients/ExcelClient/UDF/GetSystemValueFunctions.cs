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
            if (!Enum.TryParse(property, out SystemProperty enteredValue))
            {
                return ExcelError.ExcelErrorValue; // #VALUE
            }

            return ExcelAsyncUtil.Observe(nameof(GetSystemValue), new object[] { enteredValue }, () => new SystemPropertyExcelObservable(enteredValue, AddIn.Client));
        }
    }
}
