//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.Sophis2Excel;

namespace TestDataServer
{
    class Program
    {
        public static ServiceHost host;
        public static DataServer server;

        static void Main(string[] args)
        {
            var dataService = new TestDataServiceProvider();

            var baseAddress = new Uri("net.tcp://localhost:8080/SophisDataService");
            server = new DataServer(dataService);
            host = new ServiceHost(server, baseAddress);
            host.Open();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
