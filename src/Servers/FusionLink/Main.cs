//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
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
        public static AfterAllInitializationScenario afterAllInitializationScenario;
        public static PortfolioActionListener _portfolioActionListener;
        public static PortfolioEventListener _portfolioEventListener;

        public static ServiceHost _host;

        public static DataServer DataServer;
        public static CaptionBar CaptionBar;

        public void EntryPoint()
        {
            if (UserRight.CanOpen())
            {
                afterAllInitializationScenario = new AfterAllInitializationScenario();
                afterAllInitializationScenario.AfterAllInitialization += AfterAllInitialization;
                CSMScenario.Register("AfterAllInitialization", afterAllInitializationScenario);
            }
        }

        private void AfterAllInitialization(object sender, EventArgs e)
        {
            LoadFusionLink();
        }

        private void LoadFusionLink()
        {
            try
            {
                RegisterListeners();

                var startupTask = RegisterServer().ContinueWith(t => {

                    RegisterUI();

                    UpdateCaption();

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.LoadFailureMessage + Environment.NewLine + Environment.NewLine + ex.ToString(), Resources.ErrorCaption);
            }
        }

        private void RegisterListeners()
        {
            _globalFunctions = new FusionInvestGlobalFunctions();
            CSMGlobalFunctions.Register(_globalFunctions);

            _portfolioEventListener = new PortfolioEventListener();
            CSMPortfolioEvent.Register("PortfolioEventListener", CSMPortfolioEvent.eMOrder.M_oAfter, _portfolioEventListener);

            _portfolioActionListener = new PortfolioActionListener();
            CSMPortfolioAction.Register("PortfolioActionListener", CSMPortfolioAction.eMOrder.M_oAfter, _portfolioActionListener);
        }

        private void RegisterUI()
        {
            CSMScenario.Register(Resources.ScenarioShowCaptionBarMessage, new ShowFusionLinkScenario());
            CSMScenario.Register(Resources.OpenFusionLinkExcel, new OpenFusionLinkExcelScenario());

            CSMPositionCtxMenu.Register(Resources.CopyCellAsExcelReference, new CopyCellAsRTDFunctionToClipboard());
            CSMPositionCtxMenu.Register(Resources.CopyTableAsExcelReference, new CopyRowAsRTDTableToClipboard());

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
            try
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
            catch (Exception ex)
            {
                CSMLog.Write("Main", "OnCaptionBarButtonClicked", CSMLog.eMVerbosity.M_error, ex.ToString());

                MessageBox.Show(ex.Message, Resources.ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Task RegisterServer()
        {
            var aggregateListener = new AggregateListener(_portfolioActionListener, _portfolioEventListener);
            var dataServiceProvider = new FusionDataServiceProvider(_globalFunctions as IGlobalFunctions, aggregateListener);

            CreateDataServerFromConfig(dataServiceProvider);

            return Task.Run(() =>
            {

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

            });
        }

        private void OnSubscriptionChanged(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(() => {

                UpdateCaption();

            });
        }

        private static void CreateDataServerFromConfig(FusionDataServiceProvider dataServiceProvider)
        {
            DataServer = new DataServer(dataServiceProvider);

            string defaultMessage = "";
            CSMConfigurationFile.getEntryValue("FusionLink", "DefaultMessage", ref defaultMessage, Resources.DefaultDataLoadingMessage);

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

            Dispatcher.CurrentDispatcher.InvokeAsync(() => 
            {
                UpdateCaption();

            });
        }

        private void OnDataUpdatedFromProvider(object sender, DataUpdatedFromProviderEventArgs e)
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(() => {

                UpdateCaption();

            });
        }

        public void Close()
        {
            try
            {
                DataServer.OnClientConnectionChanged -= OnClientConnectionChanged;
                DataServer.Close();
                _host?.Close();
            }
            catch(Exception ex)
            {
                CSMLog.Write("Main", "Close", CSMLog.eMVerbosity.M_error, ex.ToString());
            }
        }
    }
}
