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
using sophis.scenario;
using sophis.utils;

namespace RxdSolutions.FusionLink
{

    public class Main : IMain
    {
        public static ServiceHost _host;
        public static SynchronizationContext _context;

        public static DataServer DataServer;
        public static CaptionBar CaptionBar;

        private TimeSpan? _lastRefreshTimeTakenInUI = null;
        private TimeSpan? _lastRefreshTimeTakenOverall = null;

        public void EntryPoint()
        {
            RegisterServer();

            RegisterUI();
        }

        private void RegisterUI()
        {
            CSMScenario.Register("Display FusionLink", new ShowFusionLinkScenario());

            CSMPositionCtxMenu.Register("Copy Cell as Excel Reference", new CopyRTDCellToClipboard());
            CSMPositionCtxMenu.Register("Copy Table as Excel References", new CopyRTDTableToClipboard());

            CaptionBar = new CaptionBar();
            CaptionBar.OnButtonClicked += OnCaptionBarButtonClicked;
            CaptionBar.DisplayButton = true;
            CaptionBar.ButtonText = "Stop";
            CaptionBar.ButtonToolTip = "Click here to Start / Stop FusionLink";
            CaptionBar.Image = Properties.Resources.InfoIcon;
            CaptionBar.Show();
        }

        private void OnCaptionBarButtonClicked(object sender, EventArgs e)
        {
            if (DataServer == null)
                return;

            if (DataServer.IsRunning)
                DataServer.Stop();
            else
                DataServer.Start();

            CaptionBar.ButtonText = DataServer.IsRunning ? "Stop" : "Start";

            UpdateCaption();
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
                    _host = DataServerHostFactory.Create(DataServer);
                }
                catch (AddressAlreadyInUseException)
                {
                    //Another Sophis has already assumed the role of server. Sink the exception.
                    CSMLog.Write("Main", "EntryPoint", CSMLog.eMVerbosity.M_error, "Another instance is already listening and acting as the RTD Server");

                    _host = null;
                }

                DataServer = _host.SingletonInstance as DataServer;

                DataServer.Start();
                DataServer.OnClientConnectionChanged += OnClientConnectionChanged;
                DataServer.OnDataUpdatedFromProvider += OnDataUpdatedFromProvider;

#if DEBUG
                DataServer.OnSubscriptionChanged += OnSubscriptionChanged;
#endif

            }).ContinueWith (t => {
                UpdateCaption();
            }, uiThreadScheduler);
        }

        private void OnSubscriptionChanged(object sender, EventArgs e)
        {
            _context.Post(x => {

                UpdateCaption();

            }, null);
        }

        private static void CreateDataServerFromConfig(SophisDataServiceProvider dataServiceProvider)
        {
            DataServer = new DataServer(dataServiceProvider);

            int refreshRate = 0;
            string defaultMessage = "";
            CSMConfigurationFile.getEntryValue("FusionLink", "RefreshRate", ref refreshRate, 30);
            CSMConfigurationFile.getEntryValue("FusionLink", "DefaultMessage", ref defaultMessage, "Getting data... please wait");

            DataServer.ProviderPollingInterval = refreshRate;
            DataServer.DefaultMessage = defaultMessage;
        }

        private void UpdateCaption()
        {
            var caption = new StringBuilder();

            //Listening
            int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            string dataServiceIdentifierCaption = $"FusionLink Connection Id is {processId}";
            caption.Append(dataServiceIdentifierCaption);

            //Clients
            int clientCount = DataServer.Clients.Count;

            if (clientCount > 0)
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
            }

            if (DataServer.IsRunning)
            {
                //Config
                string refreshRate = $"Refreshing every {DataServer.ProviderPollingInterval} second(s)";
                caption.Append(" / ");
                caption.Append(refreshRate);

                //Perf
                if (_lastRefreshTimeTakenInUI.HasValue)
                {
                    string lastRefresh = $"Last refresh took {Math.Round(_lastRefreshTimeTakenInUI.Value.TotalSeconds, 1)} second(s)";
                    caption.Append(" / ");
                    caption.Append(lastRefresh);

#if DEBUG
                    var lastRefreshOverall = $" ({Math.Round(_lastRefreshTimeTakenOverall.Value.TotalSeconds, 1)} overall)";
                    caption.Append(lastRefreshOverall);
#endif
                }
            }
            else
            {
                caption.Append(" / ");
                caption.Append($"FusionLink stopped");
            }
                
#if DEBUG
            var subs = $"(Subscriptions = Portfolio:{DataServer.PortfolioSubscriptionCount},Position:{DataServer.PositonSubscriptionCount},System:{DataServer.SystemValueCount})";
            caption.Append(" / ");
            caption.Append(subs);
#endif

            CaptionBar.Text = caption.ToString();
        }

        private void OnClientConnectionChanged(object sender, ClientConnectionChangedEventArgs e)
        {
            if (DataServer.Clients.Count > 0)
            {
                if (!DataServer.IsRunning)
                {
                    DataServer.Start();
                }
            }
            
            _context.Post(x => 
            {
                UpdateCaption();

            }, null);
        }

        private void OnDataUpdatedFromProvider(object sender, DataUpdatedFromProviderEventArgs e)
        {
            _lastRefreshTimeTakenInUI = e.UITimeTaken;
            _lastRefreshTimeTakenOverall = e.OverallTime;

            _context.Post(x => {

                UpdateCaption();

            }, null);
        }

        public void Close()
        {
            DataServer.OnClientConnectionChanged -= OnClientConnectionChanged;
            DataServer.Stop();
            _host?.Close();
        }
    }
}
