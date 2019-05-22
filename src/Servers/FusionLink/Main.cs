//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Properties;
using sophis;
using sophis.misc;
using sophis.portfolio;
using sophis.scenario;
using sophis.utils;

namespace RxdSolutions.FusionLink
{
    public class Main : IMain
    {
        public static CSMGlobalFunctions _globalFunctions;
        public static SophisLoadedScenario _sophisLoadedScenario;

        public static ServiceHost _host;
        public static SynchronizationContext _context;

        public static DataServer DataServer;
        public static CaptionBar CaptionBar;

        public void EntryPoint()
        {
            _globalFunctions = new FusionInvestGlobalFunctions();
            CSMGlobalFunctions.Register(_globalFunctions);

            _sophisLoadedScenario = new SophisLoadedScenario();
            CSMScenario.Register("SophisLoadedScenario", _sophisLoadedScenario);

            SophisLoadedScenario.OnAfterAllInitialisation += (s, e) => {

                RegisterServer();

                RegisterUI();

            };
        }

        private void RegisterUI()
        {
            CSMScenario.Register(Resources.ScenarioShowCaptionBarMessage, new ShowFusionLinkScenario());
            CSMPositionCtxMenu.Register(Resources.CopyCellAsExcelReference, new CopyRTDCellToClipboard());
            CSMPositionCtxMenu.Register(Resources.CopyTableAsExcelReference, new CopyRTDTableToClipboard());

            CaptionBar = new CaptionBar();
            CaptionBar.OnButtonClicked += OnCaptionBarButtonClicked;
            CaptionBar.DisplayButton = true;
            CaptionBar.ButtonText = Resources.StopButtonText;
            CaptionBar.ButtonToolTip = Resources.StartStopButtonTooltip;
            CaptionBar.Image = Resources.InfoIcon;
            CaptionBar.Show();

            CaptionBar.Text = Resources.LoadingMessage;
        }

        private void OnCaptionBarButtonClicked(object sender, EventArgs e)
        {
            if (DataServer == null)
                return;

            if (DataServer.IsRunning)
                DataServer.Stop();
            else
                DataServer.Start();

            CaptionBar.ButtonText = DataServer.IsRunning ? Resources.StopButtonText : Resources.StartButtonText;

            UpdateCaption();
        }

        private void RegisterServer()
        {
            _context = SynchronizationContext.Current;
            var uiThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            var dataServiceProvider = new FusionDataServiceProvider(_context, _globalFunctions as IGlobalFunctions);

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

        private static void CreateDataServerFromConfig(FusionDataServiceProvider dataServiceProvider)
        {
            DataServer = new DataServer(dataServiceProvider);

            int refreshRate = 0;
            string defaultMessage = "";
            CSMConfigurationFile.getEntryValue("FusionLink", "DefaultMessage", ref defaultMessage, Resources.DefaultDataLoadingMessage);

            DataServer.ProviderPollingInterval = refreshRate;
            DataServer.DefaultMessage = defaultMessage;
        }

        private void UpdateCaption()
        {
            var caption = new StringBuilder();

            //Listening
            int processId = Process.GetCurrentProcess().Id;
            string dataServiceIdentifierCaption = string.Format(Resources.ConnectionIdMessage, processId);
            caption.Append(dataServiceIdentifierCaption);

            //Clients
            int clientCount = DataServer.Clients.Count;

            if (clientCount > 0)
            {
                string clientsConnectedCaption = "";
                if (clientCount == 1)
                {
                    clientsConnectedCaption = string.Format(Resources.SingleClientConnectedMessage, clientCount);
                }
                else if (clientCount > 1)
                {
                    clientsConnectedCaption = string.Format(Resources.MultipleClientsConnectedMessage, clientCount);
                }

                caption.Append(" / ");
                caption.Append(clientsConnectedCaption);
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
            _context.Post(x => {

                UpdateCaption();

            }, null);
        }

        public void Close()
        {
            DataServer.OnClientConnectionChanged -= OnClientConnectionChanged;
            DataServer.Close();
            _host?.Close();
        }
    }
}
