//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Interface;
using RxdSolutions.FusionLink.Properties;

namespace RxdSolutions.FusionLink
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DataServer : IDataServiceServer
    {
        private readonly Dictionary<string, IDataServiceClient> _clients;
        private readonly IDataServerProvider _dataServiceProvider;

        private readonly Subscriptions<(int Id, string Column)> _positionSubscriptions;
        private readonly Subscriptions<(int Id, string Column)> _portfolioSubscriptions;
        private readonly Subscriptions<(int Id, PortfolioProperty Property)> _portfolioPropertySubscriptions;
        private readonly Subscriptions<SystemProperty> _systemSubscriptions;

        private readonly AutoResetEvent _clientMonitorResetEvent;
        private readonly int _clientCheckInterval;
        private Thread _clientMonitorThread;
        private bool _isClosed = false;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _dataPublisher;
        private BlockingCollection<DataAvailableEventArgs> _publishQueue;

        public event EventHandler<ClientConnectionChangedEventArgs> OnClientConnectionChanged;
        public event EventHandler<EventArgs> OnStatusChanged;
        public event EventHandler<EventArgs> OnSubscriptionChanged;
        public event EventHandler<DataAvailableEventArgs> OnDataReceived;
        public event EventHandler<EventArgs> OnPublishQueueChanged;

        public int ClientCount
        {
            get
            {
                return _clients.Count;
            }
        }

        public string DefaultMessage { get; set; } = Resources.DefaultGettingDataMessage;

        public bool IsRunning { get; private set; }

        public DataServer(IDataServerProvider dataService)
        {
            _clients = new Dictionary<string, IDataServiceClient>();
            _dataServiceProvider = dataService;
            _dataServiceProvider.DataAvailable += DataService_DataAvailable;

            _positionSubscriptions = new Subscriptions<(int, string)>() { DefaultMessage = DefaultMessage };
            _positionSubscriptions.OnValueChanged += PositionDataPointChanged;
            _positionSubscriptions.SubscriptionAdded += PositionSubscriptionAdded;
            _positionSubscriptions.SubscriptionRemoved += PositionSubscriptionRemoved;

            _portfolioSubscriptions = new Subscriptions<(int, string)>() { DefaultMessage = DefaultMessage };
            _portfolioSubscriptions.OnValueChanged += PortfolioDataPointChanged;
            _portfolioSubscriptions.SubscriptionAdded += PortfolioSubscriptionAdded;
            _portfolioSubscriptions.SubscriptionRemoved += PortfolioSubscriptionRemoved;

            _systemSubscriptions = new Subscriptions<SystemProperty>() { DefaultMessage = DefaultMessage };
            _systemSubscriptions.OnValueChanged += SystemDataPointChanged;
            _systemSubscriptions.SubscriptionAdded += SystemSubscriptionAdded;
            _systemSubscriptions.SubscriptionRemoved += SystemSubscriptionRemoved;

            _portfolioPropertySubscriptions = new Subscriptions<(int, PortfolioProperty)>() { DefaultMessage = DefaultMessage };
            _portfolioPropertySubscriptions.OnValueChanged += PortfolioPropertyPointChanged;
            _portfolioPropertySubscriptions.SubscriptionAdded += PortfolioPropertySubscriptionAdded;
            _portfolioPropertySubscriptions.SubscriptionRemoved += PortfolioPropertySubscriptionRemoved;

            _clientMonitorResetEvent = new AutoResetEvent(false);
            _clientCheckInterval = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;
            _clientMonitorThread = new Thread(new ThreadStart(CheckClientsAlive));
            _clientMonitorThread.Name = "FusionLinkClientConnectionMonitor";
            _clientMonitorThread.Start();

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            if (IsRunning)
                return;

            lock (this)
            {
                if (IsRunning)
                    return;

                IsRunning = true;

                _publishQueue = new BlockingCollection<DataAvailableEventArgs>();

                StartPublisher();

                _dataServiceProvider.Start();

                OnStatusChanged?.Invoke(this, new EventArgs());
            }

            SendServiceStatus();
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            lock (this)
            {
                if (!IsRunning)
                    return;

                IsRunning = false;

                _publishQueue?.CompleteAdding();

                _dataPublisher.Wait();

                _dataPublisher = null;

                _dataServiceProvider.Stop();

                OnStatusChanged?.Invoke(this, new EventArgs());
            }

            SendServiceStatus();
        }

        public void Close()
        {
            Stop();

            _isClosed = true;
            _clientMonitorResetEvent.Set();
            _clientMonitorThread.Join();
        }

        public void Register()
        {
            try
            {
                var c = OperationContext.Current.GetCallbackChannel<IDataServiceClient>();

                lock (_clients)
                {
                    if (!_clients.ContainsKey(OperationContext.Current.SessionId))
                        _clients.Add(OperationContext.Current.SessionId, c);
                }

                OnClientConnectionChanged?.Invoke(this, new ClientConnectionChangedEventArgs(ClientConnectionStatus.Connected, c));

                SendServiceStatus();
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void Unregister()
        {
            try
            {
                lock (_clients)
                {
                    Unregister(OperationContext.Current.SessionId);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public ServiceStatus GetServiceStatus()
        {
            try
            {
                return this.IsRunning ? ServiceStatus.Started : ServiceStatus.Stopped;
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void SubscribeToPositionValue(int positionId, string column)
        {
            try
            {
                var dp = _positionSubscriptions.Add(OperationContext.Current.SessionId, (positionId, column));

                OnSubscriptionChanged?.Invoke(this, new EventArgs());

                SendMessageToAllClients((s, c) =>
                {

                    if (_positionSubscriptions.IsSubscribed(OperationContext.Current.SessionId, (dp.Key.Id, dp.Key.Column)))
                    {
                        c.SendPositionValue(dp.Key.Id, dp.Key.Column, dp.Value);
                    }

                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void SubscribeToPositionValues(List<(int positionId, string column)> items)
        {
            foreach (var (positionId, column) in items)
            {
                SubscribeToPositionValue(positionId, column);
            }
        }

        public void SubscribeToPortfolioValue(int portfolioId, string column)
        {
            try
            {
                var dp = _portfolioSubscriptions.Add(OperationContext.Current.SessionId, (portfolioId, column));

                OnSubscriptionChanged?.Invoke(this, new EventArgs());

                SendMessageToAllClients((s, c) =>
                {

                    if (_portfolioSubscriptions.IsSubscribed(OperationContext.Current.SessionId, (dp.Key.Id, dp.Key.Column)))
                    {
                        c.SendPortfolioValue(dp.Key.Id, dp.Key.Column, dp.Value);
                    }

                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void SubscribeToPortfolioValues(List<(int portfolioId, string column)> items)
        {
            foreach (var (positionId, column) in items)
            {
                SubscribeToPortfolioValue(positionId, column);
            }
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            try
            {
                var dp = _systemSubscriptions.Add(OperationContext.Current.SessionId, property);

                OnSubscriptionChanged?.Invoke(this, new EventArgs());

                SendMessageToAllClients((s, c) =>
                {

                    if (_systemSubscriptions.IsSubscribed(OperationContext.Current.SessionId, dp.Key))
                    {
                        c.SendSystemValue(dp.Key, dp.Value);
                    }

                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void SubscribeToPortfolioProperty(int portfolioId, PortfolioProperty property)
        {
            try
            {
                var dp = _portfolioPropertySubscriptions.Add(OperationContext.Current.SessionId, (portfolioId, property));

                OnSubscriptionChanged?.Invoke(this, new EventArgs());

                SendMessageToAllClients((s, c) =>
                {

                    if (_portfolioPropertySubscriptions.IsSubscribed(OperationContext.Current.SessionId, (dp.Key.Id, dp.Key.Property)))
                    {
                        c.SendPortfolioProperty(dp.Key.Id, dp.Key.Property, dp.Value);
                    }

                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void SubscribeToPortfolioProperties(List<(int portfolioId, PortfolioProperty property)> items)
        {
            foreach (var (positionId, property) in items)
            {
                SubscribeToPortfolioProperty(positionId, property);
            }
        }

        public void UnsubscribeFromPositionValue(int positionId, string column)
        {
            try
            {
                _positionSubscriptions.Remove(OperationContext.Current.SessionId, (positionId, column));

                OnSubscriptionChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void UnsubscribeFromPositionValues(List<(int positionId, string column)> items)
        {
            foreach(var (positionId, column) in items)
            {
                UnsubscribeFromPositionValue(positionId, column);
            }
        }

        public void UnsubscribeFromPortfolioValue(int portfolioId, string column)
        {
            try
            {
                _portfolioSubscriptions.Remove(OperationContext.Current.SessionId, (portfolioId, column));

                OnSubscriptionChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void UnsubscribeFromPortfolioValues(List<(int portfolioId, string column)> items)
        {
            foreach (var (portfolioId, column) in items)
            {
                UnsubscribeFromPortfolioValue(portfolioId, column);
            }
        }

        public void UnsubscribeFromSystemValue(SystemProperty property)
        {
            try
            {
                _systemSubscriptions.Remove(OperationContext.Current.SessionId, property);

                OnSubscriptionChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void UnsubscribeFromPortfolioProperty(int portfolioId, PortfolioProperty property)
        {
            try
            {
                _portfolioPropertySubscriptions.Remove(OperationContext.Current.SessionId, (portfolioId, property));

                OnSubscriptionChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void UnsubscribeFromPortfolioProperties(List<(int portfolioId, PortfolioProperty property)> items)
        {
            foreach (var (portfolioId, property) in items)
            {
                UnsubscribeFromPortfolioProperty(portfolioId, property);
            }
        }

        public List<int> GetPositions(int folioId, PositionsToRequest position)
        {
            try
            {
                return _dataServiceProvider.GetPositions(folioId, position);
            }
            catch (PortfolioNotFoundException)
            {
                throw new FaultException<PortfolioNotFoundFaultContract>(new PortfolioNotFoundFaultContract() { PortfolioId = folioId });
            }
            catch (PortfolioNotLoadedException)
            {
                throw new FaultException<PortfolioNotLoadedFaultContract>(new PortfolioNotLoadedFaultContract() { PortfolioId = folioId });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _dataServiceProvider.GetPriceHistory(instrumentId, startDate, endDate);
            }
            catch (InstrumentNotFoundException)
            {
                throw new FaultException<InstrumentNotFoundFaultContract>(new InstrumentNotFoundFaultContract() { Instrument = instrumentId.ToString() });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public List<PriceHistory> GetPriceHistory(string reference, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _dataServiceProvider.GetPriceHistory(reference, startDate, endDate);
            }
            catch (InstrumentNotFoundException)
            {
                throw new FaultException<InstrumentNotFoundFaultContract>(new InstrumentNotFoundFaultContract() { Instrument = reference });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public List<CurvePoint> GetCurvePoints(string currency, string family, string reference)
        {
            try
            {
                return _dataServiceProvider.GetCurvePoints(currency, family, reference);
            }
            catch (CurrencyNotFoundException)
            {
                throw new FaultException<CurrencyNotFoundFaultContract>(new CurrencyNotFoundFaultContract() { Currency = currency });
            }
            catch (CurveFamilyFoundException)
            {
                throw new FaultException<CurveFamilyNotFoundFaultContract>(new CurveFamilyNotFoundFaultContract() { CurveFamily = family });
            }
            catch (CurveNotFoundException)
            {
                throw new FaultException<CurveNotFoundFaultContract>(new CurveNotFoundFaultContract() { Curve = reference });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void RequestCalculate()
        {
            try
            {
                _dataServiceProvider.RequestCalculate();
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void LoadPositions()
        {
            try
            {
                _dataServiceProvider.LoadPositions();
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public int PositonValueSubscriptionCount
        {
            get => _positionSubscriptions.Count;
        }

        public int PortfolioValueSubscriptionCount
        {
            get => _portfolioSubscriptions.Count;
        }

        public int PortfolioPropertySubscriptionCount
        {
            get => _portfolioPropertySubscriptions.Count;
        }

        public int SystemValueCount
        {
            get => _systemSubscriptions.Count;
        }

        public int PublishQueueCount
        {
            get => _publishQueue.Count;
        }

        private void StartPublisher()
        {
            _dataPublisher = Task.Run(() =>
            {
                try
                {
                    foreach (DataAvailableEventArgs e in _publishQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
                    {
                        if (!IsRunning)
                            return;

                        MergeData(e);

                        OnPublishQueueChanged?.Invoke(this, new EventArgs());
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        Stop();
                    }
                    catch (Exception)
                    {
                        IsRunning = false;
                    }
                }

            }, _cancellationTokenSource.Token);
        }

        private void MergeData(DataAvailableEventArgs e)
        {
            foreach (var kvp in e.PortfolioValues)
            {
                var dp = _portfolioSubscriptions.Get(kvp.Key);
                if (dp is object)
                    dp.Value = kvp.Value;
            }

            foreach (var kvp in e.PortfolioProperties)
            {
                var dp = _portfolioPropertySubscriptions.Get(kvp.Key);
                if (dp is object)
                    dp.Value = kvp.Value;
            }

            foreach (var kvp in e.PositionValues)
            {
                var dp = _positionSubscriptions.Get(kvp.Key);
                if (dp is object)
                    dp.Value = kvp.Value;
            }

            foreach (var kvp in e.SystemValues)
            {
                var dp = _systemSubscriptions.Get(kvp.Key);
                if (dp is object)
                    dp.Value = kvp.Value;
            }
        }

        private void CheckClientsAlive()
        {
            while (!_isClosed)
            {
                SendMessageToAllClients((sessionId, client) => client.Heartbeat());

                _clientMonitorResetEvent.WaitOne(_clientCheckInterval);
            }
        }

        private void SendMessageToAllClients(Action<string, IDataServiceClient> send)
        {
            lock (_clients)
            {
                if (_clients.Count == 0)
                    return;

                // send number to clients
                foreach (var key in _clients.Keys.ToList())
                {
                    var c = _clients[key];

                    try
                    {
                        send.Invoke(key, c);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.ToString());

                        Unregister(key);
                    }
                }
            }
        }

        private void SystemDataPointChanged(object sender, DataPointChangedEventArgs<SystemProperty> e)
        {
            SendMessageToAllClients((s, c) =>
            {

                if (_systemSubscriptions.IsSubscribed(s, e.DataPoint.Key))
                    c.SendSystemValue(e.DataPoint.Key, e.DataPoint.Value);

            });
        }

        private void PortfolioDataPointChanged(object sender, DataPointChangedEventArgs<(int Id, string Column)> e)
        {
            SendMessageToAllClients((s, c) =>
            {

                if (_portfolioSubscriptions.IsSubscribed(s, e.DataPoint.Key))
                    c.SendPortfolioValue(e.DataPoint.Key.Id, e.DataPoint.Key.Column, e.DataPoint.Value);

            });
        }

        private void PositionDataPointChanged(object sender, DataPointChangedEventArgs<(int Id, string Column)> e)
        {
            SendMessageToAllClients((s, c) =>
            {

                if (_positionSubscriptions.IsSubscribed(s, e.DataPoint.Key))
                    c.SendPositionValue(e.DataPoint.Key.Id, e.DataPoint.Key.Column, e.DataPoint.Value);

            });
        }

        private void PortfolioPropertyPointChanged(object sender, DataPointChangedEventArgs<(int Id, PortfolioProperty Property)> e)
        {
            SendMessageToAllClients((s, c) =>
            {

                if (_portfolioPropertySubscriptions.IsSubscribed(s, e.DataPoint.Key))
                    c.SendPortfolioProperty(e.DataPoint.Key.Id, e.DataPoint.Key.Property, e.DataPoint.Value);

            });
        }

        /// <remarks>
        /// This method must be called inside a _clients lock
        /// </remarks>
        private void Unregister(string sessionId)
        {
            if (_clients.ContainsKey(sessionId))
                _clients.Remove(sessionId);

            foreach (var (Id, Column) in _portfolioSubscriptions.GetKeys())
                _portfolioSubscriptions.Remove(sessionId, (Id, Column));

            foreach (var (Id, Column) in _positionSubscriptions.GetKeys())
                _positionSubscriptions.Remove(sessionId, (Id, Column));

            foreach (var sub in _systemSubscriptions.GetKeys())
                _systemSubscriptions.Remove(sessionId, sub);

            foreach ((int Id, PortfolioProperty Property) sub in _portfolioPropertySubscriptions.GetKeys())
                _portfolioPropertySubscriptions.Remove(sessionId, sub);

            OnClientConnectionChanged?.Invoke(this, new ClientConnectionChangedEventArgs(ClientConnectionStatus.Disconnected, null));
        }

        private void SendServiceStatus()
        {
            SendMessageToAllClients((id, client) =>
            {

                client.SendServiceStaus(GetServiceStatus());

            });
        }

        private void DataService_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            OnDataReceived?.Invoke(this, e);

            if (!_publishQueue.IsAddingCompleted)
            {
                _publishQueue.Add(e);
                OnPublishQueueChanged?.Invoke(this, new EventArgs());
            }
        }

        private void SystemSubscriptionAdded(object sender, SubscriptionChangedEventArgs<SystemProperty> e)
        {
            _dataServiceProvider.SubscribeToSystemValue(e.Key);
        }

        private void SystemSubscriptionRemoved(object sender, SubscriptionChangedEventArgs<SystemProperty> e)
        {
            _dataServiceProvider.UnsubscribeFromSystemValue(e.Key);
        }

        private void PortfolioPropertySubscriptionAdded(object sender, SubscriptionChangedEventArgs<(int Id, PortfolioProperty Property)> e)
        {
            _dataServiceProvider.SubscribeToPortfolioProperty(e.Key.Id, e.Key.Property);
        }

        private void PortfolioPropertySubscriptionRemoved(object sender, SubscriptionChangedEventArgs<(int Id, PortfolioProperty Property)> e)
        {
            _dataServiceProvider.UnsubscribeFromPortfolioProperty(e.Key.Id, e.Key.Property);
        }

        private void PortfolioSubscriptionRemoved(object sender, SubscriptionChangedEventArgs<(int Id, string Column)> e)
        {
            _dataServiceProvider.UnsubscribeFromPortfolio(e.Key.Id, e.Key.Column);
        }

        private void PortfolioSubscriptionAdded(object sender, SubscriptionChangedEventArgs<(int Id, string Column)> e)
        {
            _dataServiceProvider.SubscribeToPortfolio(e.Key.Id, e.Key.Column);
        }

        private void PositionSubscriptionRemoved(object sender, SubscriptionChangedEventArgs<(int Id, string Column)> e)
        {
            _dataServiceProvider.UnsubscribeFromPosition(e.Key.Id, e.Key.Column);
        }

        private void PositionSubscriptionAdded(object sender, SubscriptionChangedEventArgs<(int Id, string Column)> e)
        {
            _dataServiceProvider.SubscribeToPosition(e.Key.Id, e.Key.Column);
        }
    }
}