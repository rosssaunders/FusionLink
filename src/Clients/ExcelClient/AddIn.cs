//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Linq;
using ExcelDna.Integration;
using ExcelDna.Registration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class AddIn : IExcelAddIn
    {
        //The client needs to be static so the Excel functions (which must be static) can access it.
        public static DataServiceClient Client; 
        public static ConnectionMonitor ConnectionMonitor;
        public static AvailableConnections AvailableConnections;

        public static bool IsShuttingDown;

        static AddIn()
        {
            Client = new DataServiceClient();
        }

        public void AutoOpen()
        {
            RegisterFunctions();

            // setup error handler
            ExcelIntegration.RegisterUnhandledExceptionHandler(ex => ex.ToString());

            var app = ExcelDnaUtil.Application as Microsoft.Office.Interop.Excel.Application;
            app.RTD.ThrottleInterval = 100;

            //Monitor for FusionLink connections
            AvailableConnections = new AvailableConnections();

            //Open the client connection
            ConnectionMonitor = new ConnectionMonitor(AvailableConnections);
            ConnectionMonitor.RegisterClient(Client);
            ExcelComAddInHelper.LoadComAddIn(new ComAddIn(Client, ConnectionMonitor, AvailableConnections));

            Client.OnConnectionStatusChanged += Client_OnConnectionStatusChanged;

            //Start the monitor
            AvailableConnections.FindAvailableServicesAsync().ContinueWith(result =>
            {
                if (IsShuttingDown)
                    return;

                ConnectionMonitor.Start();

                CustomRibbon.Refresh();
            });
        }

        private void Client_OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            CustomRibbon.Refresh();
        }

        public void AutoClose()
        {
            ConnectionMonitor.Stop();

            Client?.Close();
        }

        public void RegisterFunctions()
        {
            ExcelRegistration.GetExcelFunctions()
                             .Select(UpdateHelpTopic)
                             .RegisterFunctions();
        }

        public ExcelFunctionRegistration UpdateHelpTopic(ExcelFunctionRegistration funcReg)
        {
            funcReg.FunctionAttribute.Category = Resources.ExcelHelpCategory;
            funcReg.FunctionAttribute.HelpTopic = new Uri(new Uri(Resources.DocumentationBaseAddress), funcReg.FunctionAttribute.HelpTopic).ToString();

            return funcReg;
        }
    }
}