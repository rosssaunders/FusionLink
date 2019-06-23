//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Linq;
using ExcelDna.Integration;
using ExcelDna.Registration;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class AddIn : IExcelAddIn
    {
        //The client needs to be static so the Excel functions (which must be static) can access it.
        public static DataServiceClient Client; 
        public static ConnectionMonitor ConnectionMonitor;

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

            //Open the client connection
            ConnectionMonitor = new ConnectionMonitor();
            ConnectionMonitor.RegisterClient(Client);
            ExcelComAddInHelper.LoadComAddIn(new ComAddIn(Client, ConnectionMonitor));

            app.StatusBar = Resources.SearchingForServersMessage;

            //Start the monitor
            ConnectionMonitor.FindAvailableServicesAsync().ContinueWith(result =>
            {
                ConnectionMonitor.Start();

                CustomRibbon.Refresh(); //Inform Excel to refresh the UI

                app.StatusBar = false;
            });
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