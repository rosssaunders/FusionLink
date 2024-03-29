﻿//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Client.Properties;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.Client
{
    public class DataServiceClient : IDisposable
    {
        private IRealTimeServer _realTimeServer;
        private DataServiceCallbackClient _callback;
        private EndpointAddress _connection;
        private Uri _via;

        private readonly object _connectionLock;

        private readonly HashSet<(int Id, string Column)> _positionCellValueSubscriptions;
        private readonly HashSet<(int PortfolioId, int InstrumentId, string Column)> _flatPositionCellValueSubscriptions;
        private readonly HashSet<(int Id, string Column)> _portfolioCellValueSubscriptions;
        private readonly HashSet<(int Id, PortfolioProperty Property)> _portfolioPropertySubscriptions;
        private readonly HashSet<(object Id, string Property)> _instrumentPropertySubscriptions;
        private readonly HashSet<(object Id, string Property)> _currencyPropertySubscriptions;

        private readonly HashSet<SystemProperty> _systemSubscriptions;

        private readonly Binding _binding;

        public event EventHandler<ConnectionStatusChangedEventArgs> OnConnectionStatusChanged;
        public event EventHandler<PositionValueReceivedEventArgs> OnPositionValueReceived;
        public event EventHandler<FlatPositionValueReceivedEventArgs> OnFlatPositionValueReceived;
        public event EventHandler<PortfolioValueReceivedEventArgs> OnPortfolioValueReceived;
        public event EventHandler<SystemValueReceivedEventArgs> OnSystemValueReceived;
        public event EventHandler<ServiceStatusReceivedEventArgs> OnServiceStatusReceived;
        public event EventHandler<PortfolioPropertyReceivedEventArgs> OnPortfolioPropertyReceived;
        public event EventHandler<InstrumentPropertyReceivedEventArgs> OnInstrumentPropertyReceived;
        public event EventHandler<CurrencyPropertyReceivedEventArgs> OnCurrencyPropertyReceived;

        public DataServiceClient()
        {
            _connectionLock = new object();
            _positionCellValueSubscriptions = new HashSet<(int, string)>();
            _flatPositionCellValueSubscriptions = new HashSet<(int, int, string)>();
            _portfolioCellValueSubscriptions = new HashSet<(int, string)>();
            _portfolioPropertySubscriptions = new HashSet<(int Id, PortfolioProperty Property)>();
            _instrumentPropertySubscriptions = new HashSet<(object Id, string Property)>();
            _systemSubscriptions = new HashSet<SystemProperty>();
            _currencyPropertySubscriptions = new HashSet<(object Id, string Property)>();
            _binding = CreateTcpBinding();
        }

        public CommunicationState State 
        {
            get 
            {
                lock (_connectionLock)
                {
                    if (_realTimeServer is object)
                        return ((ICommunicationObject)_realTimeServer).State;

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

        public Uri Via
        {
            get
            {
                lock (_connectionLock)
                {
                    return _via;
                }
            }
            private set
            {
                _via = value;
            }
        }

        public bool IsConnecting { get; private set; }

        public void Open(EndpointAddress endpointAddress, Uri via)
        {
            lock(_connectionLock)
            {
                IsConnecting = true;

                try
                {
                    var address = new EndpointAddress(endpointAddress.Uri, CreateIdentity());

                    var binding = _binding;

                    _callback = new DataServiceCallbackClient();
                    _callback.OnSystemValueReceived += CallBack_OnSystemValueReceived;
                    _callback.OnPositionValueReceived += CallBack_OnPositionValueReceived;
                    _callback.OnFlatPositionValueReceived += CallBack_OnFlatPositionValueReceived;
                    _callback.OnPortfolioValueReceived += CallBack_OnPortfolioValueReceived;
                    _callback.OnServiceStatusReceived += Callback_OnServiceStatusReceived;
                    _callback.OnPortfolioPropertyReceived += Callback_OnPortfolioPropertyReceived;
                    _callback.OnInstrumentPropertyReceived += Callback_OnInstrumentPropertyReceived;
                    _callback.OnCurrencyPropertyReceived += Callback_OnCurrencyPropertyReceived;

                    _realTimeServer = DuplexChannelFactory<IRealTimeServer>.CreateChannel(_callback, binding, address, via);

                    _realTimeServer.Register();

                    //Subscribe to any topics in case this is a reconnection
                    _realTimeServer.SubscribeToPositionValues(_positionCellValueSubscriptions.ToList());
                    _realTimeServer.SubscribeToFlatPositionValues(_flatPositionCellValueSubscriptions.ToList());
                    _realTimeServer.SubscribeToPortfolioValues(_portfolioCellValueSubscriptions.ToList());
                    _realTimeServer.SubscribeToPortfolioProperties(_portfolioPropertySubscriptions.ToList());
                    _realTimeServer.SubscribeToInstrumentProperties(_instrumentPropertySubscriptions.ToList());
                    _realTimeServer.SubscribeToCurrencyProperties(_currencyPropertySubscriptions.ToList());

                    foreach (var ps in _systemSubscriptions)
                        _realTimeServer.SubscribeToSystemValue(ps);

                    Connection = address;
                    Via = via;

                    OnConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs());
                }
                finally
                {
                    IsConnecting = false;
                }
            }
        }

        public void Test(EndpointAddress endpointAddress, Uri via)
        {
            lock (_connectionLock)
            {
                var address = new EndpointAddress(endpointAddress.Uri, CreateIdentity());
                var binding = _binding;
                var callback = new DataServiceCallbackClient();
                var realTimeServer = DuplexChannelFactory<IRealTimeServer>.CreateChannel(callback, _binding, address, via);
                ((IChannel)realTimeServer).Open();
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
                            _callback.OnFlatPositionValueReceived -= CallBack_OnFlatPositionValueReceived;
                            _callback.OnPortfolioValueReceived -= CallBack_OnPortfolioValueReceived;
                            _callback.OnPortfolioPropertyReceived -= Callback_OnPortfolioPropertyReceived;
                            _callback.OnInstrumentPropertyReceived -= Callback_OnInstrumentPropertyReceived;
                            _callback = null;
                        }
                        catch
                        {
                            //Sink
                        }
                    }

                    if (_realTimeServer is object)
                    {
                        try
                        {
                            var clientChannel = (IClientChannel)_realTimeServer;

                            if (State == CommunicationState.Opened)
                            {
                                //Subscribe to any topics in case this is a reconnection
                                _realTimeServer.UnsubscribeFromPositionValues(_positionCellValueSubscriptions.ToList());
                                _realTimeServer.UnsubscribeFromFlatPositionValues(_flatPositionCellValueSubscriptions.ToList());
                                _realTimeServer.UnsubscribeFromPortfolioValues(_portfolioCellValueSubscriptions.ToList());
                                _realTimeServer.UnsubscribeFromPortfolioProperties(_portfolioPropertySubscriptions.ToList());
                                _realTimeServer.UnsubscribeFromInstrumentProperties(_instrumentPropertySubscriptions.ToList());

                                foreach (var ps in _systemSubscriptions)
                                    _realTimeServer.UnsubscribeFromSystemValue(ps);

                                _realTimeServer.Unregister();
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
                            _realTimeServer = null;
                        }
                        catch
                        {
                            //Sink
                        }
                    }

                    _realTimeServer = null;
                    Connection = null;
                    Via = null;

                    OnConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs());
                }
                finally
                {
                    IsConnecting = false;
                }
            }
        }

        private static NetTcpBinding CreateTcpBinding()
        {
            var binding = new NetTcpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                SendTimeout = new TimeSpan(0, 5, 0),
                ReceiveTimeout = new TimeSpan(0, 5, 0)
            };
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            return binding;
        }

        public void LoadPositions()
        {
            if(GetServiceStatus() == ServiceStatus.Started)
            {
                _realTimeServer.LoadPositions();
            }
            else
            {
                throw new ApplicationException("Not connected");
            }
        }

        public void RequestCalculate()
        {
            _realTimeServer.RequestCalculate();
        }

        public ServiceStatus GetServiceStatus()
        {
            if (_realTimeServer is null)
                return ServiceStatus.NotConnected;

            try
            {
                return _realTimeServer.GetServiceStatus();
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

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.SubscribeToPositionValue(positionId, column));
            }
        }

        public void SubscribeToFlatPositionValue(int portfolioId, int instrumentId, string column)
        {
            lock (_positionCellValueSubscriptions)
            {
                if (!_flatPositionCellValueSubscriptions.Contains((portfolioId, instrumentId, column)))
                    _flatPositionCellValueSubscriptions.Add((portfolioId, instrumentId, column));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.SubscribeToFlatPositionValue(portfolioId, instrumentId, column));
            }
        }

        public void SubscribeToPortfolioValue(int folioId, string column)
        {
            lock(_portfolioCellValueSubscriptions)
            {
                if (!_portfolioCellValueSubscriptions.Contains((folioId, column)))
                    _portfolioCellValueSubscriptions.Add((folioId, column));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.SubscribeToPortfolioValue(folioId, column));
            }
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            lock(_systemSubscriptions)
            {
                if (!_systemSubscriptions.Contains(property))
                    _systemSubscriptions.Add(property);

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.SubscribeToSystemValue(property));
            }
        }

        public void SubscribeToPortfolioProperty(int folioId, PortfolioProperty property)
        {
            lock(_portfolioPropertySubscriptions)
            {
                if (!_portfolioPropertySubscriptions.Contains((folioId, property)))
                    _portfolioPropertySubscriptions.Add((folioId, property));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.SubscribeToPortfolioProperty(folioId, property));
            }
        }

        public void UnsubscribeToPortfolioProperty(int folioId, PortfolioProperty property)
        {
            lock(_portfolioPropertySubscriptions)
            {
                if (_portfolioPropertySubscriptions.Contains((folioId, property)))
                    _portfolioPropertySubscriptions.Remove((folioId, property));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.UnsubscribeFromPortfolioProperty(folioId, property));
            }
        }

        public void SubscribeToInstrumentProperty(object instrument, string property)
        {
            lock (_instrumentPropertySubscriptions)
            {
                if (!_instrumentPropertySubscriptions.Contains((instrument, property)))
                    _instrumentPropertySubscriptions.Add((instrument, property));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.SubscribeToInstrumentProperty(instrument, property));
            }
        }

        public void SubscribeToCurrencyProperty(object currency, string property)
        {
            lock (_currencyPropertySubscriptions)
            {
                if (!_currencyPropertySubscriptions.Contains((currency, property)))
                    _currencyPropertySubscriptions.Add((currency, property));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.SubscribeToCurrencyProperty(currency, property));
            }
        }

        public void UnsubscribeToCurrencyProperty(object currency, string property)
        {
            lock (_currencyPropertySubscriptions)
            {
                if (_currencyPropertySubscriptions.Contains((currency, property)))
                    _currencyPropertySubscriptions.Remove((currency, property));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.UnsubscribeFromCurrencyProperty(currency, property));
            }
        }

        public void UnsubscribeToInstrumentProperty(object instrument, string property)
        {
            lock (_instrumentPropertySubscriptions)
            {
                if (_instrumentPropertySubscriptions.Contains((instrument, property)))
                    _instrumentPropertySubscriptions.Remove((instrument, property));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.UnsubscribeFromInstrumentProperty(instrument, property));
            }
        }

        public void UnsubscribeToPositionValue(int positionId, string column)
        {
            lock(_positionCellValueSubscriptions)
            {
                if (_positionCellValueSubscriptions.Contains((positionId, column)))
                    _positionCellValueSubscriptions.Remove((positionId, column));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.UnsubscribeFromPositionValue(positionId, column));
            }
        }

        public void UnsubscribeToFlatPositionValue(int portfolioId, int instrumentId, string column)
        {
            lock (_flatPositionCellValueSubscriptions)
            {
                if (_flatPositionCellValueSubscriptions.Contains((portfolioId, instrumentId, column)))
                    _flatPositionCellValueSubscriptions.Remove((portfolioId, instrumentId, column));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.UnsubscribeFromFlatPositionValue(portfolioId, instrumentId, column));
            }
        }

        public void UnsubscribeToPortfolioValue(int folioId, string column)
        {
            lock(_portfolioCellValueSubscriptions)
            {
                if (_portfolioCellValueSubscriptions.Contains((folioId, column)))
                    _portfolioCellValueSubscriptions.Remove((folioId, column));

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.UnsubscribeFromPortfolioValue(folioId, column));
            }
        }

        public void UnsubscribeToSystemValue(SystemProperty property)
        {
            lock(_systemSubscriptions)
            {
                if (_systemSubscriptions.Contains(property))
                    _systemSubscriptions.Remove(property);

                InvokeServerWithErrorHandlingAsync(() => _realTimeServer.UnsubscribeFromSystemValue(property));
            }
        }

        private T ExecuteBatchRequest<T>(Func<IOnDemandServer, T> func)
        {
            var onDemandServer = ChannelFactory<IOnDemandServer>.CreateChannel(_binding, Connection, Via);
            
            var results = func(onDemandServer);

            try
            {
                ((IClientChannel)onDemandServer)?.Close();
            }
            finally
            {
                ((IDisposable)onDemandServer)?.Dispose();
            }
            
            return results;
        }

        public List<int> GetPositions(int portfolioId, PositionsToRequest positions)
        {
            try
            {
                return ExecuteBatchRequest(x => x.GetPositions(portfolioId, positions));
            }
            catch (FaultException<PortfolioNotFoundFaultContract> ex)
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

        public List<int> GetFlatPositions(int portfolioId, PositionsToRequest positions)
        {
            try
            {
                return ExecuteBatchRequest(x => x.GetFlatPositions(portfolioId, positions));
            }
            catch (FaultException<PortfolioNotFoundFaultContract> ex)
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

        public DateTime AddBusinessDays(DateTime date, int numberOfDays, string currency, CalendarType calendarType, string name)
        {
            try
            {
                return ExecuteBatchRequest(x => x.AddBusinessDays(date, numberOfDays, currency, calendarType, name));
            }
            catch (FaultException<CalendarNotFoundFaultContract> ex)
            {
                throw new CalendarNotFoundException($"{Resources.CalendarNotFoundMessage} - {ex.Detail.Calendar}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public List<PriceHistory> GetInstrumentPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return ExecuteBatchRequest(x => x.GetInstrumentPriceHistory(instrumentId, startDate, endDate));
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

        public List<PriceHistory> GetInstrumentPriceHistory(string reference, DateTime startDate, DateTime endDate)
        {
            try
            {
                return ExecuteBatchRequest(x => x.GetInstrumentPriceHistory(reference, startDate, endDate));
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

        public List<PriceHistory> GetCurrencyPriceHistory(int currencyId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return ExecuteBatchRequest(x => x.GetCurrencyPriceHistory(currencyId, startDate, endDate));
            }
            catch (FaultException<CurrencyNotFoundFaultContract> ex)
            {
                throw new CurrencyNotFoundException($"{Resources.CurrencyNotFoundMessage} - {ex.Detail.Currency}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public List<PriceHistory> GetCurrencyPriceHistory(string reference, DateTime startDate, DateTime endDate)
        {
            try
            {
                return ExecuteBatchRequest(x => x.GetCurrencyPriceHistory(reference, startDate, endDate));
            }
            catch (FaultException<CurrencyNotFoundFaultContract> ex)
            {
                throw new CurrencyNotFoundException($"{Resources.CurrencyNotFoundMessage} - {ex.Detail.Currency}");
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
                return ExecuteBatchRequest(x => x.GetCurvePoints(currency, family, reference));
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

        public List<Transaction> GetPositionTransactions(int positionId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var results = ExecuteBatchRequest(x => x.GetPositionTransactions(positionId, startDate, endDate));

                results = results.OrderBy(x => x.TransactionCode).ToList();

                return results;
            }
            catch (FaultException<PositionNotFoundFaultContract> ex)
            {
                throw new CurveNotFoundException($"{Resources.PositionNotFoundMessage} - {ex.Detail.PositionId}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public List<Transaction> GetPortfolioTransactions(int portfolioId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var results = ExecuteBatchRequest(x => x.GetPortfolioTransactions(portfolioId, startDate, endDate));

                results = results.OrderBy(x => x.TransactionCode).ToList();

                return results;
            }
            catch (FaultException<PortfolioNotFoundFaultContract> ex)
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

        public DataTable GetInstrumentSet(int instrumentId, string property)
        {
            try
            {
                var results = ExecuteBatchRequest(x => x.GetInstrumentSet(instrumentId, property));

                return results;
            }
            catch (FaultException<InstrumentNotFoundFaultContract> ex)
            {
                throw new InstrumentNotFoundException($"{Resources.PortfolioNotFoundMessage} - {ex.Detail.Instrument}");
            }
            catch (FaultException<InvalidFieldFaultContract>)
            {
                throw new InvalidFieldException($"{Resources.InvalidFieldMessage}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
            catch (FaultException ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public DataTable GetInstrumentSet(string reference, string property)
        {
            try
            {
                var results = ExecuteBatchRequest(x => x.GetInstrumentSet(reference, property));

                return results;
            }
            catch (FaultException<InvalidFieldFaultContract>)
            {
                throw new InvalidFieldException($"{Resources.InvalidFieldMessage}");
            }
            catch (FaultException<InstrumentNotFoundFaultContract> ex)
            {
                throw new InstrumentNotFoundException($"{Resources.InstrumentNotFoundMessage} - {ex.Detail.Instrument}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
            catch (FaultException ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public DataTable GetCurrencySet(int currencyId, string property)
        {
            try
            {
                var results = ExecuteBatchRequest(x => x.GetCurrencySet(currencyId, property));

                return results;
            }
            catch (FaultException<InvalidFieldFaultContract>)
            {
                throw new InvalidFieldException($"{Resources.InvalidFieldMessage}");
            }
            catch (FaultException<InstrumentNotFoundFaultContract> ex)
            {
                throw new InstrumentNotFoundException($"{Resources.InstrumentNotFoundMessage} - {ex.Detail.Instrument}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
            catch (FaultException ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public DataTable GetCurrencySet(string currency, string property)
        {
            try
            {
                var results = ExecuteBatchRequest(x => x.GetCurrencySet(currency, property));

                return results;
            }
            catch (FaultException<InvalidFieldFaultContract>)
            {
                throw new InvalidFieldException($"{Resources.InvalidFieldMessage}");
            }
            catch (FaultException<CurrencyNotFoundFaultContract> ex)
            {
                throw new CurrencyNotFoundException($"{Resources.CurrencyNotFoundMessage} - {ex.Detail.Currency}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
            catch (FaultException ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        public DataTable GetReportSqlSourceResults(string reportName, string sourceName)
        {
            try
            {
                var results = ExecuteBatchRequest(x => x.GetReportSqlSourceResults(reportName, sourceName));

                return results;
            }
            catch (FaultException<ReportNotFoundFaultContract> ex)
            {
                throw new ReportNotFoundException($"{Resources.ReportNotFoundMessage} - {reportName} - {sourceName}");
            }
            catch (FaultException<ErrorFaultContract> ex)
            {
                throw new DataServiceException(ex.Message);
            }
            catch (FaultException ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }

        //private void InvokeServerWithErrorHandling(Action action)
        //{
        //    if (_realTimeServer is object && State == CommunicationState.Opened)
        //    {
        //        try
        //        {
        //            action();
        //        }
        //        catch (FaultException<ErrorFaultContract> ex)
        //        {
        //            throw new DataServiceException(ex.Detail.Message);
        //        }
        //    }
        //}

        private void InvokeServerWithErrorHandlingAsync(Action action)
        {
            if (_realTimeServer is object && State == CommunicationState.Opened)
            {
                try
                {
                    Task.Run(action);
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

        private void CallBack_OnFlatPositionValueReceived(object sender, FlatPositionValueReceivedEventArgs e)
        {
            OnFlatPositionValueReceived?.Invoke(sender, e);
        }

        private void CallBack_OnSystemValueReceived(object sender, SystemValueReceivedEventArgs e)
        {
            OnSystemValueReceived?.Invoke(sender, e);
        }

        private void Callback_OnPortfolioPropertyReceived(object sender, PortfolioPropertyReceivedEventArgs e)
        {
            OnPortfolioPropertyReceived?.Invoke(sender, e);
        }

        private void Callback_OnInstrumentPropertyReceived(object sender, InstrumentPropertyReceivedEventArgs e)
        {
            OnInstrumentPropertyReceived?.Invoke(sender, e);
        }

        private void Callback_OnCurrencyPropertyReceived(object sender, CurrencyPropertyReceivedEventArgs e)
        {
            OnCurrencyPropertyReceived?.Invoke(sender, e);
        }

        static EndpointIdentity CreateIdentity()
        {
            var self = WindowsIdentity.GetCurrent();

            // Need an UPN string here
            string domain = Environment.UserDomainName;
            if (domain != null)
            {
                string[] split = self.Name.Split('\\');
                if (split.Length == 2)
                {
                    return EndpointIdentity.CreateUpnIdentity(split[1] + "@" + domain);
                }
            }

            throw new ApplicationException("Unable to generate a UPN");
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

                    try 
                    {
                        Close();
                    }
                    catch
                    {
                        //Sink
                    }
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