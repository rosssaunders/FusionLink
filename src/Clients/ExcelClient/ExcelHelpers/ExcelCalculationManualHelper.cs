//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public sealed class ExcelCalculationManualHelper : XlCall, IDisposable
    {
        readonly object oldCalculationMode;

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