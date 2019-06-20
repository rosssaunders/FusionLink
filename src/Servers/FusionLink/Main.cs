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
        private static CSMGlobalFunctions _globalFunctions;
        private static AfterAllInitializationScenario afterAllInitializationScenario;
        private static PortfolioActionListener _portfolioActionListener;
        private static PortfolioEventListener _portfolioEventListener;
        private static ServiceHost _host;
        private static DataServer _dataServer;

        private bool _displayDebugMessage = false;

        public static CaptionBar CaptionBar;

        public void EntryPoint()
        {
            if (UserRight.CanOpen())
            {
                LoadConfig();

                RegisterListeners();

                RegisterScenarios();

                afterAllInitializationScenario = new AfterAllInitializationScenario();
                afterAllInitializationScenario.AfterAllInitialization += AfterAllInitialization;
                CSMScenario.Register("AfterAllInitialization", afterAllInitializationScenario);
            }
        }

        public void Close()
        {
            try
            {
                _dataServer.OnClientConnectionChanged -= OnClientConnectionChanged;
                _dataServer.Close();
                _host?.Close();
            }
            catch (Exception ex)
            {
                CSMLog.Write("Main", "Close", CSMLog.eMVerbosity.M_error, ex.ToString());
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
                var startupTask = RegisterServer().ContinueWith(t => {

                    if(t.Exception == null)
                    {
                        RegisterUI();

                        UpdateCaption();
                    }
                    else
                    {
                        MessageBox.Show(Resources.LoadFailureMessage + Environment.NewLine + Environment.NewLine + t.Exception.ToString(), Resources.ErrorCaption);
                    }

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

        private void RegisterScenarios()
        {
            CSMScenario.Register(Resources.ScenarioShowCaptionBarMessage, new ShowFusionLinkScenario());
            CSMScenario.Register(Resources.OpenFusionLinkExcel, new OpenFusionLinkExcelScenario());
        }

        private void RegisterUI()
        {
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
                if (_dataServer == null)
                    return;

                if (_dataServer.IsRunning)
                    _dataServer.Stop();
                else
                    _dataServer.Start();

                CaptionBar.ButtonText = _dataServer.IsRunning ? Resources.StopButtonText : Resources.StartButtonText;

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

                if(_displayDebugMessage)
                    _dataServer.OnSubscriptionChanged += OnSubscriptionChanged;
            });
        }

        private void OnSubscriptionChanged(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => {

                UpdateCaption();

            });
        }

        private static void CreateDataServerFromConfig(FusionDataServiceProvider dataServiceProvider)
        {
            _dataServer = new DataServer(dataServiceProvider);

            string defaultMessage = "";
            CSMConfigurationFile.getEntryValue("FusionLink", "DefaultMessage", ref defaultMessage, Resources.DefaultDataLoadingMessage);

            _dataServer.DefaultMessage = defaultMessage;
        }

        private void LoadConfig()
        {
            CSMConfigurationFile.getEntryValue("FusionLink", "ShowDebug", ref _displayDebugMessage, false);
        }

        private void UpdateCaption()
        {
            var caption = new StringBuilder();

            //Listening
            int processId = Process.GetCurrentProcess().Id;
            string dataServiceIdentifierCaption = string.Format(Resources.ConnectionIdMessage, processId);
            caption.Append(dataServiceIdentifierCaption);

            //Clients
            int clientCount = _dataServer.Clients.Count;

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

            if (_displayDebugMessage)
            {
                var subs = $"(Subs = PortVal:{_dataServer.PortfolioValueSubscriptionCount},PortProp:{_dataServer.PortfolioPropertySubscriptionCount},Pos:{_dataServer.PositonValueSubscriptionCount},Sys:{_dataServer.SystemValueCount})";
                caption.Append(" / ");
                caption.Append(subs);
            }                

            CaptionBar.Text = caption.ToString();
        }

        private void OnClientConnectionChanged(object sender, ClientConnectionChangedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => 
            {
                UpdateCaption();

            });
        }
    }
}
