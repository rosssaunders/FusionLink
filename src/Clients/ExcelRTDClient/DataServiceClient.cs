//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class DataServiceClient : IDisposable
    {
        private IDataServiceServer _server;
        private DataServiceClientCallback _callback;

        private readonly HashSet<(int Id, string Column)> _positionSubscriptions;
        private readonly HashSet<(int Id, string Column)> _portfolioSubscriptions;
        private readonly HashSet<SystemProperty> _systemSubscriptions;

        public event EventHandler<ConnectionStatusChangedEventArgs> OnConnectionStatusChanged;
        public event EventHandler<PositionValueReceivedEventArgs> OnPositionValueReceived;
        public event EventHandler<PortfolioValueReceivedEventArgs> OnPortfolioValueReceived;
        public event EventHandler<SystemValueReceivedEventArgs> OnSystemValueReceived;

        public DataServiceClient()
        {
            _positionSubscriptions = new HashSet<(int, string)>();
            _portfolioSubscriptions = new HashSet<(int, string)>();
            _systemSubscriptions = new HashSet<SystemProperty>();
        }

        public CommunicationState State 
        {
            get 
            {
                if (_server is object)
                    return ((ICommunicationObject)_server).State;

                return CommunicationState.Closed;
            }
        }

        public EndpointAddress Connection { get; private set; }

        public void Open(EndpointAddress endpointAddress)
        {
            var address = endpointAddress;
           
            var binding = new NetNamedPipeBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            _callback = new DataServiceClientCallback();
            _callback.OnSystemValueReceived += CallBack_OnSystemValueReceived;
            _callback.OnPositionValueReceived += CallBack_OnPositionValueReceived;
            _callback.OnPortfolioValueReceived += CallBack_OnPortfolioValueReceived;

            _server = DuplexChannelFactory<IDataServiceServer>.CreateChannel(_callback, binding, address);
            
            try
            {
                _server.Register();

                //Subscribe to any topics in case this is a reconnection
                foreach(var ps in _positionSubscriptions)
                    _server.SubscribeToPositionValue(ps.Id, ps.Column);

                foreach (var ps in _portfolioSubscriptions)
                    _server.SubscribeToPortfolioValue(ps.Id, ps.Column);

                foreach (var ps in _systemSubscriptions)
                    _server.SubscribeToSystemValue(ps);

                Connection = address;

                OnConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs());
            }
            catch
            {
                OnConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs());

                throw;
            }
        }

        private void CallBack_OnPortfolioValueReceived(object sender, PortfolioValueReceivedEventArgs e)
        {
            OnPortfolioValueReceived?.Invoke(sender, e);
        }

        private void CallBack_OnPositionValueReceived(object sender, PositionValueReceivedEventArgs e)
        {
            OnPositionValueReceived?.Invoke(sender, e);
        }

        private void CallBack_OnSystemValueReceived(object sender, SystemValueReceivedEventArgs e)
        {
            OnSystemValueReceived?.Invoke(sender, e);
        }

        public List<int> GetPositions(int portfolioId, Positions positions)
        {
            return _server.GetPositions(portfolioId, positions);
        }

        public void Close()
        {
            if (_callback is object)
            {
                try
                {
                    _callback.OnSystemValueReceived -= CallBack_OnSystemValueReceived;
                    _callback.OnPositionValueReceived -= CallBack_OnPositionValueReceived;
                    _callback.OnPortfolioValueReceived -= CallBack_OnPortfolioValueReceived;
                    _callback = null;
                }
                catch
                {
                    //Sink
                }
            }

            if (_server is object)
            {
                try
                {
                    var clientChannel = (IClientChannel)_server;

                    if (State == CommunicationState.Opened)
                    {
                        _server.Unregister();

                        //Subscribe to any topics in case this is a reconnection
                        foreach (var ps in _positionSubscriptions)
                            _server.UnsubscribeToPositionValue(ps.Id, ps.Column);

                        foreach (var ps in _portfolioSubscriptions)
                            _server.UnsubscribeToPortfolioValue(ps.Id, ps.Column);

                        foreach (var ps in _systemSubscriptions)
                            _server.UnsubscribeToSystemValue(ps);
                    }

                    try
                    {
                        if (State != CommunicationState.Faulted)
                        {
                            clientChannel.Close();
                        }
                    }
                    finally
                    {
                        if (State != CommunicationState.Closed)
                        {
                            clientChannel.Abort();
                        }
                    }

                    clientChannel.Dispose();
                    _server = null;
                }
                catch
                {
                    //Sink
                }
            }
        }

        public void SubscribeToPositionValue(int positionId, string column)
        {
            if(!_positionSubscriptions.Contains((positionId, column)))
                _positionSubscriptions.Add((positionId, column));

            if(_server is object && State == CommunicationState.Opened)
                _server.SubscribeToPositionValue(positionId, column);
        }

        public void SubscribeToPortfolioValue(int folioId, string column)
        {
            if (!_portfolioSubscriptions.Contains((folioId, column)))
                _portfolioSubscriptions.Add((folioId, column));

            if (_server is object && State == CommunicationState.Opened)
                _server.SubscribeToPortfolioValue(folioId, column);
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            if (!_systemSubscriptions.Contains(property))
                _systemSubscriptions.Add(property);
            
            if (_server is object && State == CommunicationState.Opened)
                _server.SubscribeToSystemValue(property);
        }

        public void UnsubscribeToPositionValue(int positionId, string column)
        {
            if (!_positionSubscriptions.Contains((positionId, column)))
                _positionSubscriptions.Remove((positionId, column));

            if (_server is object && State == CommunicationState.Opened)
                _server.UnsubscribeToPositionValue(positionId, column);
        }

        public void UnsubscribeToPortfolioValue(int folioId, string column)
        {
            if (!_portfolioSubscriptions.Contains((folioId, column)))
                _portfolioSubscriptions.Remove((folioId, column));

            if (_server is object && State == CommunicationState.Opened)
                _server.UnsubscribeToPortfolioValue(folioId, column);
        }

        public void UnsubscribeToSystemValue(SystemProperty property)
        {
            if (!_systemSubscriptions.Contains(property))
                _systemSubscriptions.Remove(property);

            if (_server is object && State == CommunicationState.Opened)
                _server.UnsubscribeToSystemValue(property);
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _positionSubscriptions.Clear();
                    _portfolioSubscriptions.Clear();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}