//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class ExcelCalculationManualHelper : XlCall, IDisposable
    {
        object oldCalculationMode;

        public ExcelCalculationManualHelper()
        {
            oldCalculationMode = Excel(xlfGetDocument, 14);
            Excel(xlcOptionsCalculation, 3);
        }

        public void Dispose()
        {
            Excel(xlcOptionsCalculation, oldCalculationMode);
        }
    }
}