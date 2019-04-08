//  Copyright (c) RXD SOlutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Net.Security;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using RxdSolutions.Sophis2Excel.Interface;
using sophis;
using sophis.portfolio;

namespace RxdSolutions.Sophis2Excel
{
    public class Main : IMain
    {
        public static ServiceHost host;
        public static DataServer server;
        public static SynchronizationContext context;

        public void EntryPoint()
        {
            context = SynchronizationContext.Current;

            var dataService = new SophisDataServiceProvider(context);

            Task.Run(() => {

                var baseAddress = new Uri("net.pipe://localhost/");
                server = new DataServer(dataService);
                host = new ServiceHost(server, baseAddress);

                var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

                host.AddServiceEndpoint(typeof(IDataServiceServer), binding, "SophisDataService");

                host.Open();

            });
        }

        public void Close()
        {
            host.Close();

            GC.Collect();
        }
    }
}
