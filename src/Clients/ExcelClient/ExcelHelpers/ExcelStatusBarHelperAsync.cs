//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Threading.Tasks;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class ExcelStatusBarHelpers
    {
        public static void SetStatusBarWithResetDelay(this Microsoft.Office.Interop.Excel.Application app, string message, int delayInSeconds)
        {
            if(app != null)
            {
                app.StatusBar = message;
                Task.Delay(TimeSpan.FromSeconds(delayInSeconds))
                    .ContinueWith(_ => {

                        ExcelAsyncUtil.QueueAsMacro(() => app.StatusBar = false);

                    });
            }
        }

        public static void ResetStatusBar(this Microsoft.Office.Interop.Excel.Application app)
        {
            if(app != null)
            {
                ExcelAsyncUtil.QueueAsMacro(() => app.StatusBar = false);
            }
        }
    }
}