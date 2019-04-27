//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ExcelDna.Integration;


namespace RxdSolutions.FusionLink.ExcelClient
{
    public class AddIn : IExcelAddIn
    {
        //The client needs to be static so the Excel functions (which must be static) can access it.
        public static DataServiceClient Client; 
        public static ConnectionMonitor ConnectionMonitor;

        public void AutoOpen()
        {
            // setup error handler
            ExcelIntegration.RegisterUnhandledExceptionHandler(ex => ex.ToString());

            var app = ExcelDnaUtil.Application as Microsoft.Office.Interop.Excel.Application;
            app.RTD.ThrottleInterval = 100;
            app.StatusBar = "Searching for available FusionLink servers. Please wait...";

            //Start the monitor
            ConnectionMonitor = new ConnectionMonitor();
            ConnectionMonitor.FindAvailableServices();
            ConnectionMonitor.Start();

            //Open the client connection
            Client = new DataServiceClient();
            ConnectionMonitor.RegisterClient(Client);

            ExcelComAddInHelper.LoadComAddIn(new ComAddIn(Client, ConnectionMonitor));
        }

        public void AutoClose()
        {
            ConnectionMonitor.Stop();

            Client?.Close();
        }
    }
}