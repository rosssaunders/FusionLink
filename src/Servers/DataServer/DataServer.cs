//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Interface;
using RxdSolutions.FusionLink.Properties;

namespace RxdSolutions.FusionLink
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
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
        public event EventHandler<EventArgs> OnSubscriptionChanged;

        public IReadOnlyList<IDataServiceClient> Clients => _clients.Values.ToList();

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
            _clientMonitorThread.Start();

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void SystemSubscriptionAdded(object sender, SubscriptionChangedEventArgs<SystemProperty> e)
        {
            _dataServiceProvider.SubscribeToSystemValue(e.Key);
        }

        private void SystemSubscriptionRemoved(object sender, SubscriptionChangedEventArgs<SystemProperty> e)
        {
            _dataServiceProvider.UnsubscribeToSystemValue(e.Key);
        }

        private void PortfolioPropertySubscriptionAdded(object sender, SubscriptionChangedEventArgs<(int Id, PortfolioProperty Property)> e)
        {
            _dataServiceProvider.SubscribeToPortfolioProperty(e.Key.Id, e.Key.Property);
        }

        private void PortfolioPropertySubscriptionRemoved(object sender, SubscriptionChangedEventArgs<(int Id, PortfolioProperty Property)> e)
        {
            _dataServiceProvider.UnsubscribeToPortfolioProperty(e.Key.Id, e.Key.Property);
        }

        private void PortfolioSubscriptionRemoved(object sender, SubscriptionChangedEventArgs<(int Id, string Column)> e)
        {
            _dataServiceProvider.UnsubscribeToPortfolio(e.Key.Id, e.Key.Column);
        }

        private void PortfolioSubscriptionAdded(object sender, SubscriptionChangedEventArgs<(int Id, string Column)> e)
        {
            _dataServiceProvider.SubscribeToPortfolio(e.Key.Id, e.Key.Column);
        }

        private void PositionSubscriptionRemoved(object sender, SubscriptionChangedEventArgs<(int Id, string Column)> e)
        {
            _dataServiceProvider.UnsubscribeToPosition(e.Key.Id, e.Key.Column);
        }

        private void PositionSubscriptionAdded(object sender, SubscriptionChangedEventArgs<(int Id, string Column)> e)
        {
            _dataServiceProvider.SubscribeToPosition(e.Key.Id, e.Key.Column);
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

                _publishQueue.CompleteAdding();

                _dataPublisher.Wait();

                _dataPublisher = null;

                _dataServiceProvider.Stop();
            }

            SendServiceStatus();
        }

        private void StartPublisher()
        {
            _dataPublisher = Task.Run(() =>
            {
                foreach (DataAvailableEventArgs e in _publishQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
                {
                    if (!IsRunning)
                        return;

                    MergeData(e);
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

        public void Close()
        {
            Stop();

            _isClosed = true;
            _clientMonitorResetEvent.Set();
            _clientMonitorThread.Join();
        }

        public void Register()
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

        public void Unregister()
        {
            Unregister(OperationContext.Current.SessionId);
        }

        public ServiceStatus GetServiceStatus()
        {
            return this.IsRunning ? ServiceStatus.Started : ServiceStatus.Stopped;
        }

        public void SubscribeToPositionValue(int positionId, string column)
        {
            var dp = _positionSubscriptions.Add(OperationContext.Current.SessionId, (positionId, column));

            OnSubscriptionChanged?.Invoke(this, new EventArgs());

            SendMessageToAllClients((s, c) => {

                if (_positionSubscriptions.IsSubscribed(OperationContext.Current.SessionId, (dp.Key.Id, dp.Key.Column)))
                {
                    c.SendPositionValue(dp.Key.Id, dp.Key.Column, dp.Value);
                }

            });
        }

        public void SubscribeToPortfolioValue(int portfolioId, string column)
        {
            var dp = _portfolioSubscriptions.Add(OperationContext.Current.SessionId, (portfolioId, column));
 
            OnSubscriptionChanged?.Invoke(this, new EventArgs());

            SendMessageToAllClients((s, c) => {

                if (_portfolioSubscriptions.IsSubscribed(OperationContext.Current.SessionId, (dp.Key.Id, dp.Key.Column)))
                {
                    c.SendPortfolioValue(dp.Key.Id, dp.Key.Column, dp.Value);
                }

            });
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            var dp = _systemSubscriptions.Add(OperationContext.Current.SessionId, property);

            OnSubscriptionChanged?.Invoke(this, new EventArgs());

            SendMessageToAllClients((s, c) => {

                if (_systemSubscriptions.IsSubscribed(OperationContext.Current.SessionId, dp.Key))
                {
                    c.SendSystemValue(dp.Key, dp.Value);
                }

            });
        }

        public void SubscribeToPortfolioProperty(int portfolioId, PortfolioProperty property)
        {
            var dp = _portfolioPropertySubscriptions.Add(OperationContext.Current.SessionId, (portfolioId, property));

            OnSubscriptionChanged?.Invoke(this, new EventArgs());

            SendMessageToAllClients((s, c) => {

                if (_portfolioPropertySubscriptions.IsSubscribed(OperationContext.Current.SessionId, (dp.Key.Id, dp.Key.Property)))
                {
                    c.SendPortfolioProperty(dp.Key.Id, dp.Key.Property, dp.Value);
                }

            });
        }

        public void UnsubscribeToPositionValue(int positionId, string column)
        {
            _positionSubscriptions.Remove(OperationContext.Current.SessionId, (positionId, column));

            OnSubscriptionChanged?.Invoke(this, new EventArgs());
        }

        public void UnsubscribeToPortfolioValue(int portfolioId, string column)
        {
            _portfolioSubscriptions.Remove(OperationContext.Current.SessionId, (portfolioId, column));

            OnSubscriptionChanged?.Invoke(this, new EventArgs());
        }

        public void UnsubscribeToSystemValue(SystemProperty property)
        {
            _systemSubscriptions.Remove(OperationContext.Current.SessionId, property);

            OnSubscriptionChanged?.Invoke(this, new EventArgs());
        }

        public void UnsubscribeToPortfolioProperty(int portfolioId, PortfolioProperty property)
        {
            _portfolioPropertySubscriptions.Remove(OperationContext.Current.SessionId, (portfolioId, property));

            OnSubscriptionChanged?.Invoke(this, new EventArgs());
        }

        public List<int> GetPositions(int folioId, PositionsToRequest position)
        {
            try
            {
                return _dataServiceProvider.GetPositions(folioId, position);
            }
            catch (PortfolioNotFoundException)
            {
                throw new FaultException<PortfolioNotFoundFaultContract>(new PortfolioNotFoundFaultContract(), new FaultReason("Portfolio Not Found"));
            }
            catch (PortfolioNotLoadedException)
            {
                throw new FaultException<PortfolioNotLoadedFaultContract>(new PortfolioNotLoadedFaultContract(), new FaultReason("Portfolio Not Loaded"));
            }
        }

        public List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _dataServiceProvider.GetPriceHistory(instrumentId, startDate, endDate);
            }
            catch(InstrumentNotFoundException)
            {
                throw new FaultException<InstrumentNotFoundFaultContract>(new InstrumentNotFoundFaultContract(), new FaultReason("Instrument not found"));
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
                throw new FaultException<InstrumentNotFoundFaultContract>(new InstrumentNotFoundFaultContract(), new FaultReason("Instrument not found"));
            }
        }

        public List<CurvePoint> GetCurvePoints(string curency, string family, string reference)
        {
            try
            {
                return _dataServiceProvider.GetCurvePoints(curency, family, reference);
            }
            catch (CurrencyNotFoundException)
            {
                throw new FaultException<CurrencyNotFoundFaultContract>(new CurrencyNotFoundFaultContract(), new FaultReason("Curve not found"));
            }
            catch (CurveFamilyFoundException)
            {
                throw new FaultException<CurveFamilyNotFoundFaultContract>(new CurveFamilyNotFoundFaultContract(), new FaultReason("Curve family found"));
            }
            catch (CurveNotFoundException)
            {
                throw new FaultException<CurveNotFoundFaultContract>(new CurveNotFoundFaultContract(), new FaultReason("Curve not found"));
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
            if (_clients.Count == 0)
                return;

            // send number to clients
            foreach(var key in _clients.Keys.ToList())
            {
                // can't do foreach because we want to remove dead ones
                var c = _clients[key];
                try
                {
                    send.Invoke(key, c);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);

                    Unregister(key);
                }
            }
        }

        private void SystemDataPointChanged(object sender, DataPointChangedEventArgs<SystemProperty> e)
        {
            SendMessageToAllClients((s, c) => {

                if (_systemSubscriptions.IsSubscribed(s, e.DataPoint.Key))
                    c.SendSystemValue(e.DataPoint.Key, e.DataPoint.Value);

            });
        }

        private void PortfolioDataPointChanged(object sender, DataPointChangedEventArgs<(int Id, string Column)> e)
        {
            SendMessageToAllClients((s, c) => {

                if (_portfolioSubscriptions.IsSubscribed(s, e.DataPoint.Key))
                    c.SendPortfolioValue(e.DataPoint.Key.Id, e.DataPoint.Key.Column, e.DataPoint.Value);

            });
        }

        private void PositionDataPointChanged(object sender, DataPointChangedEventArgs<(int Id, string Column)> e)
        {
            SendMessageToAllClients((s, c) => {

                if (_positionSubscriptions.IsSubscribed(s, e.DataPoint.Key))
                    c.SendPositionValue(e.DataPoint.Key.Id, e.DataPoint.Key.Column, e.DataPoint.Value);

            });
        }

        private void PortfolioPropertyPointChanged(object sender, DataPointChangedEventArgs<(int Id, PortfolioProperty Property)> e)
        {
            SendMessageToAllClients((s, c) => {

                if (_portfolioPropertySubscriptions.IsSubscribed(s, e.DataPoint.Key))
                    c.SendPortfolioProperty(e.DataPoint.Key.Id, e.DataPoint.Key.Property, e.DataPoint.Value);

            });
        }

        private void Unregister(string sessionId)
        {
            lock (_clients)
            {
                if (_clients.ContainsKey(sessionId))
                    _clients.Remove(sessionId);
            }

            foreach (var sub in _portfolioSubscriptions.GetKeys())
                _portfolioSubscriptions.Remove(sessionId, (sub.Id, sub.Column));

            foreach (var sub in _positionSubscriptions.GetKeys())
                _positionSubscriptions.Remove(sessionId, (sub.Id, sub.Column));

            foreach (var sub in _systemSubscriptions.GetKeys())
                _systemSubscriptions.Remove(sessionId, sub);

            foreach (var sub in _portfolioPropertySubscriptions.GetKeys())
                _portfolioPropertySubscriptions.Remove(sessionId, sub);

            OnClientConnectionChanged?.Invoke(this, new ClientConnectionChangedEventArgs(ClientConnectionStatus.Disconnected, null));
        }

        private void SendServiceStatus()
        {
            SendMessageToAllClients((id, client) => {

                client.SendServiceStaus(GetServiceStatus());

            });
        }

        private void DataService_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            _publishQueue.Add(e);
        }

        public void RequestCalculate()
        {
            _dataServiceProvider.RequestCalculate();
        }

        public void LoadPositions()
        {
            _dataServiceProvider.LoadPositions();
        }
    }
}