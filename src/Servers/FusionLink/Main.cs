//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.Properties;
using RxdSolutions.FusionLink.Services;
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

        private static PortfolioActionListener _portfolioActionListener;
        private static PortfolioEventListener _portfolioEventListener;
        private static PositionActionListener _positionActionListener;
        private static PositionEventListener _positionEventListener;
        private static TransactionActionListener _transactionActionListener;
        private static TransactionEventListener _transactionEventListener;

        private static ServiceHost _host;
        
        private bool _displayDebugMessage = false;

        public static DataServer DataServer;
        public static CaptionBar CaptionBar;

        public Dispatcher _context;

        public void EntryPoint()
        {
            if (UserRight.CanOpen())
            {
                _context = Dispatcher.CurrentDispatcher;

                LoadConfig();

                RegisterListeners();

                RegisterScenarios();

                LoadFusionLink();
            }
        }

        public void Close()
        {
            try
            {
                DataServer.OnClientConnectionChanged -= OnClientConnectionChanged;
                DataServer.Close();
                _host?.Close();
            }
            catch (Exception ex)
            {
                CSMLog.Write("Main", "Close", CSMLog.eMVerbosity.M_error, ex.ToString());
            }
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

            _positionEventListener = new PositionEventListener();
            CSMPositionEvent.Register("PositionEventListener", CSMPositionEvent.eMOrder.M_oAfter, _positionEventListener);

            _positionActionListener = new PositionActionListener();
            CSMPositionAction.Register("PositionActionListener", CSMPositionAction.eMOrder.M_oAfter, _positionActionListener);

            _transactionEventListener = new TransactionEventListener();
            CSMTransactionEvent.Register("TransactionEventListener", CSMTransactionEvent.eMOrder.M_oAfter, _transactionEventListener);

            _transactionActionListener = new TransactionActionListener();
            CSMTransactionAction.Register("TransactionActionListener", CSMTransactionAction.eMOrder.M_oSavingInDataBase, _transactionActionListener);
        }

        private void RegisterScenarios()
        {
            CSMScenario.Register(Resources.ScenarioShowCaptionBarMessage, new ShowFusionLinkScenario());
            CSMScenario.Register(Resources.OpenFusionLinkExcel, new OpenFusionLinkExcelScenario());
            CSMScenario.Register(Resources.ShowDiagnostics, new ShowDiagnosticsScenario());
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
            var aggregatePortfolioListener = new AggregatePortfolioListener(_portfolioActionListener, _portfolioEventListener);
            var aggregatePositionListener = new AggregatePositionListener(_positionActionListener, _positionEventListener);
            var aggregateTransactionListener = new AggregateTransactionListener(_transactionActionListener, _transactionEventListener);

            var dataServiceProvider = new FusionDataServiceProvider(_globalFunctions as IGlobalFunctions,
                                                                    aggregatePortfolioListener,
                                                                    aggregatePositionListener,
                                                                    aggregateTransactionListener,
                                                                    new PositionService(),
                                                                    new InstrumentService(),
                                                                    new CurveService());

            CreateDataServerFromConfig(dataServiceProvider);

            return Task.Run(() =>
            {
                try
                {
                    _host = DataServerHostFactory.Create(DataServer);
                    _host.Faulted += Host_Faulted;
                }
                catch (AddressAlreadyInUseException)
                {
                    //Another Sophis has already assumed the role of server. Sink the exception.
                    CSMLog.Write("Main", "EntryPoint", CSMLog.eMVerbosity.M_error, "Another instance is already listening and acting as the FusionLink Server");

                    _host = null;
                }

                DataServer = _host.SingletonInstance as DataServer;

                DataServer.Start();
                DataServer.OnClientConnectionChanged += OnClientConnectionChanged;

                if(_displayDebugMessage)
                    DataServer.OnSubscriptionChanged += OnSubscriptionChanged;
            });
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            CSMLog.Write("Main", "Host_Faulted", CSMLog.eMVerbosity.M_error, "The FusionInvest host has faulted.");
        }

        private void OnSubscriptionChanged(object sender, EventArgs e)
        {
            _context.InvokeAsync(() => {

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

            if (DataServer.ClientCount > 0)
            {
                string clientsConnectedCaption = "";
                if (DataServer.ClientCount == 1)
                {
                    clientsConnectedCaption = string.Format(Resources.SingleClientConnectedMessage, DataServer.ClientCount);
                }
                else if (DataServer.ClientCount > 1)
                {
                    clientsConnectedCaption = string.Format(Resources.MultipleClientsConnectedMessage, DataServer.ClientCount);
                }

                caption.Append(" / ");
                caption.Append(clientsConnectedCaption);
            }

            if (_displayDebugMessage)
            {
                var subs = $"(Subs = PortVal:{DataServer.PortfolioValueSubscriptionCount},PortProp:{DataServer.PortfolioPropertySubscriptionCount},Pos:{DataServer.PositonValueSubscriptionCount},Sys:{DataServer.SystemValueCount})";
                caption.Append(" / ");
                caption.Append(subs);
            }                

            CaptionBar.Text = caption.ToString();
        }

        private void OnClientConnectionChanged(object sender, ClientConnectionChangedEventArgs e)
        {
            _context.InvokeAsync(() => 
            {
                UpdateCaption();

            }, DispatcherPriority.ApplicationIdle);
        }
    }
}
