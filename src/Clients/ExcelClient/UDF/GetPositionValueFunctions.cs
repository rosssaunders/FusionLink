//  Copyright (c) RXD Solutions. All rights reserved.
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetPositionValueFunctions
    {
        [ExcelFunction(Name = "GETPOSITIONVALUE",
                       Description = "Returns position cell value",
                       Category = "Get-Position-Value")]
        public static object GetPositionValue(int positionId, string column)
        {
            return ExcelAsyncUtil.Observe(nameof(GetPositionValue), new object[] { positionId, column }, () => new PositionValueExcelObservable(positionId, column, AddIn.Client));
        }

        [ExcelFunction(Name = "PSV",
                       Description = "Returns position cell value",
                       Category = "Get-Position-Value")]
        public static object PSV(int positionId, string column)
        {
            return GetPositionValue(positionId, column);
        }
    }
}
