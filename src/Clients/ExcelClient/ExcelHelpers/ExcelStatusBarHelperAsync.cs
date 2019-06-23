//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Threading.Tasks;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ExcelStatusBarHelperAsync
    {
        public static void SetStatusBarWithResetDelay(string message, int delayInSeconds)
        {
            var app = ExcelDnaUtil.Application as Microsoft.Office.Interop.Excel.Application;
            app.StatusBar = message;
            Task.Delay(TimeSpan.FromSeconds(delayInSeconds))
                .ContinueWith(_ => {

                    ExcelAsyncUtil.QueueAsMacro(() => app.StatusBar = false);
                    
                });
        }

        public static void ResetStatusBar()
        {
            var app = ExcelDnaUtil.Application as Microsoft.Office.Interop.Excel.Application;
            ExcelAsyncUtil.QueueAsMacro(() => app.StatusBar = false);
        }
    }
}