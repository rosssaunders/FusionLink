//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DataServer : IDataServiceServer
    {
        private readonly Dictionary<string, IDataServiceClient> _clients;
        private readonly IDataServerProvider _dataServiceProvider;

        private Subscriptions<(int Id, string Column)> _positionSubscriptions;
        private Subscriptions<(int Id, string Column)> _portfolioSubscriptions;
        private Subscriptions<SystemProperty> _systemSubscriptions;

        private AutoResetEvent _providerRefreshRunningResetEvent;
        private Thread _providerRefreshThread;

        private AutoResetEvent _clientMonitorResetEvent;
        private Thread _clientMonitorThread;
        private int _clientCheckInterval;

        public event EventHandler<ClientConnectionChangedEventArgs> OnClientConnectionChanged;
        public event EventHandler<DataUpdatedFromProviderEventArgs> OnDataUpdatedFromProvider;
        public event EventHandler<EventArgs> OnSubscriptionChanged;

        public IReadOnlyList<IDataServiceClient> Clients => _clients.Values.ToList();

        public string DefaultMessage { get; set; } = "Getting data... please wait";

        /// <remarks>
        /// In seconds
        /// </remarks>
        public int ProviderPollingInterval { get; set; } = 30;

        public bool IsRunning { get; private set; }

        public DataServer(IDataServerProvider dataService)
        {
            _clients = new Dictionary<string, IDataServiceClient>();
            _dataServiceProvider = dataService;

            _positionSubscriptions = new Subscriptions<(int, string)>() { DefaultMessage = DefaultMessage };
            _positionSubscriptions.OnValueChanged += PositionDataPointChanged;

            _portfolioSubscriptions = new Subscriptions<(int, string)>() { DefaultMessage = DefaultMessage }; 
            _portfolioSubscriptions.OnValueChanged += PortfolioDataPointChanged;

            _systemSubscriptions = new Subscriptions<SystemProperty>() { DefaultMessage = DefaultMessage };
            _systemSubscriptions.OnValueChanged += SystemDataPointChanged;

            _providerRefreshRunningResetEvent = new AutoResetEvent(false);
            _clientMonitorResetEvent = new AutoResetEvent(false);

            _clientCheckInterval = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;
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

                _providerRefreshThread = new Thread(new ThreadStart(UpdateDataFromProvider));
                _providerRefreshThread.Start();
               
                _clientMonitorThread = new Thread(new ThreadStart(CheckClientsAlive));
                _clientMonitorThread.Start();
            }
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

                _providerRefreshRunningResetEvent.Set();

                if (!_providerRefreshThread.Join(TimeSpan.FromSeconds(5)))
                {
                    _providerRefreshThread.Abort();
                }

                _clientMonitorResetEvent.Set();

                if (!_clientMonitorThread.Join(TimeSpan.FromSeconds(5)))
                {
                    _clientMonitorThread.Abort();
                }
            }
        }

        public void Register()
        {
            var c = OperationContext.Current.GetCallbackChannel<IDataServiceClient>();

            lock(_clients)
            {
                if (!_clients.ContainsKey(OperationContext.Current.SessionId))
                    _clients.Add(OperationContext.Current.SessionId, c);
            }

            OnClientConnectionChanged?.Invoke(this, new ClientConnectionChangedEventArgs(ClientConnectionStatus.Connected, c));
        }

        public void Unregister()
        {
            var c = OperationContext.Current.GetCallbackChannel<IDataServiceClient>();

            Unregister(OperationContext.Current.SessionId, c);
        }

        private void Unregister(string sessionId, IDataServiceClient c)
        {
            lock (_clients)
            {
                if (_clients.ContainsKey(sessionId))
                    _clients.Remove(sessionId);
            }

            foreach(var sub in _portfolioSubscriptions.GetKeys())
                _portfolioSubscriptions.Remove(sessionId, (sub.Id, sub.Column));

            foreach (var sub in _positionSubscriptions.GetKeys())
                _positionSubscriptions.Remove(sessionId, (sub.Id, sub.Column));

            foreach (var sub in _systemSubscriptions.GetKeys())
                _systemSubscriptions.Remove(sessionId, sub);

            OnClientConnectionChanged?.Invoke(this, new ClientConnectionChangedEventArgs(ClientConnectionStatus.Disconnected, null));
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

        public List<int> GetPositions(int folioId, Positions position)
        {
            return _dataServiceProvider.GetPositions(folioId, position);
        }

        public int PositonSubscriptionCount {
            get => _positionSubscriptions.Count;
        }

        public int PortfolioSubscriptionCount {
            get => _portfolioSubscriptions.Count;
        }

        public int SystemValueCount {
            get => _systemSubscriptions.Count;
        }

        private void UpdatePositionSubscriptions()
        {
            var keys = _positionSubscriptions.GetKeys();

            var dict = new Dictionary<(int, string), object>();
            foreach (var key in keys)
                dict.Add(key, null);

            _dataServiceProvider.GetPositionValues(dict);

            foreach (var kvp in dict)
            {
                var dp = _positionSubscriptions.Get(kvp.Key);
                if(dp is object)
                    dp.Value = kvp.Value;
            }
        }

        private void UpdatePortfolioSubscriptions()
        {
            var keys = _portfolioSubscriptions.GetKeys();

            var dict = new Dictionary<(int, string), object>();
            foreach (var key in keys)
                dict.Add(key, null);

            _dataServiceProvider.GetPortfolioValues(dict);

            foreach (var kvp in dict)
            {
                var dp = _portfolioSubscriptions.Get(kvp.Key);
                if (dp is object)
                    dp.Value = kvp.Value;
            }
        }

        private void UpdateDataFromProvider()
        {
            var waitTime = ProviderPollingInterval * 1000;

            while (IsRunning)
            {
                UpdateData();

                _providerRefreshRunningResetEvent.WaitOne(waitTime);
            }
        }

        private void CheckClientsAlive()
        {
            while (IsRunning)
            {
                SendMessageToAllClients((sessionId, client) => client.Heartbeat());

                _clientMonitorResetEvent.WaitOne(_clientCheckInterval);
            }
        }

        private void UpdateData()
        {
            if (_clients.Count == 0)
                return;

            if (!_dataServiceProvider.IsBusy)
            {
                //Avoid overloading the service provider
                lock(_dataServiceProvider)
                {
                    var overallTimer = Stopwatch.StartNew();

                    UpdatePositionSubscriptions();

                    var elapsedUITime = _dataServiceProvider.ElapsedTimeOfLastCall;

                    UpdatePortfolioSubscriptions();

                    elapsedUITime += _dataServiceProvider.ElapsedTimeOfLastCall;

                    UpdateSystemPropertySubscriptions();

                    elapsedUITime += _dataServiceProvider.ElapsedTimeOfLastCall;

                    overallTimer.Stop();

                    OnDataUpdatedFromProvider?.Invoke(this, new DataUpdatedFromProviderEventArgs(elapsedUITime, overallTimer.Elapsed));
                }
            }
        }

        private void UpdateSystemPropertySubscriptions()
        {
            var keys = _systemSubscriptions.GetKeys();

            var dict = new Dictionary<SystemProperty, object>();
            foreach (var key in keys)
                dict.Add(key, null);

            _dataServiceProvider.GetSystemValues(dict);

            foreach (var kvp in dict)
            {
                var dp = _systemSubscriptions.Get(kvp.Key);
                if (dp is object)
                    dp.Value = kvp.Value;
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

                    Unregister(key, c);
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
    }
}