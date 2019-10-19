//  Copyright (c) RXD Solutions. All rights reserved.


using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class ConnectionFunctions
    {
        [ExcelFunction(Name = "GETCONNECTIONSTATUS", 
                       Description = "Returns the status of the connection to FusionInvest",
                       HelpTopic = "Get-Connection-Status")]
        public static object GetConnectionStatus()
        {
            return ExcelAsyncUtil.Observe(nameof(GetConnectionStatus), null, () => new ConnectionStatusExcelObservable(AddIn.Client));
        }

        [ExcelFunction(Name = "GETAVAILABLECONNECTIONS", 
                       Description = "Returns a comma separated list of the available connections",
                       HelpTopic = "Get-Available-Connections")]
        public static object GetAvailableConnections()
        {
            return ExcelAsyncUtil.Observe(nameof(GetAvailableConnections), null, () => new AvailableConnectionsExcelObservable(AddIn.AvailableConnections));
        }

        [ExcelFunction(Name = "GETCONNECTION", 
                       Description = "Returns the connection name of the connection to FusionInvest",
                       HelpTopic = "Get-Connection")]
        public static object GetConnection()
        {
            return ExcelAsyncUtil.Observe(nameof(GetConnection), null, () => new ConnectionNameExcelObservable(AddIn.Client, AddIn.ConnectionMonitor, AddIn.AvailableConnections));
        }

        [ExcelFunction(Name = "GETCONNECTIONID", 
                       Description = "Returns the connection Id of the connection to FusionInvest",
                       HelpTopic = "Get-Connection-Id")]
        public static object GetConnectionId()
        {
            return ExcelAsyncUtil.Observe(nameof(GetConnectionId), null, () => new ConnectionIdExcelObservable(AddIn.Client));
        }

        [ExcelFunction(Name = "GETSERVICESTATUS", 
                       Description = "Returns the status of the Service",
                       HelpTopic = "Get-Service-Status")]
        public static object GetServiceStatus()
        {
            return ExcelAsyncUtil.Observe(nameof(GetServiceStatus), null, () => new ServiceStatusExcelObservable(AddIn.Client));
        }

        [ExcelFunction(Name = "GETLASTMESSAGERECEIVEDTIME",
                       Description = "Returns the status of the Service",
                       HelpTopic = "Get-Last-Message-Received-Time")]
        public static object GetLastMessageReceivedTime()
        {
            return ExcelAsyncUtil.Observe(nameof(GetLastMessageReceivedTime), null, () => new LastMessageReceivedTimeObservable(AddIn.Client));
        }
    }
}
