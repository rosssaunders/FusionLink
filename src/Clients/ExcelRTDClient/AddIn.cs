//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ExcelDna.Integration;
using static ExcelDna.Integration.XlCall;

namespace RxdSolutions.FusionLink.RTDClient
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

            ExcelComAddInHelper.LoadComAddIn(new ComAddIn(Client, ConnectionMonitor));

            app.StatusBar = "Searching for available FusionLink servers. Please wait...";

            //Start the monitor
            ConnectionMonitor = new ConnectionMonitor();
            ConnectionMonitor.FindAvailableServices();
            ConnectionMonitor.Start();

            //Open the client connection
            Client = new DataServiceClient();
            ConnectionMonitor.RegisterClient(Client);
        }

        public void AutoClose()
        {
            ConnectionMonitor.Stop();

            Client?.Close();
        }

        [ExcelFunction(Name = "GETPOSITIONS", Description = "Returns a list of position ids of the given portfolio", Category = "FusionLink")]
        public static object GetPositions(int portfolioId)
        {
            var positionIds = Client.GetPositions(portfolioId);

            double[,] array = new double[positionIds.Count,1];
            for (var i = 0; i < positionIds.Count; i++)
            {
                array[i, 0] = positionIds[i];
            }

            var caller = Excel(xlfCaller) as ExcelReference;
            if (caller == null)
                return array;

            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            if (rows == 0 || columns == 0)
                return array;

            if ((caller.RowLast - caller.RowFirst + 1 == rows) &&
                (caller.ColumnLast - caller.ColumnFirst + 1 == columns))
            {
                // Size is already OK - just return result
                return array;
            }

            var rowLast = caller.RowFirst + rows - 1;
            var columnLast = caller.ColumnFirst + columns - 1;

            // Check for the sheet limits
            if (rowLast > ExcelDnaUtil.ExcelLimits.MaxRows - 1 ||
                columnLast > ExcelDnaUtil.ExcelLimits.MaxColumns - 1)
            {
                // Can't resize - goes beyond the end of the sheet - just return #VALUE
                // (Can't give message here, or change cells)
                return ExcelError.ExcelErrorValue;
            }

            // TODO: Add some kind of guard for ever-changing result?
            ExcelAsyncUtil.QueueAsMacro(() => {
                // Create a reference of the right size
                var target = new ExcelReference(caller.RowFirst, rowLast, caller.ColumnFirst, columnLast, caller.SheetId);
                ExcelRangeResizer.DoResize(target); // Will trigger a recalc by writing formula
            });

            // Return what we have - to prevent flashing #N/A
            return array;
        }
    }
}