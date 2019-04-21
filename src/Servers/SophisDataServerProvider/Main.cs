//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System.ServiceModel;
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

            }).ContinueWith(t => {
                UpdateCaption();
            }, uiThreadScheduler);
        }

        private static void CreateDataServerFromConfig(SophisDataServiceProvider dataServiceProvider)
        {
            _dataServer = new DataServer(dataServiceProvider);

            int refreshRate = 0;
            string defaultMessage = "";
            CSMConfigurationFile.getEntryValue("FusionLink", "RefreshRate", ref refreshRate, 2000);
            CSMConfigurationFile.getEntryValue("FusionLink", "DefaultMessage", ref defaultMessage, "Getting data... please wait");

            _dataServer.ProviderPollingInterval = refreshRate;
            _dataServer.DefaultMessage = defaultMessage;
        }

        private void UpdateCaption()
        {
            var clientCount = _dataServer.Clients.Count;

            string clientsConnectedCaption = "";
            if (clientCount == 1)
            {
                clientsConnectedCaption = $" ({clientCount} client connected)";
            }
            else if(clientCount > 1)
            {
                clientsConnectedCaption = $" ({clientCount} clients connected)";
            }

            var processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            var dataServiceIdentifierCaption = $"Excel connection Id is {processId}";

            _captionBar.Text = $"{dataServiceIdentifierCaption} {clientsConnectedCaption}";
            _captionBar.Show();
        }

        private void OnClientConnectionChanged(object sender, ClientConnectionChangedEventArgs e)
        {
            _context.Send(x => 
            {
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
