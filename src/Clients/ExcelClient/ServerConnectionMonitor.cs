//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Threading;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ServerConnectionMonitor : IDisposable
    {
        public event EventHandler<EventArgs> AvailableEndpointsChanged;

        public bool IsSearchingForEndPoints { get; private set; }

        private readonly List<EndpointAddressVia> _availableEndpoints;
        
        private readonly ServiceHost _announcementServiceHost;
        private readonly AnnouncementService _announcementService;

        private readonly Thread _aliveConnectionMonitor;
        private readonly AutoResetEvent _resetEvent;

        public ServerConnectionMonitor()
        {
            _availableEndpoints = new List<EndpointAddressVia>();

            // Subscribe the announcement events
            _announcementService = new AnnouncementService();
            _announcementService.OnlineAnnouncementReceived += OnOnlineEvent;
            _announcementService.OfflineAnnouncementReceived += OnOfflineEvent;

            // Create ServiceHost for the AnnouncementService
            _announcementServiceHost = new ServiceHost(_announcementService);
            _announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());
            _announcementServiceHost.Open();

            _resetEvent = new AutoResetEvent(false);
            _aliveConnectionMonitor = new Thread(MonitorConnectionsForDeadEndpoints)
            {
                IsBackground = true,
                Name = "AliveConnectionMonitor",
                Priority = ThreadPriority.Lowest
            };

            _aliveConnectionMonitor.Start();
        }

        private void MonitorConnectionsForDeadEndpoints(object state)
        {
            var waitTime = TimeSpan.FromSeconds(5);

            while (!disposedValue)
            {
                lock (_availableEndpoints)
                {
                    var toRemove = new List<EndpointAddressVia>();

                    foreach (var connection in _availableEndpoints)
                    {
                        try
                        {
                            using var client = new DataServiceClient();
                            client.Test(connection.EndpointAddress, connection.Via);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.ToString());

                            //Remove from the list of available connections
                            toRemove.Add(connection);
                        }
                    }

                    if (toRemove.Count > 0)
                    {
                        foreach(var ep in toRemove)
                            _availableEndpoints.Remove(ep);
                    }
                }

                _resetEvent.WaitOne(waitTime);
            }
        }

        public IReadOnlyList<EndpointAddressVia> AvailableEndpoints
        {
            get
            {
                lock (_availableEndpoints)
                {
                    return _availableEndpoints.ToList();
                }
            }
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

                var findResponse = discoveryClient.Find(new FindCriteria(typeof(IRealTimeServer)) { Duration = TimeSpan.FromSeconds(3) });

                discoveryClient.Close();

                lock (_availableEndpoints)
                {
                    foreach (var endPoint in findResponse.Endpoints)
                    {
                        var remoteUsername = new ConnectionBuilder(endPoint.Address.Uri).GetConnectionUsername();

                        if (!string.Equals(remoteUsername, Environment.UserName, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        var found = false;

                        foreach (var knownEndpoint in AvailableEndpoints)
                        {
                            if (knownEndpoint.Via == endPoint.ListenUris[0])
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            _availableEndpoints.Add(new EndpointAddressVia(endPoint.Address, endPoint.ListenUris[0], ConnectionType.Automatic));
                            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                        }
                    }

                    foreach (var knownEndpoint in AvailableEndpoints)
                    {
                        var found = false;
                        foreach (var endPoints in findResponse.Endpoints)
                        {
                            if (knownEndpoint.Via == endPoints.ListenUris[0])
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
            }
            finally
            {
                IsSearchingForEndPoints = false;

                AvailableEndpointsChanged?.Invoke(this, new EventArgs());
            }
        }

        public void Remove(EndpointAddressVia ea)
        {
            lock (_availableEndpoints)
            {
                _availableEndpoints.Remove(ea);

                AvailableEndpointsChanged?.Invoke(this, new EventArgs());
            }
        }

        private void OnOfflineEvent(object sender, AnnouncementEventArgs e)
        {
            try
            {
                lock (_availableEndpoints)
                {
                    var idx = _availableEndpoints.FindIndex(new Predicate<EndpointAddressVia>(x => x.Via == e.EndpointDiscoveryMetadata.ListenUris[0]));

                    if (idx != -1)
                        _availableEndpoints.RemoveAt(idx);

                    AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                }
            }
            catch(Exception ex)
            {
                //Sink
            }
        }

        internal void AddManualConnection(Uri connection)
        {
            try
            {
                lock (_availableEndpoints)
                {
                    //Check the user name
                    var remoteUsername = new ConnectionBuilder(connection).GetConnectionUsername();
                    if (!string.Equals(remoteUsername, Environment.UserName, StringComparison.InvariantCultureIgnoreCase))
                        return;

                    //Check if we already know about this one
                    var alreadyExists = _availableEndpoints.Any(x => x.Via == connection);
                    if (!alreadyExists)
                    {
                        _availableEndpoints.Add(new EndpointAddressVia(new EndpointAddress(connection), connection, ConnectionType.Manual));
                        AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                //Sink
            }
        }

        private void OnOnlineEvent(object sender, AnnouncementEventArgs e)
        {
            try
            {
                lock (_availableEndpoints)
                {
                    //Check the user name
                    var remoteUsername = new ConnectionBuilder(e.EndpointDiscoveryMetadata.Address.Uri).GetConnectionUsername();
                    if (!string.Equals(remoteUsername, Environment.UserName, StringComparison.InvariantCultureIgnoreCase))
                        return;

                    //Check if we already know about this one
                    var alreadyExists = _availableEndpoints.Any(x => x.EndpointAddress == e.EndpointDiscoveryMetadata.Address);
                    if (!alreadyExists)
                    {
                        _availableEndpoints.Add(new EndpointAddressVia(e.EndpointDiscoveryMetadata.Address, e.EndpointDiscoveryMetadata.ListenUris[0], ConnectionType.Automatic));
                        AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                    }
                }
            }
            catch(Exception ex)
            {
                //Sink
            }
        }

        public EndpointAddressVia FindEndpoint(Uri connection)
        {
            lock (_availableEndpoints)
            {
                return _availableEndpoints.SingleOrDefault(x => x.Via == connection);
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
                    _resetEvent.Set();
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
