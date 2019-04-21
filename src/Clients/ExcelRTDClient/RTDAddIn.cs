//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Linq;
using System.Reflection;
using ExcelDna.Integration;
using static ExcelDna.Integration.XlCall;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class RTDAddIn : IExcelAddIn
    {
        //The client needs to be static so the Excel functions (which must be static) can access it.
        public static DataServiceClient Client; 
        static ConnectionMonitor _connectionMonitor;

        public void AutoOpen()
        {
            // setup error handler
            ExcelIntegration.RegisterUnhandledExceptionHandler(ex => ex.ToString());

            // increase RTD refresh rate since the 2 seconds default is too slow (move as setting to ribbon later)
            object app;
            object rtd;
            app = ExcelDnaUtil.Application;
            rtd = app.GetType().InvokeMember("RTD", BindingFlags.GetProperty, null, app, null);
            rtd.GetType().InvokeMember("ThrottleInterval", BindingFlags.SetProperty, null, rtd, new object[] { 100 });

            //Open the client connection
            Client = new DataServiceClient();
            Client.FindAvailableServices();

            //Start the monitor
            _connectionMonitor = new ConnectionMonitor(Client);

            if(Client.AvailableEndpoints.Count > 0)
            {
                _connectionMonitor.SetConnection(Client.AvailableEndpoints[0].Uri);
            }
            
            _connectionMonitor.Start();
        }

        public void AutoClose()
        {
            _connectionMonitor.Stop();

            Client.Close();
        }

        [ExcelFunction("Gets Position values")]
        public static object GetPositionValue(int positionId, string column)
        {
            return ExcelAsyncUtil.Observe(nameof(GetPositionValue), new object[] { positionId, column }, () => new PositionValueExcelObservable(positionId, column, Client));
        }

        [ExcelFunction("Gets Portfolio values")]
        public static object GetPortfolioValue(int portfolioId, string column)
        {
            return ExcelAsyncUtil.Observe(nameof(GetPortfolioValue), new object[] { portfolioId, column }, () => new PortfolioValueExcelObservable(portfolioId, column, Client));
        }

        [ExcelFunction("Returns the status of the connection to FusionInvest")]
        public static object GetConnectionStatus()
        {
            return ExcelAsyncUtil.Observe(nameof(GetConnectionStatus), null, () => new ConnectionStatusExcelObservable(Client));
        }

        [ExcelFunction("Returns the status of the connection to FusionInvest")]
        public static object GetAvailableConnections()
        {
            Client.FindAvailableServices();

            return string.Join(",", Client.AvailableEndpoints.Select(x => x.Uri.ToString()));
        }

        [ExcelFunction("Returns the status of the connection to FusionInvest")]
        public static void SetConnection(string connection)
        {
            if(!string.IsNullOrWhiteSpace(connection))
            {
                _connectionMonitor.SetConnection(new Uri(connection));
            }
        }

        [ExcelFunction("Returns the connection string of the connection to FusionInvest")]
        public static object GetConnection()
        {
            return ExcelAsyncUtil.Observe(nameof(GetConnection), null, () => new ConnectionExcelObservable(Client));
        }

        [ExcelFunction("Returns the Portfolio Date of FusionInvest")]
        public static object GetPortfolioDate()
        {
            return ExcelAsyncUtil.Observe(nameof(GetPortfolioDate), null, () => new PortfolioDateExcelObservable(Client));
        }

        [ExcelFunction("Returns a list of position ids of the given portfolio")]
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