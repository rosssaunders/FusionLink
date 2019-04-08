//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.ServiceModel;
using RxdSolutions.Sophis2Excel.Interface;

namespace RTD.Excel
{
    public class DataServiceClient : IDataServiceClient
    {
        private IDataServiceServer _server;

        public DataServiceClient()
        {

        }

        public void Open()
        {
            // setup connection to server

            var user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            var a = new EndpointAddress(
                    new Uri("net.pipe://localhost/SophisDataService"),
                    EndpointIdentity.CreateUpnIdentity(user));

            var b = new NetNamedPipeBinding();

            var f = new DuplexChannelFactory<IDataServiceServer>(this, b, a);
            _server = f.CreateChannel();
            _server.Register(); // this makes the server get a callback channel to us so it can call SendValue
        }

        public void Close()
        {
            _server.UnRegister();
        }

        public event EventHandler<PositionValueReceivedEventArgs> OnPositionValueReceived;
        public event EventHandler<PortfolioValueReceivedEventArgs> OnPortfolioValueReceived;

        public void SendPortfolioValue(int portfolioId, string column, DataTypeEnum type, object value)
        {
            // invert method call from WCF into event for Rx
            OnPortfolioValueReceived?.Invoke(this, new PortfolioValueReceivedEventArgs(portfolioId, column, type, value));
        }

        public void SendPositionValue(int positionId, string column, DataTypeEnum type, object value)
        {
            // invert method call from WCF into event for Rx
            OnPositionValueReceived?.Invoke(this, new PositionValueReceivedEventArgs(positionId, column, type, value));
        }

        public void SubscribeToPosition(int positionId, string column)
        {
            _server.SubscribeToPosition(positionId, column);
        }

        public void SubscribeToPortfolio(int folioId, string column)
        {
            _server.SubscribeToPortfolio(folioId, column);
        }
    }
}