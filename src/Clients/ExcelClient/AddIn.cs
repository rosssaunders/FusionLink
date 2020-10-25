//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
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
        public static ServerConnectionMonitor AvailableConnections;

        public static bool IsShuttingDown;

        static AddIn()
        {
            Client = new DataServiceClient();
        }

        public AddIn()
        {
            Application.EnableVisualStyles();

            RegisterFunctions();

            // setup error handler
            ExcelIntegration.RegisterUnhandledExceptionHandler(ex => ex.ToString());

            var app = ExcelDnaUtil.Application as Microsoft.Office.Interop.Excel.Application;
            app.RTD.ThrottleInterval = 100;

            //Monitor for FusionLink connections
            AvailableConnections = new ServerConnectionMonitor();

            //Open the client connection
            ConnectionMonitor = new ConnectionMonitor(AvailableConnections);
            ConnectionMonitor.RegisterClient(Client);
        }

        public void AutoOpen()
        {
            ExcelComAddInHelper.LoadComAddIn(new ComAddIn(ConnectionMonitor, AvailableConnections));

            //Start the monitor
            AvailableConnections.FindAvailableServicesAsync().ContinueWith(result =>
            {
                if (IsShuttingDown)
                    return;

                var firstConnection = AvailableConnections.AvailableEndpoints.FirstOrDefault();
                if (firstConnection is object)
                    ConnectionMonitor.SetConnection(firstConnection.Via);

                CustomRibbon.Refresh();

                Client.OnConnectionStatusChanged += Client_OnConnectionStatusChanged;
            });
        }

        private void Client_OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            CustomRibbon.Refresh();
        }

        public void AutoClose()
        {
            ConnectionMonitor.Close();

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