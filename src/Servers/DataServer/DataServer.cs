//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
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

        internal Subscriptions<(int Id, string Column)> _positionSubscriptions;
        internal Subscriptions<(int Id, string Column)> _portfolioSubscriptions;

        private System.Timers.Timer _providerDataRefreshTimer;

        private ObservableValue<DateTime> _portfolioDate;

        public event EventHandler<ClientConnectionChangedEventArgs> OnClientConnectionChanged;
        public event EventHandler<DataUpdatedFromProviderEventArgs> OnDataUpdatedFromProvider;

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
        }

        private void PortfolioDateChanged(object sender, PropertyChangedEventArgs e)
        {
            SendMessageToAllClients(c => {

                c.SendSystemValue(SystemProperty.PortfolioDate, _portfolioDate.Value);

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

                _providerDataRefreshTimer = new System.Timers.Timer(TimeSpan.FromSeconds(ProviderPollingInterval).TotalMilliseconds);
                _providerDataRefreshTimer.Elapsed += UpdateDataFromProvider;
                _providerDataRefreshTimer.Start();

                IsRunning = true;
            }
        }

        public void Stop()
        {
            lock(this)
            {
                if (!IsRunning)
                    return;

                if (_providerDataRefreshTimer is object)
                {
                    _providerDataRefreshTimer.Elapsed -= UpdateDataFromProvider;
                    _providerDataRefreshTimer.Stop();
                    _providerDataRefreshTimer.Dispose();
                }

                IsRunning = false;
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
            var dp = _positionSubscriptions.Add((positionId, column));

            SendMessageToAllClients(c => {

                c.SendPositionValue(dp.Key.Id, dp.Key.Column, dp.Value);

            });
        }

        public void SubscribeToPortfolioValue(int portfolioId, string column)
        {
            var dp = _portfolioSubscriptions.Add((portfolioId, column));

            SendMessageToAllClients(c => {

                c.SendPortfolioValue(dp.Key.Id, dp.Key.Column, dp.Value);

            });
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            if(property == SystemProperty.PortfolioDate)
            {
                if (_portfolioDate is null)
                {
                    _portfolioDate = new ObservableValue<DateTime>();
                    _portfolioDate.PropertyChanged += PortfolioDateChanged;
                }

                UpdatePortfolioDate();

                SendMessageToAllClients(c => {

                    c.SendSystemValue(SystemProperty.PortfolioDate, _portfolioDate.Value);

                });
            }
        }

        public void UnsubscribeToPositionValue(int positionId, string column)
        {
            _positionSubscriptions.Remove((positionId, column));
        }

        public void UnsubscribeToPortfolioValue(int portfolioId, string column)
        {
            _portfolioSubscriptions.Remove((portfolioId, column));
        }

        public void UnsubscribeToSystemValue(SystemProperty property)
        {
            if (property == SystemProperty.PortfolioDate)
            {
                if (_portfolioDate is object)
                {
                    _portfolioDate.PropertyChanged -= PortfolioDateChanged;
                    _portfolioDate = null;
                }
            }
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
                _positionSubscriptions.Get(kvp.Key).Value = kvp.Value;
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
                _portfolioSubscriptions.Get(kvp.Key).Value = kvp.Value;
            }
        }

        private void UpdateDataFromProvider(object sender, ElapsedEventArgs e)
        {
            lock(_providerDataRefreshTimer)
            {
                UpdateData();
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
                    Task.Run(() => {

                        var timer = Stopwatch.StartNew();

                        UpdatePositionSubscriptions();

                        UpdatePortfolioSubscriptions();

                        UpdatePortfolioDate();

                        timer.Stop();

                        OnDataUpdatedFromProvider?.Invoke(this, new DataUpdatedFromProviderEventArgs(timer.Elapsed));
                    });
                }
            }
        }

        private void UpdatePortfolioDate()
        {
            if (_portfolioDate is object)
                _portfolioDate.Value = _dataServiceProvider.GetPortfolioDate();
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