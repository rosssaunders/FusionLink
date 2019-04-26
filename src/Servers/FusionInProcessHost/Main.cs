//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using sophis;
using sophis.misc;
using sophis.portfolio;
using sophis.utils;

namespace RxdSolutions.FusionLink
{
    public class Main : IMain
    {
        public static ServiceHost _host;
        public static SynchronizationContext _context;

        private static DataServer _dataServer;
        private static CaptionBar _captionBar;

        private TimeSpan? _lastRefreshTimeTakenInUI = null;
        private TimeSpan? _lastRefreshTimeTakenOverall = null;

        public void EntryPoint()
        {
            RegisterServer();

            RegisterUI();
        }

        private void RegisterUI()
        {
            CSMPositionCtxMenu.Register("Copy Cell as Excel Reference", new CopyRTDCellToClipboard());

            CSMPositionCtxMenu.Register("Copy Table as Excel References", new CopyRTDTableToClipboard());

            _captionBar = new CaptionBar();
        }

        private void RegisterServer()
        {
            _context = SynchronizationContext.Current;
            var uiThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            var dataServiceProvider = new SophisDataServiceProvider(_context);

            CreateDataServerFromConfig(dataServiceProvider);

            _ = Task.Run(() => {

                try
                {
                    _host = DataServerHostFactory.Create(_dataServer);
                }
                catch (AddressAlreadyInUseException)
                {
                    //Another Sophis has already assumed the role of server. Sink the exception.
                    CSMLog.Write("Main", "EntryPoint", CSMLog.eMVerbosity.M_error, "Another instance is already listening and acting as the RTD Server");

                    _host = null;
                }

                _dataServer = _host.SingletonInstance as DataServer;

                _dataServer.Start();
                _dataServer.OnClientConnectionChanged += OnClientConnectionChanged;
                _dataServer.OnDataUpdatedFromProvider += OnDataUpdatedFromProvider;

#if DEBUG
                _dataServer.OnSubscriptionChanged += OnSubscriptionChanged;
#endif

            }).ContinueWith (t => {
                UpdateCaption();
            }, uiThreadScheduler);
        }

        private void OnSubscriptionChanged(object sender, EventArgs e)
        {
            _context.Send(x => {

                UpdateCaption();

            }, null);
        }

        private static void CreateDataServerFromConfig(SophisDataServiceProvider dataServiceProvider)
        {
            _dataServer = new DataServer(dataServiceProvider);

            int refreshRate = 0;
            string defaultMessage = "";
            CSMConfigurationFile.getEntryValue("FusionLink", "RefreshRate", ref refreshRate, 30);
            CSMConfigurationFile.getEntryValue("FusionLink", "DefaultMessage", ref defaultMessage, "Getting data... please wait");

            _dataServer.ProviderPollingInterval = refreshRate;
            _dataServer.DefaultMessage = defaultMessage;
        }

        private void UpdateCaption()
        {
            var caption = new StringBuilder();

            //Listening
            var processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            var dataServiceIdentifierCaption = $"FusionLink Connection Id is {processId}";
            caption.Append(dataServiceIdentifierCaption);

            //Clients
            var clientCount = _dataServer.Clients.Count;

            if(clientCount > 0)
            {
                string clientsConnectedCaption = "";
                if (clientCount == 1)
                {
                    clientsConnectedCaption = $" {clientCount} client connected";
                }
                else if (clientCount > 1)
                {
                    clientsConnectedCaption = $" {clientCount} clients connected";
                }

                caption.Append(" / ");
                caption.Append(clientsConnectedCaption);

                //Config
                var refreshRate = $"Refreshing every {_dataServer.ProviderPollingInterval} second(s)";
                caption.Append(" / ");
                caption.Append(refreshRate);

                //Perf
                if (_lastRefreshTimeTakenInUI.HasValue)
                {
                    var lastRefresh = $"Last refresh took {Math.Round(_lastRefreshTimeTakenInUI.Value.TotalSeconds, 1)} second(s)";
                    caption.Append(" / ");
                    caption.Append(lastRefresh);

#if DEBUG
                    var lastRefreshOverall = $"({Math.Round(_lastRefreshTimeTakenOverall.Value.TotalSeconds, 1)} overall)";
                    caption.Append(" / ");
                    caption.Append(lastRefreshOverall);
#endif
                }
            }

#if DEBUG
            var subs = $"(Portfolio:{_dataServer.PortfolioSubscriptionCount},Position:{_dataServer.PositonSubscriptionCount},System:{_dataServer.SystemValueCount})";
            caption.Append(" / ");
            caption.Append(subs);
#endif

            _captionBar.Text = caption.ToString();
            _captionBar.Show();
        }

        private void OnClientConnectionChanged(object sender, ClientConnectionChangedEventArgs e)
        {
            _context.Send(x => 
            {
                if(_dataServer.Clients.Count == 0)
                {
                    if(_dataServer.IsRunning)
                    {
                        _dataServer.Stop();
                        _lastRefreshTimeTakenInUI = null;
                    }
                }
                else
                {
                    if (!_dataServer.IsRunning)
                    {
                        _dataServer.Start();
                    }
                }

                UpdateCaption();

            }, null);
        }

        private void OnDataUpdatedFromProvider(object sender, DataUpdatedFromProviderEventArgs e)
        {
            _lastRefreshTimeTakenInUI = e.UITimeTaken;
            _lastRefreshTimeTakenOverall = e.OverallTime;

            _context.Send(x => {

                UpdateCaption();

            }, null);
        }

        public void Close()
        {
            _dataServer.OnClientConnectionChanged -= OnClientConnectionChanged;
            _dataServer.Stop();
            _host?.Close();
        }
    }
}
