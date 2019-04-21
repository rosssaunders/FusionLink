//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class ConnectionMonitor
    {
        private readonly DataServiceClient _client;

        private bool _running = false;
        private readonly ManualResetEvent _resetEvent;
        private Task _monitor;

        private readonly object _monitorLock = new object();

        public ConnectionMonitor(DataServiceClient client)
        {
            _client = client;
            _resetEvent = new ManualResetEvent(false);
        }

        private HashSet<CommunicationState> _closedStates = new HashSet<CommunicationState>() { CommunicationState.Closed, CommunicationState.Faulted };

        private Uri _connection;

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
                        if (_closedStates.Contains(_client.State))
                        {
                            try
                            {
                                if (_client.AvailableEndpoints.Count > 0)
                                {
                                    if(_connection == null)
                                    {
                                        _connection = _client.AvailableEndpoints.FirstOrDefault()?.Uri;
                                    }

                                    var connectionToAttempt = _client.FindEndpoint(_connection);

                                    if(connectionToAttempt is null)
                                    {
                                        //The set connection doesn't exist anylonger. Clear it. 
                                        _connection = null;
                                    }

                                    if (connectionToAttempt is object)
                                        _client.Open(connectionToAttempt);
                                }
                            }
                            catch
                            {

                            }
                        }

                        _resetEvent.WaitOne(1000);
                    }
                });
            }
        }

        public void Stop()
        {
            _running = false;
            _resetEvent.Set();
            _monitor?.Wait();

            _client.Close();
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
    }
}