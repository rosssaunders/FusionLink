using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration;
using ExcelDna.Integration.Extensibility;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class ComAddIn : ExcelComAddIn
    {
        private readonly DataServiceClient client;
        private readonly ConnectionMonitor monitor;

        public ComAddIn(DataServiceClient client, ConnectionMonitor monitor)
        {
            this.client = client;
            this.monitor = monitor;
        }

        public override void OnDisconnection(ext_DisconnectMode RemoveMode, ref Array custom)
        {
            client.Close();
            client.Dispose();

            monitor.Stop();
            monitor.Dispose();
        }
    }
}
