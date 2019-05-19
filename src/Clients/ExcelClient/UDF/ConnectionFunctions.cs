using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class ConnectionFunctions
    {
        [ExcelFunction(Name = "GETCONNECTIONSTATUS", Description = "Returns the status of the connection to FusionInvest", Category = "FusionLink")]
        public static object GetConnectionStatus()
        {
            return ExcelAsyncUtil.Observe(nameof(GetConnectionStatus), null, () => new ConnectionStatusExcelObservable(AddIn.Client));
        }

        [ExcelFunction(Name = "GETAVAILABLECONNECTIONS", Description = "Returns a comma seperated list of the available connections", Category = "FusionLink")]
        public static object GetAvailableConnections()
        {
            return ExcelAsyncUtil.Observe(nameof(GetAvailableConnections), null, () => new AvailableConnectionsExcelObservable(AddIn.ConnectionMonitor));
        }

        [ExcelFunction(Name = "SETCONNECTION", Description = "Returns the status of the connection to FusionInvest", Category = "FusionLink")]
        public static void SetConnection(string connection)
        {
            if (!string.IsNullOrWhiteSpace(connection))
            {
                AddIn.ConnectionMonitor.SetConnection(new Uri(connection));
            }
        }

        [ExcelFunction(Name = "GETCONNECTION", Description = "Returns the connection string of the connection to FusionInvest", Category = "FusionLink")]
        public static object GetConnection()
        {
            return ExcelAsyncUtil.Observe(nameof(GetConnection), null, () => new ConnectionStringExcelObservable(AddIn.Client));
        }

        [ExcelFunction(Name = "GETCONNECTIONID", Description = "Returns the connection Id of the connection to FusionInvest", Category = "FusionLink")]
        public static object GetConnectionId()
        {
            return ExcelAsyncUtil.Observe(nameof(GetConnectionId), null, () => new ConnectionIdExcelObservable(AddIn.Client));
        }

        [ExcelFunction(Name = "GETSERVICESTATUS", Description = "Returns the status of the Service", Category = "FusionLink")]
        public static object GetServiceStatus()
        {
            return ExcelAsyncUtil.Observe(nameof(GetServiceStatus), null, () => new ServiceStatusExcelObservable(AddIn.Client));
        }
    }
}
