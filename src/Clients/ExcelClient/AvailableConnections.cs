//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class AvailableConnections : IDisposable
    {
        public event EventHandler<EventArgs> AvailableEndpointsChanged;

        public bool IsSearchingForEndPoints { get; private set; }

        private readonly List<EndPointAddressVia> _availableEndpoints;
        
        private readonly ServiceHost _announcementServiceHost;
        private readonly AnnouncementService _announcementService;

        public AvailableConnections()
        {
            _availableEndpoints = new List<EndPointAddressVia>();

            // Subscribe the announcement events
            _announcementService = new AnnouncementService();
            _announcementService.OnlineAnnouncementReceived += OnOnlineEvent;
            _announcementService.OfflineAnnouncementReceived += OnOfflineEvent;

            // Create ServiceHost for the AnnouncementService
            _announcementServiceHost = new ServiceHost(_announcementService);
            _announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());
            _announcementServiceHost.Open();
        }

        public IReadOnlyList<EndPointAddressVia> AvailableEndpoints
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
                            _availableEndpoints.Add(new EndPointAddressVia(endPoint.Address, endPoint.ListenUris[0], ConnectionType.Automatic));
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

        public void Remove(EndPointAddressVia ea)
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
                    var idx = _availableEndpoints.FindIndex(new Predicate<EndPointAddressVia>(x => x.Via == e.EndpointDiscoveryMetadata.ListenUris[0]));

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
                        _availableEndpoints.Add(new EndPointAddressVia(new EndpointAddress(connection), connection, ConnectionType.Manual));
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
                        _availableEndpoints.Add(new EndPointAddressVia(e.EndpointDiscoveryMetadata.Address, e.EndpointDiscoveryMetadata.ListenUris[0], ConnectionType.Automatic));
                        AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                    }
                }
            }
            catch(Exception ex)
            {
                //Sink
            }
        }

        public EndPointAddressVia FindEndpoint(Uri connection)
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
