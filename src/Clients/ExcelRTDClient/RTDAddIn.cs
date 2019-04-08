//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System.Reflection;
using ExcelDna.Integration;
using ExcelDna.Integration.RxExcel;

namespace RTD.Excel
{
    public class RTDAddIn : IExcelAddIn
    {
        static DataServiceClient client;

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
            client = new DataServiceClient();
            client.Open();
        }

        public void AutoClose()
        {
            client.Close();
        }

        [ExcelFunction("Gets Position values")]
        public static object GetPositionValue(int positionId, string column)
        {
            return ExcelAsyncUtil.Observe(nameof(GetPositionValue), new object[] { positionId, column }, () => new PositionExcelObservable(positionId, column, client));
        }

        [ExcelFunction("Gets portfolio values")]
        public static object GetPortfolioValue(int portfolioId, string column)
        {
            return ExcelAsyncUtil.Observe(nameof(GetPortfolioValue), new object[] { portfolioId, column }, () => new PortfolioExcelObservable(portfolioId, column, client));
        }
    }
}