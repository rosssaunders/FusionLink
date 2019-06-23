//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Threading.Tasks;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ExcelStatusBarHelper : XlCall, IDisposable
    {
        Microsoft.Office.Interop.Excel.Application app;

        public ExcelStatusBarHelper(string message)
        {
            app = ExcelDnaUtil.Application as Microsoft.Office.Interop.Excel.Application;
            app.StatusBar = message;
        }

        public void Dispose()
        {
            app.StatusBar = false;
        }
    }

    public class ExcelStatusBarHelper2
    {
        public static void SetStatusBarWithResetDelay(string message, int delayInSeconds)
        {
            var app = ExcelDnaUtil.Application as Microsoft.Office.Interop.Excel.Application;
            app.StatusBar = message;
            Task.Delay(TimeSpan.FromSeconds(delayInSeconds))
                .ContinueWith(_ => { app.StatusBar = false; });
        }
    }
}