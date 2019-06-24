//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using RxdSolutions.FusionLink.ExcelClient.Properties;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class DataServiceClient : IDisposable
    {
        private IDataServiceServer _server;
        private DataServiceClientCallback _callback;
        private EndpointAddress _connection;

        private readonly object _connectionLock;

        private readonly HashSet<(int Id, string Column)> _positionCellValueSubscriptions;
        private readonly HashSet<(int Id, string Column)> _portfolioCellValueSubscriptions;
        private readonly HashSet<(int Id, PortfolioProperty Property)> _portfolioPropertySubscriptions;

        private readonly HashSet<SystemProperty> _systemSubscriptions;

        public event EventHandler<ConnectionStatusChangedEventArgs> OnConnectionStatusChanged;
        public event EventHandler<PositionValueReceivedEventArgs> OnPositionValueReceived;
        public event EventHandler<PortfolioValueReceivedEventArgs> OnPortfolioValueReceived;
        public event EventHandler<SystemValueReceivedEventArgs> OnSystemValueReceived;
        public event EventHandler<ServiceStatusReceivedEventArgs> OnServiceStatusReceived;
        public event EventHandler<PortfolioPropertyReceivedEventArgs> OnPortfolioPropertyReceived;

        public DataServiceClient()
        {
            _connectionLock = new object();
            _positionCellValueSubscriptions = new HashSet<(int, string)>();
            _portfolioCellValueSubscriptions = new HashSet<(int, string)>();
            _portfolioPropertySubscriptions = new HashSet<(int Id, PortfolioProperty Property)>();
            _systemSubscriptions = new HashSet<SystemProperty>();
        }

        public CommunicationState State 
        {
            get 
            {
                lock (_connectionLock)
                {
                    if (_server is object)
                        return ((ICommunicationObject)_server).State;

                    return CommunicationState.Closed;
                }
            }
        }

        public DateTime LastMessageReceivedTime
        {
            get
            {
                if(_callback == null)
                {
                    return DateTime.MinValue;
                }

                return _callback.LastReceivedMessageTimestamp;
            }
        }

        public EndpointAddress Connection
        {
            get
            {
                lock(_connectionLock)
                {
                    return _connection;
                }
            }
            private set
            {
                _connection = value;
            }
        }

        public bool IsConnecting { get; private set; }

        public void Open(EndpointAddress endpointAddress)
        {
            lock(_connectionLock)
            {
                IsConnecting = true;

                try
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
                    _callback.OnServiceStatusReceived += Callback_OnServiceStatusReceived;
                    _callback.OnPortfolioPropertyReceived += Callback_OnPortfolioPropertyReceived;

                    _server = DuplexChannelFactory<IDataServiceServer>.CreateChannel(_callback, binding, address);

                    _server.Register();

                    //Subscribe to any topics in case this is a reconnection
                    foreach (var (Id, Column) in _positionCellValueSubscriptions)
                        _server.SubscribeToPositionValue(Id, Column);

                    foreach (var (Id, Column) in _portfolioCellValueSubscriptions)
                        _server.SubscribeToPortfolioValue(Id, Column);

                    foreach (var (Id, Property) in _portfolioPropertySubscriptions)
                        _server.SubscribeToPortfolioProperty(Id, Property);

                    foreach (var ps in _systemSubscriptions)
                        _server.SubscribeToSystemValue(ps);

                    Connection = address;

                    OnConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs());
                }
                finally
                {
                    IsConnecting = false;
                }
            }
        }

        public void Close()
        {
            lock(_connectionLock)
            {
                IsConnecting = true;

                try
                {
                    if (_callback is object)
                    {
                        try
                        {
                            _callback.OnSystemValueReceived -= CallBack_OnSystemValueReceived;
                            _callback.OnPositionValueReceived -= CallBack_OnPositionValueReceived;
                            _callback.OnPortfolioValueReceived -= CallBack_OnPortfolioValueReceived;
                            _callback.OnPortfolioPropertyReceived -= Callback_OnPortfolioPropertyReceived;
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
                                foreach (var (Id, Column) in _positionCellValueSubscriptions)
                                    _server.UnsubscribeFromPositionValue(Id, Column);

                                foreach (var (Id, Column) in _portfolioCellValueSubscriptions)
                                    _server.UnsubscribeFromPortfolioValue(Id, Column);

                                foreach (var ps in _systemSubscriptions)
                                    _server.UnsubscribeFromSystemValue(ps);

                                foreach (var (Id, Property) in _portfolioPropertySubscriptions)
                                    _server.UnsubscribeFromPortfolioProperty(Id, Property);
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

                    _server = null;
                    Connection = null;

                    OnConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs());
                }
                finally
                {
                    IsConnecting = false;
                }
            }
        }

        internal void LoadPositions()
        {
            _server.LoadPositions();
        }

        internal void RequestCalculate()
        {
            _server.RequestCalculate();
        }

        public ServiceStatus GetServiceStatus()
        {
            if (_server is null)
                return ServiceStatus.NotConnected;

            try
            {
                return _server.GetServiceStatus();
            }
            catch (Exception)
            {
                return ServiceStatus.NotConnected;
            }
        }

        public void SubscribeToPositionValue(int positionId, string column)
        {
            lock(_positionCellValueSubscriptions)
            {
                if (!_positionCellValueSubscriptions.Contains((positionId, column)))
                    _positionCellValueSubscriptions.Add((positionId, column));

                InvokeServerWithErrorHandling(() => _server.SubscribeToPositionValue(positionId, column));
            }
        }

        public void SubscribeToPortfolioValue(int folioId, string column)
        {
            lock(_portfolioCellValueSubscriptions)
            {
                if (!_portfolioCellValueSubscriptions.Contains((folioId, column)))
                    _portfolioCellValueSubscriptions.Add((folioId, column));

                InvokeServerWithErrorHandling(() => _server.SubscribeToPortfolioValue(folioId, column));
            }
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            lock(_systemSubscriptions)
            {
                if (!_systemSubscriptions.Contains(property))
                    _systemSubscriptions.Add(property);

                InvokeServerWithErrorHandling(() => _server.SubscribeToSystemValue(property));
            }
        }

        public void SubscribeToPortfolioProperty(int folioId, PortfolioProperty property)
        {
            lock(_portfolioPropertySubscriptions)
            {
                if (!_portfolioPropertySubscriptions.Contains((folioId, property)))
                    _portfolioPropertySubscriptions.Add((folioId, property));

                InvokeServerWithErrorHandling(() => _server.SubscribeToPortfolioProperty(folioId, property));
            }
        }

        public void UnsubscribeToPortfolioProperty(int folioId, PortfolioProperty property)
        {
            lock(_portfolioPropertySubscriptions)
            {
                if (!_portfolioPropertySubscriptions.Contains((folioId, property)))
                    _portfolioPropertySubscriptions.Remove((folioId, property));

                InvokeServerWithErrorHandling(() => _server.UnsubscribeFromPortfolioProperty(folioId, property));
            }
        }

        public void UnsubscribeToPositionValue(int positionId, string column)
        {
            lock(_positionCellValueSubscriptions)
            {
                if (!_positionCellValueSubscriptions.Contains((positionId, column)))
                    _positionCellValueSubscriptions.Remove((positionId, column));

                InvokeServerWithErrorHandling(() => _server.UnsubscribeFromPositionValue(positionId, column));
            }
        }

        public void UnsubscribeToPortfolioValue(int folioId, string column)
        {
            lock(_portfolioCellValueSubscriptions)
            {
                if (!_portfolioCellValueSubscriptions.Contains((folioId, column)))
                    _portfolioCellValueSubscriptions.Remove((folioId, column));

                InvokeServerWithErrorHandling(() => _server.UnsubscribeFromPortfolioValue(folioId, column));
            }
        }

        public void UnsubscribeToSystemValue(SystemProperty property)
        {
            lock(_systemSubscriptions)
            {
                if (!_systemSubscriptions.Contains(property))
                    _systemSubscriptions.Remove(property);

                InvokeServerWithErrorHandling(() => _server.UnsubscribeFromSystemValue(property));
            }
        }


        public List<int> GetPositions(int portfolioId, PositionsToRequest positions)
        {
            try
            {
                return _server.GetPositions(portfolioId, positions);
            }
            catch(FaultException<PortfolioNotFoundFaultContract> ex)
            {
                throw new PortfolioNotFoundException($"{Resources.PortfolioNotFoundMessage} - {ex.Detail.PortfolioId}");
            }
            catch (FaultException<PortfolioNotLoadedFaultContract> ex)
            {
                throw new PortfolioNotLoadedException($"{Resources.PortfolioNotLoadedMessage} - {ex.Detail.PortfolioId}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _server.GetPriceHistory(instrumentId, startDate, endDate);
            }
            catch (FaultException<InstrumentNotFoundFaultContract> ex)
            {
                throw new InstrumentNotFoundException($"{Resources.InstrumentNotFoundMessage} - {ex.Detail.Instrument}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public List<PriceHistory> GetPriceHistory(string reference, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _server.GetPriceHistory(reference, startDate, endDate);
            }
            catch (FaultException<InstrumentNotFoundFaultContract> ex)
            {
                throw new InstrumentNotFoundException($"{Resources.InstrumentNotFoundMessage} - {ex.Detail.Instrument}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public List<CurvePoint> GetCurvePoints(string currency, string family, string reference)
        {
            try
            {
                return _server.GetCurvePoints(currency, family, reference);
            }
            catch (FaultException<CurveNotFoundFaultContract> ex)
            {
                throw new CurveNotFoundException($"{Resources.CurveNotFoundMessage} - {ex.Detail.Curve}");
            }
            catch (FaultException<CurveFamilyNotFoundFaultContract> ex)
            {
                throw new CurveNotFoundException($"{Resources.CurveNotFoundMessage} - {ex.Detail.CurveFamily}");
            }
            catch (FaultException<CurrencyNotFoundFaultContract> ex)
            {
                throw new CurveNotFoundException($"{Resources.CurveNotFoundMessage} - {ex.Detail.Currency}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }


        private void InvokeServerWithErrorHandling(Action action)
        {
            if (_server is object && State == CommunicationState.Opened)
            {
                try
                {
                    action();
                }
                catch (FaultException<ErrorFaultContract> ex)
                {
                    throw new DataServiceException(ex.Detail.Message);
                }
            }
        }

        private void Callback_OnServiceStatusReceived(object sender, ServiceStatusReceivedEventArgs e)
        {
            OnServiceStatusReceived?.Invoke(sender, e);
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

        private void Callback_OnPortfolioPropertyReceived(object sender, PortfolioPropertyReceivedEventArgs e)
        {
            OnPortfolioPropertyReceived?.Invoke(sender, e);
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _positionCellValueSubscriptions.Clear();
                    _portfolioCellValueSubscriptions.Clear();
                    _systemSubscriptions.Clear();
                    _portfolioPropertySubscriptions.Clear();
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