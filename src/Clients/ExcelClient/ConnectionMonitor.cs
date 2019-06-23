//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Threading;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ConnectionMonitor : IDisposable
    {
        private readonly List<DataServiceClient> _clients;

        private readonly HashSet<CommunicationState> _closedStates = new HashSet<CommunicationState>() { CommunicationState.Closed, CommunicationState.Faulted };

        private Uri _connection;

        private readonly AnnouncementService _announcementService;
        private readonly List<EndpointAddress> _availableEndpoints;
        private readonly ServiceHost _announcementServiceHost;

        private bool _running = false;
        private readonly AutoResetEvent _resetEvent;
        private Task _monitor;

        private readonly object _monitorLock = new object();

        public event EventHandler<EventArgs> AvailableEndpointsChanged;

        public bool IsSearchingForEndPoints { get; private set; }

        public IReadOnlyList<EndpointAddress> AvailableEndpoints
        {
            get
            {
                lock(_availableEndpoints)
                {
                    return _availableEndpoints.ToList();
                }
            }
        }

        public bool IsConnected { get; private set; }

        public ConnectionMonitor()
        {
            _clients = new List<DataServiceClient>();

            _resetEvent = new AutoResetEvent(false);

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

        public void RegisterClient(DataServiceClient client)
        {
            _clients.Add(client);
        }

        public Task FindAvailableServicesAsync()
        {
            IsSearchingForEndPoints = true;

            return Task.Run(() => {

                FindAvailableServices();

            });
        }
        
        public void FindAvailableServices()
        {
            try
            {
                IsSearchingForEndPoints = true;

                AvailableEndpointsChanged?.Invoke(this, new EventArgs());

                var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                var findResponse = discoveryClient.Find(new FindCriteria(typeof(IDataServiceServer)) { Duration = TimeSpan.FromSeconds(3) });

                foreach (var endPoints in findResponse.Endpoints)
                {
                    var found = false;

                    foreach (var knownEndpoint in AvailableEndpoints)
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
                            _availableEndpoints.Add(endPoints.Address);
                            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                        }
                    }
                }

                foreach (var knownEndpoint in AvailableEndpoints)
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
                            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                        }
                    }
                }
            }
            finally
            {
                IsSearchingForEndPoints = false;

                AvailableEndpointsChanged?.Invoke(this, new EventArgs());
            }
        }

        public void Start()
        {
            lock(_monitorLock)
            {
                if (_running)
                    return;

                _running = true;

                _monitor = Task.Run(() => {

                    while (_running)
                    {
                        foreach(var client in _clients)
                        {
                            if (!_closedStates.Contains(client.State))
                            {
                                continue;
                            }

                            try
                            {
                                //Check if we think we are connected by the connection state is broken
                                if(IsConnected)
                                {
                                    IsConnected = false;

                                    client.Close();
                                }

                                if(AvailableEndpoints.Count == 0)
                                {
                                    continue;
                                }

                                if (_connection == null)
                                {
                                    _connection = AvailableEndpoints.FirstOrDefault()?.Uri;
                                }

                                var connectionToAttempt = FindEndpoint(_connection);

                                if (connectionToAttempt is null)
                                {
                                    IsConnected = false;

                                    //The connection doesn't exist.
                                    _connection = null;
                                }

                                if (connectionToAttempt is object)
                                {
                                    try
                                    {
                                        client.Open(connectionToAttempt);

                                        IsConnected = true;
                                    }
                                    catch (TimeoutException)
                                    {
                                        //Ignore and try again on the next pass
                                        IsConnected = false;
                                    }
                                    catch (CommunicationException ex)
                                    {
                                        IsConnected = false;

                                        //Looks like the server is dead. Remove from the available list.
                                        lock (_availableEndpoints)
                                        {
                                            _availableEndpoints.Remove(connectionToAttempt);
                                        }

                                        AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                                    }
                                    catch (Exception)
                                    {
                                        IsConnected = false;
                                    }
                                }
                            }
                            catch
                            {
                                IsConnected = false;
                            }
                        }

                        _resetEvent.WaitOne(1000);
                    }
                });
            }
        }

        public void Stop()
        {
            lock(_monitorLock)
            {
                if(_running)
                {
                    _running = false;
                    _resetEvent.Set();
                    _monitor?.Wait();

                    foreach (var client in _clients)
                    {
                        client.Close();
                    }
                }
            }
        }

        public void SetConnection(Uri connection)
        {
            Stop();

            _connection = connection;

            Start();
        }

        public Uri GetConnection()
        {
            return _connection;
        }

        private void OnOfflineEvent(object sender, AnnouncementEventArgs e)
        {
            _availableEndpoints.Remove(e.EndpointDiscoveryMetadata.Address);
            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
        }

        private void OnOnlineEvent(object sender, AnnouncementEventArgs e)
        {
            _availableEndpoints.Add(e.EndpointDiscoveryMetadata.Address);
            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
        }

        private EndpointAddress FindEndpoint(Uri connection)
        {
            lock (_availableEndpoints)
            {
                return _availableEndpoints.SingleOrDefault(x => x.Uri == connection);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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