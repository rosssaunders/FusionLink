//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class DataServiceClient : IDisposable
    {
        private IDataServiceServer _server;
        private DataServiceClientCallback _callback;

        private readonly HashSet<(int Id, string Column)> _positionSubscriptions;
        private readonly HashSet<(int Id, string Column)> _portfolioSubscriptions;
        private readonly List<EndpointAddress> _availableEndpoints;
        private readonly AnnouncementService _announcementService;
        private readonly ServiceHost _announcementServiceHost;
        
        public event EventHandler<ConnectionStatusChangedEventArgs> OnConnectionStatusChanged;
        public event EventHandler<PositionValueReceivedEventArgs> OnPositionValueReceived;
        public event EventHandler<PortfolioValueReceivedEventArgs> OnPortfolioValueReceived;
        public event EventHandler<PortfolioDateReceivedEventArgs> OnPortfolioDateReceived;

        public DataServiceClient()
        {
            _positionSubscriptions = new HashSet<(int, string)>();
            _portfolioSubscriptions = new HashSet<(int, string)>();

            _availableEndpoints = new List<EndpointAddress>();

            // Subscribe the announcement events
            _announcementService = new AnnouncementService();
            _announcementService.OnlineAnnouncementReceived += OnOnlineEvent;
            _announcementService.OfflineAnnouncementReceived += OnOfflineEvent;

            // Create ServiceHost for the AnnouncementService
            _announcementServiceHost = new ServiceHost(_announcementService);
            _announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());
            _announcementServiceHost.Open();
        }

        private void OnOfflineEvent(object sender, AnnouncementEventArgs e)
        {
            _availableEndpoints.Remove(e.EndpointDiscoveryMetadata.Address);
        }

        private void OnOnlineEvent(object sender, AnnouncementEventArgs e)
        {
            _availableEndpoints.Add(e.EndpointDiscoveryMetadata.Address);
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

        public IReadOnlyList<EndpointAddress> AvailableEndpoints => _availableEndpoints;

        public EndpointAddress Connection { get; private set; }

        public EndpointAddress FindEndpoint(Uri connection)
        {
            lock(_availableEndpoints)
            {
                return _availableEndpoints.SingleOrDefault(x => x.Uri == connection);
            }
        }

        public void FindAvailableServices()
        {
            var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            var findResponse = discoveryClient.Find(new FindCriteria(typeof(IDataServiceServer)));

            foreach(var endPoints in findResponse.Endpoints)
            {
                var found = false;

                foreach (var knownEndpoint in _availableEndpoints)
                {
                    if(knownEndpoint.Uri == endPoints.Address.Uri)
                    {
                        found = true;
                        break;
                    }   
                }

                if (!found)
                {
                    lock (_availableEndpoints)
                    {
                        _availableEndpoints.Add(endPoints.Address);
                    }
                }
            }

            foreach(var knownEndpoint in _availableEndpoints.ToList())
            {
                var found = false;
                foreach (var endPoints in findResponse.Endpoints)
                {
                    if (knownEndpoint.Uri == endPoints.Address.Uri)
                    {
                        found = true;
                        break;
                    }    
                }

                if (!found)
                {
                    lock (_availableEndpoints)
                    {
                        _availableEndpoints.Remove(knownEndpoint);
                    }
                }
            }
        }

        public void Open(EndpointAddress endpointAddress)
        {
            var address = endpointAddress;
           
            var binding = new NetNamedPipeBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            _callback = new DataServiceClientCallback();
            _callback.OnPortfolioDateReceived += CallBack_OnPortfolioDateReceived;
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

                Connection = address;
            }
            catch (TimeoutException)
            {
                //Do Nothing. Wait for the next update.
            }
            catch (Exception)
            {
                //The endpoint must be dead. Remove it.
                _availableEndpoints.Remove(endpointAddress);

                throw;
            }
            finally
            {
                OnConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs());
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

        private void CallBack_OnPortfolioDateReceived(object sender, PortfolioDateReceivedEventArgs e)
        {
            OnPortfolioDateReceived?.Invoke(sender, e);
        }

        public List<int> GetPositions(int portfolioId)
        {
            return _server.GetPositions(portfolioId);
        }

        public void Close()
        {
            if (_callback is object)
            {
                try
                {
                    _callback.OnPortfolioDateReceived -= CallBack_OnPortfolioDateReceived;
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
                        _server.UnRegister();
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

            _server.SubscribeToPositionValue(positionId, column);
        }

        public void SubscribeToPortfolioValue(int folioId, string column)
        {
            if (!_portfolioSubscriptions.Contains((folioId, column)))
                _portfolioSubscriptions.Add((folioId, column));

            _server.SubscribeToPortfolioValue(folioId, column);
        }

        public void SubscribeToPortfolioDate()
        {
            _server.SubscribeToPortfolioDate();
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

                    _announcementServiceHost.Close();
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