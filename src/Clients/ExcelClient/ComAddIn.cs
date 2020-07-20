//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using ExcelDna.Integration.Extensibility;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ComAddIn : ExcelComAddIn
    {
        private readonly ConnectionMonitor monitor;
        private readonly AvailableConnections availableConnections;

        public ComAddIn(ConnectionMonitor monitor, AvailableConnections availableConnections)
        {
            this.monitor = monitor;
            this.availableConnections = availableConnections;
        }

        public override void OnDisconnection(ext_DisconnectMode RemoveMode, ref Array custom)
        {
            AddIn.IsShuttingDown = true;

            monitor.Stop();
            monitor.Dispose();

            availableConnections.Dispose();
        }
    }
}
