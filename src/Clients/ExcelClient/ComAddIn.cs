//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration;
using ExcelDna.Integration.Extensibility;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ComAddIn : ExcelComAddIn
    {
        private readonly DataServiceClient client;
        private readonly ConnectionMonitor monitor;
        private readonly AvailableConnections availableConnections;

        public ComAddIn(DataServiceClient client, ConnectionMonitor monitor, AvailableConnections availableConnections)
        {
            this.client = client;
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
