//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class ExcelStaticData
    {
        public static readonly DateTime ExcelMinDate = new DateTime(1899, 12, 30);

        public static readonly object[,] ExcelEmptyRange = new object[1, 1] { { ExcelEmpty.Value } };

        public static object[,] GetExcelRangeError(string message)
        {
            return new object[1, 1] { { message } };
        }
    }
}
