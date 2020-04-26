//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var a in args)
                Console.WriteLine(a);

            var testDataProvider = new TestDataServiceProvider();

            var dataServer = new RealTimeDataServer(testDataProvider);
            dataServer.OnClientConnectionChanged += Ds_OnClientConnectionChanged;

            //var host = DataServerHostFactory.CreateRealTimeServiceHost(dataServer);

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            //host.Close();
            //host = null;
        }

        private static void Ds_OnClientConnectionChanged(object sender, ClientConnectionChangedEventArgs e)
        {
            Console.WriteLine(e.Status);
        }
    }
}
