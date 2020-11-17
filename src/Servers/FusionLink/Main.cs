//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.Listeners;
using RxdSolutions.FusionLink.Properties;
using RxdSolutions.FusionLink.Provider;
using RxdSolutions.FusionLink.Ribbon;
using RxdSolutions.FusionLink.Services;
using RxdSolutions.Windows.MFC;
using sophis;
using sophis.instrument;
using sophis.misc;
using sophis.portfolio;
using sophis.utils;
using Sophis.Windows;
using Sophis.Windows.Ribbon;

namespace RxdSolutions.FusionLink
{
    public class Main : IMain
    {
        private static CSMGlobalFunctions _globalFunctions;
        private static CSMApi _api;

        private static PortfolioActionListener _portfolioActionListener;
        private static PortfolioEventListener _portfolioEventListener;
        private static PositionActionListener _positionActionListener;
        private static PositionEventListener _positionEventListener;
        private static TransactionActionListener _transactionActionListener;
        private static TransactionEventListener _transactionEventListener;
        private static InstrumentActionListener _instrumentActionListener;
        private static InstrumentEventListener _instrumentEventListener;
        private static PreferenceChangeListener _refChangeListener;

        public static ServiceHost DataServersHost;

        public static OnDemandDataServer OnDemandDataServer;
        public static RealTimeDataServer RealTimeDataServer;

        public static CaptionBar CaptionBar;

        private static DisplayDashboardCommand _displayDashboardCommand;

        public Dispatcher _context;

