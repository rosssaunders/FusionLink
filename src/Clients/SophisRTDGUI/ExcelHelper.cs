//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using sophis.portfolio;

namespace RxdSolutions.Sophis2Excel
{
    public static class ExcelHelper
    {
        public static string GetPositionFormula(long id, string column)
        {
            return $"=GetPositionValue({id}, \"{column}\")";
        }

        public static string GetPortfolioFormula(long id, string column)
        {
            return $"=GetPortfolioValue({id}, \"{column}\")";
        }
    }
}