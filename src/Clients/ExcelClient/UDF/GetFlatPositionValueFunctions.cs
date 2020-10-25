//  Copyright (c) RXD Solutions. All rights reserved.
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetFlatPositionValueFunctions
    {
        [ExcelFunction(Name = "GETFLATPOSITIONVALUE",
                       Description = "Returns flat position cell value",
                       HelpTopic = "Get-Flat-Position-Value")]
        public static object GetFlatPositionValue(int portfolioId, int instrumentId, string column)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            return ExcelAsyncUtil.Observe(nameof(GetFlatPositionValue), new object[] { portfolioId, instrumentId, column }, () => new FlatPositionValueExcelObservable(portfolioId, instrumentId, column, AddIn.Client));
        }

        [ExcelFunction(Name = "FPSV",
                       Description = "Returns flat position cell value",
                       HelpTopic = "Get-Flat-Position-Value")]
        public static object FPSV(int portfolioId, int instrumentId, string column)
        {
            return GetFlatPositionValue(portfolioId, instrumentId, column);
        }
    }
}
