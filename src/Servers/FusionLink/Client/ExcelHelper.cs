//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

namespace RxdSolutions.FusionLink.Client
{
    public static class ExcelHelper
    {
        public static string GetPositionFormula(long id, string column)
        {
            return $"=GETPOSITIONVALUE({id}, \"{column}\")";
        }

        public static string GetPortfolioFormula(long id, string column)
        {
            return $"=GETPORTFOLIOVALUE({id}, \"{column}\")";
        }
    }
}