        public void EntryPoint()
        {
            try
            {
                if (UserRight.CanOpen())
                {
                    _api = sophis.globals.CSMApi.gApi();

                    _context = Dispatcher.CurrentDispatcher;

                    RegisterListeners();

                    RegisterScenarios();

                    var serverRegistrationTask = RegisterServer();

                    if (_api.IsInGUIMode())
                    {
                        serverRegistrationTask.ContinueWith(t => {

                            if (t.Exception == null)
                            {
                                RegisterUI();

                                UpdateCaption();

                                InitialiseMenu();

                                InitialiseSophis();
                            }
                            else
                            {
                                MessageBox.Show(Resources.LoadFailureMessage + Environment.NewLine + Environment.NewLine + t.Exception.ToString(), Resources.ErrorCaption);
                            }

                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    else
                    {
                        InitialiseSophis();
                    }
                }
            }
            catch(Exception ex)
            {
                CSMLog.Write("Main", "OnCaptionBarButtonClicked", CSMLog.eMVerbosity.M_error, ex.ToString());

                if(_api.IsInGUIMode())
                    MessageBox.Show(Resources.LoadFailureMessage + Environment.NewLine + Environment.NewLine + ex.ToString(), Resources.ErrorCaption);
            }
        }

        public void Close()
        {
            try
            {
                _refChangeListener.Dispose();

                RealTimeDataServer.OnClientConnectionChanged -= OnClientConnectionChanged;
                RealTimeDataServer.Close();
                DataServersHost?.Close();
            }
            catch (Exception ex)
            {
                CSMLog.Write("Main", "Close", CSMLog.eMVerbosity.M_error, ex.ToString());
            }
        }

        /// <remarks>
        /// Sophis seems to initialize these when you first open a Portfolio View in the UI
        /// Which a user might want to use FusionLink as more of a headless server
        /// http://www.it-quants.com/Blogs/tabid/83/EntryId/72/Fusion-Capital-Sophis-v7-how-to-listen-to-coherency-events-in-a-batch-server-mode.aspx
        /// </remarks>
        private void InitialiseSophis()
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(new Action(() =>
            {
                Sophis.Event.EventModelValue.Initialize();
                Sophis.Event.EventModelRisque.Initialize();
                Sophis.Event.EventModelDRT.Initialize();

            }), DispatcherPriority.Normal);
        }

        private CSMGlobalFunctions GetGlobalFunctions()
        {
            if(string.Equals(Process.GetCurrentProcess().ProcessName, "SophisValue", StringComparison.InvariantCultureIgnoreCase))
            {
                return new FusionInvestGlobalFunctions();
            }
            else
            {
                return new SophisRisqueGlobalFunctions();
            }
        }

        private void RegisterListeners()
        {
            _globalFunctions = GetGlobalFunctions();
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

            _instrumentEventListener = new InstrumentEventListener();
            CSMInstrumentEvent.Register("InstrumentEventListener", _instrumentEventListener);

            _instrumentActionListener = new InstrumentActionListener();
            CSMInstrumentAction.Register("InstrumentActionListenerModif", CSMInstrumentAction.eMOrder.M_oModification, _instrumentActionListener);
            CSMInstrumentAction.Register("InstrumentActionListenerCreate", CSMInstrumentAction.eMOrder.M_oCreation, _instrumentActionListener);

            _refChangeListener = new PreferenceChangeListener();
            _refChangeListener.AutomaticComputingChanged += AutomaticComputingChanged;
        }

        private void RegisterScenarios()
        {
            _displayDashboardCommand = DisplayDashboardCommand.Register();

            ShowFusionLinkCommand.Register();
            OpenFusionLinkExcelCommand.Register();
            StartStopDataServerCommand.Register();
        }

        private void RegisterUI()
        {
            CSMPositionCtxMenu.Register(Resources.CopyCellAsExcelReference, new CopyCellAsRTDFunctionToClipboard());
            CSMPositionCtxMenu.Register(Resources.CopyTableAsExcelReference, new CopyRowAsRTDTableToClipboard());

            IntPtr windowHandle = Process.GetCurrentProcess().MainWindowHandle;
            CaptionBar = new CaptionBar(windowHandle)
            {
                Image = Resources.InfoIcon,
                DisplayButton = true,
                ButtonText = Resources.CommandDisplayDashboardText,
                ButtonToolTip = Resources.CommandDisplayDashboardTooltip,
                Text = Resources.LoadingMessage
            };

            CaptionBar.OnButtonClicked += OnCaptionBarButtonClicked;
            CaptionBar.Show();
        }

        private static void InitialiseMenu()
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(new Action(() =>
            {
                var ribbon = RibbonBuilder.Instance.GetRibbon();
                RibbonBuilder.Instance.BuildRibbon(ribbon);

            }), DispatcherPriority.Normal);
        }
        
        private void OnCaptionBarButtonClicked(object sender, EventArgs e)
        {
            try
            {
                _displayDashboardCommand.Execute(null, SophisApplication.MainCommandTarget);
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
            var aggregateInstrumentStaticListener = new AggregateInstrumentStaticListener();
            var aggregateInstrumentMarketDataListener = new AggregateInstrumentMarketDataListener();

            var positionService = new PositionService();
            var instrumentService = new InstrumentService();
            var currencyService = new CurrencyService();
            var curveService = new CurveService();
            var transactionService = new TransactionService();
            var priceService = new PriceService();
            var reportService = new ReportService();

            var realTimeProvider = new FusionRealTimeProvider(_globalFunctions as IGlobalFunctions,
                                                                    aggregatePortfolioListener,
                                                                    aggregatePositionListener,
                                                                    aggregateTransactionListener,
                                                                    aggregateInstrumentStaticListener,
                                                                    aggregateInstrumentMarketDataListener,
                                                                    instrumentService,
                                                                    currencyService);

            CreateRealTimeDataServerFromConfig(realTimeProvider);

            var onDemandProvider = new FusionOnDemandProvider(positionService,
                                                                instrumentService,
                                                                currencyService,
                                                                curveService,
                                                                transactionService,
                                                                priceService,
                                                                reportService);

            CreateOnDemandDataServerFromConfig(onDemandProvider);

            return Task.Run(() =>
            {
                try
                {
                    DataServersHost = DataServerHostFactory.Create(new DataServers(RealTimeDataServer, OnDemandDataServer));
                    DataServersHost.Faulted += Host_Faulted;
                }
                catch (AddressAlreadyInUseException)
                {
                    //Another Sophis has already assumed the role of server. Sink the exception.
                    CSMLog.Write("Main", "EntryPoint", CSMLog.eMVerbosity.M_error, "Another instance is already listening and acting as the FusionLink Server");

                    DataServersHost = null;
                }

                RealTimeDataServer.Start();
                RealTimeDataServer.OnClientConnectionChanged += OnClientConnectionChanged;
                RealTimeDataServer.OnStatusChanged += OnStatusChanged;
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

        private static void CreateRealTimeDataServerFromConfig(FusionRealTimeProvider realTimeProvider)
        {
            RealTimeDataServer = new RealTimeDataServer(realTimeProvider);

            string defaultMessage = "";
            CSMConfigurationFile.getEntryValue("FusionLink", "DefaultMessage", ref defaultMessage, Resources.DefaultDataLoadingMessage);

            RealTimeDataServer.DefaultMessage = defaultMessage;
        }

        private static void CreateOnDemandDataServerFromConfig(FusionOnDemandProvider onDemandProvider)
        {
            OnDemandDataServer = new OnDemandDataServer(onDemandProvider);
        }

        private static void UpdateCaption()
        {
            var caption = new StringBuilder();

            int processId = Process.GetCurrentProcess().Id;
            string machineName = Environment.MachineName;
            string dataServiceIdentifierCaption = string.Format(Resources.ConnectionIdMessage, machineName, processId);
            caption.Append(dataServiceIdentifierCaption);

            if (RealTimeDataServer.ClientCount > 0)
            {
                string clientsConnectedCaption = "";
                if (RealTimeDataServer.ClientCount == 1)
                {
                    clientsConnectedCaption = string.Format(Resources.SingleClientConnectedMessage, RealTimeDataServer.ClientCount);
                }
                else if (RealTimeDataServer.ClientCount > 1)
                {
                    clientsConnectedCaption = string.Format(Resources.MultipleClientsConnectedMessage, RealTimeDataServer.ClientCount);
                }

                caption.Append(" / ");
                caption.Append(clientsConnectedCaption);
            }

            caption.Append(" / ");
            switch (CSMPreference.GetAutomaticComputatingType())
            {
                case eMAutomaticComputingType.M_acQuotation:
                case eMAutomaticComputingType.M_acLast:
                case eMAutomaticComputingType.M_acNothing:
                    caption.Append(Resources.DataRefreshModeManual);
                    break;

                case eMAutomaticComputingType.M_acPortfolioWithoutPNL:
                case eMAutomaticComputingType.M_acPortfolioOnlyPNL:
                case eMAutomaticComputingType.M_acFolio:
                    caption.Append(Resources.DataRefreshModeAutomatic);
                    break;
            }

            if (!RealTimeDataServer.IsRunning)
            {
                caption.Append(" / ");
                caption.Append(Resources.DataServerStopped);
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

        private void OnStatusChanged(object sender, EventArgs e)
        {
            _context.InvokeAsync(() =>
            {
                UpdateCaption();

            }, DispatcherPriority.ApplicationIdle);
        }

        private void AutomaticComputingChanged(object sender, EventArgs e)
        {
            _context.InvokeAsync(() =>
            {
                UpdateCaption();

            }, DispatcherPriority.ApplicationIdle);
        }
    }
}
