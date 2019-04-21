//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        internal Subscriptions<(int Id, string Column)> _positionSubscriptions;
        internal Subscriptions<(int Id, string Column)> _portfolioSubscriptions;

        private System.Timers.Timer _providerDataRefreshTimer;
        private bool _isProviderRefreshRunning;

        private ObservableValue<DateTime> _portfolioDate;

        public event EventHandler<ClientConnectionChangedEventArgs> OnClientConnectionChanged;

        public IReadOnlyList<IDataServiceClient> Clients => _clients;

        public string DefaultMessage { get; set; } = "Getting data... please wait";

        /// <summary>
        /// In milliseconds
        /// </summary>
        public int ProviderPollingInterval { get; set; } = 2000;

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

                c.SendPortfolioDate(_portfolioDate.Value);

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
                if (_isProviderRefreshRunning)
                    return;

                _providerDataRefreshTimer = new System.Timers.Timer(ProviderPollingInterval);
                _providerDataRefreshTimer.Elapsed += UpdateDataFromProvider;
                _providerDataRefreshTimer.Start();

                _isProviderRefreshRunning = true;
            }
        }

        public void Stop()
        {
            lock(this)
            {
                if (!_isProviderRefreshRunning)
                    return;

                if (_providerDataRefreshTimer is object)
                {
                    _providerDataRefreshTimer.Elapsed -= UpdateDataFromProvider;
                    _providerDataRefreshTimer.Stop();
                    _providerDataRefreshTimer.Dispose();
                }

                _isProviderRefreshRunning = false;
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

        public void UnRegister()
        {
            var c = OperationContext.Current.GetCallbackChannel<IDataServiceClient>();
            UnRegister(c);
        }

        private void UnRegister(IDataServiceClient c)
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

        public void SubscribeToPortfolioDate()
        {
            if(_portfolioDate is null)
            {
                _portfolioDate = new ObservableValue<DateTime>();
                _portfolioDate.PropertyChanged += PortfolioDateChanged;
            }
            
            UpdatePortfolioDate();

            SendMessageToAllClients(c => {

                c.SendPortfolioDate(_portfolioDate.Value);

            });
        }

        private void UpdatePositionSubscriptions()
        {
            var keys = _positionSubscriptions.GetKeys();

            foreach (var (Id, Column) in keys)
            {
                var value = _dataServiceProvider.GetPositionValue(Id, Column);

                _positionSubscriptions.Get((Id, Column)).Value = value;
            }
        }

        private void UpdatePortfolioSubscriptions()
        {
            var keys = _portfolioSubscriptions.GetKeys();
            
            foreach (var (Id, Column) in keys)
            {
                var value = _dataServiceProvider.GetPortfolioValue(Id, Column);

                _portfolioSubscriptions.Get((Id, Column)).Value = value;
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
                Task.Run(() => {

                    UpdatePositionSubscriptions();

                    UpdatePortfolioSubscriptions();

                    UpdatePortfolioDate();
                });
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

                    UnRegister(c);
                }
            }
        }

        public List<int> GetPositions(int folioId)
        {
            return _dataServiceProvider.GetPositions(folioId);
        }
    }
}