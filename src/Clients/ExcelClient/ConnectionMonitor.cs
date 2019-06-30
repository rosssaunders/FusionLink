//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ConnectionMonitor : IDisposable
    {
        private readonly List<DataServiceClient> _clients;

        private readonly HashSet<CommunicationState> _closedStates = new HashSet<CommunicationState>() { CommunicationState.Closed, CommunicationState.Faulted };

        private Uri _connection;

        private bool _running = false;
        private readonly AutoResetEvent _resetEvent;
        private Task _monitor;

        private readonly object _monitorLock = new object();
        private readonly AvailableConnections _connections;

        public bool IsConnected { get; private set; }

        public ConnectionMonitor(AvailableConnections connections)
        {
            _clients = new List<DataServiceClient>();

            _resetEvent = new AutoResetEvent(false);

            _connections = connections;
        }

        public void RegisterClient(DataServiceClient client)
        {
            _clients.Add(client);
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

                                if(_connections.AvailableEndpoints.Count == 0)
                                {
                                    continue;
                                }

                                if (_connection == null)
                                {
                                    _connection = _connections.AvailableEndpoints.FirstOrDefault()?.Uri;
                                }

                                var connectionToAttempt = _connections.FindEndpoint(_connection);

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
                                        _connections.Remove(connectionToAttempt);
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var client in _clients)
                    {
                        client.Dispose();
                    }

                    _clients.Clear();

                    _resetEvent.Set();
                    _resetEvent.Dispose();
                    _monitor.Wait();
                    _monitor.Dispose();
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