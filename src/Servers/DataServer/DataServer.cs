//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DataServer : IDataServiceServer
    {
        private readonly List<IDataServiceClient> _clients;
        private readonly IDataServerProvider _dataServiceProvider;

        private Subscriptions<(int Id, string Column)> _positionSubscriptions;
        private Subscriptions<(int Id, string Column)> _portfolioSubscriptions;
        private Subscriptions<SystemProperty> _systemSubscriptions;

        private AutoResetEvent _runningResetEvent;
        private Thread _refreshThread;

        public event EventHandler<ClientConnectionChangedEventArgs> OnClientConnectionChanged;
        public event EventHandler<DataUpdatedFromProviderEventArgs> OnDataUpdatedFromProvider;
        public event EventHandler<EventArgs> OnSubscriptionChanged;

        public IReadOnlyList<IDataServiceClient> Clients => _clients;

        public string DefaultMessage { get; set; } = "Getting data... please wait";

        /// <remarks>
        /// In seconds
        /// </remarks>
        public int ProviderPollingInterval { get; set; } = 30;

        public bool IsRunning { get; private set; }

        public DataServer(IDataServerProvider dataService)
        {
            _clients = new List<IDataServiceClient>();
            _dataServiceProvider = dataService;

            _positionSubscriptions = new Subscriptions<(int, string)>() { DefaultMessage = DefaultMessage };
            _positionSubscriptions.OnValueChanged += PositionDataPointChanged;

            _portfolioSubscriptions = new Subscriptions<(int, string)>() { DefaultMessage = DefaultMessage }; 
            _portfolioSubscriptions.OnValueChanged += PortfolioDataPointChanged;

            _systemSubscriptions = new Subscriptions<SystemProperty>() { DefaultMessage = DefaultMessage };
            _systemSubscriptions.OnValueChanged += SystemDataPointChanged;

            _runningResetEvent = new AutoResetEvent(false);
        }

        private void SystemDataPointChanged(object sender, DataPointChangedEventArgs<SystemProperty> e)
        {
            SendMessageToAllClients(c => {

                c.SendSystemValue(e.DataPoint.Key, e.DataPoint.Value);

            });
        }

        private void PortfolioDataPointChanged(object sender, DataPointChangedEventArgs<(int Id, string Column)> e)
        {
            SendMessageToAllClients(c => {

                c.SendPortfolioValue(e.DataPoint.Key.Id, e.DataPoint.Key.Column, e.DataPoint.Value);

            });
        }

        private void PositionDataPointChanged(object sender, DataPointChangedEventArgs<(int Id, string Column)> e)
        {
            SendMessageToAllClients(c => {

                c.SendPositionValue(e.DataPoint.Key.Id, e.DataPoint.Key.Column, e.DataPoint.Value);

            });
        }

        public void Start()
        {
            lock(this)
            {
                if (IsRunning)
                    return;

                IsRunning = true;

                _refreshThread = new Thread(new ThreadStart(UpdateDataFromProvider));
                _refreshThread.Start();
            }
        }

        public void Stop()
        {
            lock(this)
            {
                if (!IsRunning)
                    return;

                IsRunning = false;
                _runningResetEvent.Set();
                
                if(!_refreshThread.Join(TimeSpan.FromSeconds(5)))
                {
                    _refreshThread.Abort();
                }
            }
        }

        public void Register()
        {
            var c = OperationContext.Current.GetCallbackChannel<IDataServiceClient>();

            lock(_clients)
            {
                if (!_clients.Contains(c))
                    _clients.Add(c);
            }

            OnClientConnectionChanged?.Invoke(this, new ClientConnectionChangedEventArgs(ClientConnectionStatus.Connected, c));
        }

        public void Unregister()
        {
            var c = OperationContext.Current.GetCallbackChannel<IDataServiceClient>();

            Unregister(c);
        }

        private void Unregister(IDataServiceClient c)
        {
            lock (_clients)
            {
                if (_clients.Contains(c))
                    _clients.Remove(c);
            }

            OnClientConnectionChanged?.Invoke(this, new ClientConnectionChangedEventArgs(ClientConnectionStatus.Disconnected, c));
        }

        public void SubscribeToPositionValue(int positionId, string column)
        {
            var dp = _positionSubscriptions.Add(OperationContext.Current.SessionId, (positionId, column));

            OnSubscriptionChanged?.Invoke(this, new EventArgs());

            SendMessageToAllClients(c => {

                c.SendPositionValue(dp.Key.Id, dp.Key.Column, dp.Value);

            });
        }

        public void SubscribeToPortfolioValue(int portfolioId, string column)
        {
            var dp = _portfolioSubscriptions.Add(OperationContext.Current.SessionId, (portfolioId, column));

            OnSubscriptionChanged?.Invoke(this, new EventArgs());

            SendMessageToAllClients(c => {

                c.SendPortfolioValue(dp.Key.Id, dp.Key.Column, dp.Value);

            });
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            var dp = _systemSubscriptions.Add(OperationContext.Current.SessionId, property);

            OnSubscriptionChanged?.Invoke(this, new EventArgs());

            SendMessageToAllClients(c => {

                c.SendSystemValue(dp.Key, dp.Value);

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
            var waitTime = (int)TimeSpan.FromSeconds(ProviderPollingInterval).TotalMilliseconds;

            while (IsRunning)
            {
                UpdateData();

                _runningResetEvent.WaitOne(waitTime);
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

        private void SendMessageToAllClients(Action<IDataServiceClient> send)
        {
            if (_clients.Count == 0)
                return;

            // send number to clients
            int ix = 0;
            while (ix < _clients.Count)
            {
                // can't do foreach because we want to remove dead ones
                var c = _clients[ix];
                try
                {
                    send.Invoke(c);
                    
                    ix++;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);

                    Unregister(c);
                }
            }
        }

        public List<int> GetPositions(int folioId)
        {
            return _dataServiceProvider.GetPositions(folioId);
        }
    }
}