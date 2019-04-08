//  Copyright (c) RXD SOlutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using RxdSolutions.Sophis2Excel.Interface;

namespace RxdSolutions.Sophis2Excel
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DataServer : IDataServiceServer
    {
        private readonly List<IDataServiceClient> clients;
        private readonly IDataServiceProvider dataService;

        internal Dictionary<(int positionId, string column), (DataTypeEnum type, object value)> positionSubscriptions = new Dictionary<(int, string), (DataTypeEnum, object)>();
        internal Dictionary<(int portfolioId, string column), (DataTypeEnum type, object value)> portfolioSubscriptions = new Dictionary<(int, string), (DataTypeEnum, object)>();

        private ReaderWriterLockSlim portfolioLock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim positionLock = new ReaderWriterLockSlim();
        
        private readonly System.Timers.Timer timer;

        public DataServer(IDataServiceProvider dataService)
        {
            clients = new List<IDataServiceClient>();
            this.dataService = dataService;

            // send random value to clients every 100ms
            timer = new System.Timers.Timer(2000);
            timer.Elapsed += SendData;
            timer.Start();
        }

        // for clients to make themselves known
        public void Register()
        {
            var c = OperationContext.Current.GetCallbackChannel<IDataServiceClient>();

            lock(clients)
            {
                if (!clients.Contains(c))
                    clients.Add(c);
            }
        }

        // to grafefully close the connection from the client
        public void UnRegister()
        {
            var c = OperationContext.Current.GetCallbackChannel<IDataServiceClient>();
            Unregister(c);
        }

        private void Unregister(IDataServiceClient c)
        {
            lock (clients)
            {
                if (clients.Contains(c))
                    clients.Remove(c);
            }
        }

        public void SubscribeToPosition(int positionId, string column)
        {
            positionLock.EnterWriteLock();

            if (!positionSubscriptions.ContainsKey((positionId, column)))
                positionSubscriptions.Add((positionId, column), (DataTypeEnum.String, "Getting data... please wait"));

            positionLock.ExitWriteLock();

            int ix = 0;
            while (ix < clients.Count)
            {
                var c = clients[ix];
                try
                {
                    c.SendPositionValue(positionId, column, DataTypeEnum.String, "Getting data... please wait");
                    ix++;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);

                    Unregister(c);
                }
            }
        }

        public void SubscribeToPortfolio(int portfolioId, string column)
        {
            portfolioLock.EnterUpgradeableReadLock();
            
            if (!portfolioSubscriptions.ContainsKey((portfolioId, column)))
            {
                portfolioLock.EnterWriteLock();
                portfolioSubscriptions.Add((portfolioId, column), (DataTypeEnum.String, "Getting data... please wait"));
                portfolioLock.ExitWriteLock();
            }
                
            portfolioLock.ExitUpgradeableReadLock();

            int ix = 0;
            while (ix < clients.Count)
            {
                var c = clients[ix];
                try
                {
                    c.SendPortfolioValue(portfolioId, column, DataTypeEnum.String, "Getting data... please wait");
                    ix++;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);

                    Unregister(c);
                }
            }
        }

        private void UpdatePositionSubscriptions()
        {
            positionLock.EnterReadLock();
            var keys = positionSubscriptions.Keys.ToList();
            positionLock.ExitReadLock();

            foreach (var subscription in keys)
            {
                var column = subscription.column;
                var position = subscription.positionId;

                var value = dataService.GetPositionValue(position, column);

                positionLock.EnterWriteLock();
                positionSubscriptions[(position, column)] = value;
                positionLock.ExitWriteLock();
            }
        }

        private void UpdatePortfolioSubscriptions()
        {
            portfolioLock.EnterReadLock();
            var keys = portfolioSubscriptions.Keys.ToList();
            portfolioLock.ExitReadLock();

            foreach (var subscription in keys)
            {
                var column = subscription.column;
                var portfolioId = subscription.portfolioId;

                var value = dataService.GetPortfolioValue(portfolioId, column);

                portfolioLock.EnterWriteLock();
                portfolioSubscriptions[(portfolioId, column)] = value;
                portfolioLock.ExitWriteLock();
            }
        }

        private bool _sendingData = false;

        private void SendData(object sender, ElapsedEventArgs e)
        {
            if (_sendingData)
                return;

            lock(timer)
            {
                _sendingData = true;

                UpdateAndSendData();

                _sendingData = false;
            }
        }

        private void UpdateAndSendData()
        {
            if (clients.Count == 0)
                return;

            if (!dataService.IsBusy)
            {
                Task.Run(() => {

                    UpdatePositionSubscriptions();

                    UpdatePortfolioSubscriptions();

                });
            }
            
            // send number to clients
            int ix = 0;
            while (ix < clients.Count)
            {
                // can't do foreach because we want to remove dead ones
                var c = clients[ix];
                try
                {
                    SendPositionValues(c);

                    SendPortfolioValues(c);

                    ix++;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);

                    Unregister(c);
                }
            }
        }

        private void SendPortfolioValues(IDataServiceClient c)
        {
            if (portfolioSubscriptions.Count == 0)
                return;

            portfolioLock.EnterReadLock();
            
            foreach (var sub in portfolioSubscriptions)
            {
                c.SendPortfolioValue(sub.Key.portfolioId, sub.Key.column, sub.Value.type, sub.Value.value);
            }

            portfolioLock.ExitReadLock();
        }

        private void SendPositionValues(IDataServiceClient c)
        {
            if (positionSubscriptions.Count == 0)
                return;

            positionLock.EnterReadLock();

            foreach (var sub in positionSubscriptions)
            {
                c.SendPositionValue(sub.Key.positionId, sub.Key.column, sub.Value.type, sub.Value.value);
            }

            positionLock.ExitReadLock();
        }
    }
}