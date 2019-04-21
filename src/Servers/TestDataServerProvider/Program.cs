//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink;

namespace RxdSolutions.FusionLink
{
    class Program
    {
        public ServiceHost host;

        static void Main(string[] args)
        {
            foreach (var a in args)
                Console.WriteLine(a);

            var testDataProvider = new TestDataServiceProvider();

            var dataServer = new DataServer(testDataProvider);
            dataServer.OnClientConnectionChanged += Ds_OnClientConnectionChanged;

            var host = DataServerHostFactory.Create(dataServer);

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            host.Close();
            host = null;
        }

        private static void Ds_OnClientConnectionChanged(object sender, ClientConnectionChangedEventArgs e)
        {
            Console.WriteLine(e.Status);
        }
    }
}
