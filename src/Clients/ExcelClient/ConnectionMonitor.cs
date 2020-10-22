//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RxdSolutions.FusionLink.Client;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ConnectionMonitor : IDisposable
    {
        private DataServiceClient _client;
        private Uri _connection;

        private readonly object _connectionLock = new object();
        private readonly ServerConnectionMonitor _connections;

        public bool IsConnected { get; private set; }

        public event EventHandler OnConnectionFailed;
        public event EventHandler OnConnectionSuccess;

        public ConnectionMonitor(ServerConnectionMonitor connections)
        {
            _connections = connections;
        }

        public void RegisterClient(DataServiceClient client)
        {
            _client = client;
        }

        public void Open()
        {
            lock(_connectionLock)
            {
                var client = _client;

                if (IsConnected)
                    throw new ApplicationException("Connection currently open. Please call Close first");

                try
                {
                    if (_connection == null)
                        throw new ApplicationException("No connection specified.");

                    var connectionToAttempt = _connections.FindEndpoint(_connection);

                    if (connectionToAttempt is null)
                    {
                        IsConnected = false;

                        //The connection doesn't exist.
                        _connection = null;

                        throw new ApplicationException("The requested connection does not exist.");
                    }

                    if (connectionToAttempt is object)
                    {
                        try
                        {
                            client.Open(connectionToAttempt.EndpointAddress, connectionToAttempt.Via);

                            IsConnected = true;

                            OnConnectionSuccess?.Invoke(this, new EventArgs());
                        }
                        catch (TimeoutException)
                        {
                            //Ignore and try again on the next pass
                            IsConnected = false;

                            OnConnectionFailed?.Invoke(this, new EventArgs());

                            throw;
                        }
                        catch (CommunicationException)
                        {
                            IsConnected = false;

                            //Looks like the server is dead. Remove from the available list.
                            _connections.Remove(connectionToAttempt);

                            OnConnectionFailed?.Invoke(this, new EventArgs());

                            throw;
                        }
                        catch (Exception)
                        {
                            IsConnected = false;

                            OnConnectionFailed?.Invoke(this, new EventArgs());

                            throw;
                        }
                    }
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());

                    IsConnected = false;

                    throw;
                }
            }
        }

        public void Close()
        {
            lock(_connectionLock)
            {
                _client.Close();
                IsConnected = false;
            }
        }

        public void SetConnection(Uri connection)
        {
            Close();

            _connection = connection;
            
            Open();
        }

        public void SetManualConnection(Uri connection)
        {
            _connections.AddManualConnection(connection);

            SetConnection(connection);
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
                    _client.Dispose();
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