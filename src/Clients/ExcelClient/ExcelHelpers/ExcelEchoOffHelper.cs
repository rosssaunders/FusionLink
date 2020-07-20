//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    // RIIA-style helpers to deal with Excel selections    
    public sealed class ExcelEchoOffHelper : XlCall, IDisposable
    {
        readonly object oldEcho;

        public ExcelEchoOffHelper()
        {
            oldEcho = Excel(xlfGetWorkspace, 40);
            Excel(xlcEcho, false);
        }

        public void Dispose()
        {
            Excel(xlcEcho, oldEcho);
        }
    }
}