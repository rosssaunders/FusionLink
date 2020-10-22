//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ExcelClientUI;
using ExcelDna.Integration;
using ExcelDna.Integration.CustomUI;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    [ComVisible(true)]
    public class CustomRibbon : ExcelRibbon
    {
        private readonly XNamespace _customUINS = "http://schemas.microsoft.com/office/2006/01/customui";
        private static IRibbonUI _ribbonUi;
        private Microsoft.Office.Interop.Excel.Application _application;

        private ConnectionMonitor _connectionMonitor;
        private DataServiceClient _client;
        private ServerConnectionMonitor _availableConnections;

        public CustomRibbon()
        {
        }

        public void Ribbon_Load(IRibbonUI sender)
        {
            _ribbonUi = sender;

            _connectionMonitor = AddIn.ConnectionMonitor;
            _availableConnections = AddIn.AvailableConnections;

            _client = AddIn.Client;
            _application = ExcelDnaUtil.Application as Microsoft.Office.Interop.Excel.Application;

            _client.OnConnectionStatusChanged += (s,e) => Refresh();
            _connectionMonitor.OnConnectionFailed += (s, e) => Refresh();
            _connectionMonitor.OnConnectionSuccess += (s, e) => Refresh();
        }

        public bool OnRefreshEnabled(IRibbonControl control)
        {
            if (_client.IsConnecting)
                return false;

            return !_availableConnections.IsSearchingForEndPoints;
        }

        public bool OnConnectionsEnabled(IRibbonControl control)
        {
            if (_availableConnections.IsSearchingForEndPoints)
                return false;

            if (_client.IsConnecting)
                return false;

            return true;
        }

        public bool OnConnectionActionEnabled(IRibbonControl control)
        {
            return _connectionMonitor.IsConnected;
        }

        public static void Refresh()
        {
            try
            {
                _ribbonUi?.Invalidate();
            }
            catch
            {
                //Sink
            }
        }

        public override string GetCustomUI(string ribbonId)
        {
            var ribbonXml = GetCustomRibbonXML();
            return ribbonXml;
        }

        private string GetCustomRibbonXML()
        {
            var thisAssembly = typeof(CustomRibbon).Assembly;
            var resourceName = typeof(CustomRibbon).Namespace + ".CustomRibbon.xml";

            using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var ribbonXml = reader.ReadToEnd();

                if (ribbonXml == null)
                {
                    throw new MissingManifestResourceException(resourceName);
                }

                return ribbonXml;
            }
        }

        public string OnGetContent(IRibbonControl control)
        {
            if (_availableConnections.IsSearchingForEndPoints)
            {
                return BuildMessageButton(Resources.SearchingForServersMessage).ToString();
            }

            var items = new List<XElement>();

            var connections = GetAliveEndPoints().Select(endPoint =>
            {
                string GetCurrentFlag()
                {
                    if (_client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        if (_client.Via.Equals(endPoint.Via))
                        {
                            return " (Current)";
                        }
                    }

                    return string.Empty;
                }

                string GetConnectionFlag()
                {
                    if (endPoint.ConnectionType == ConnectionType.Manual)
                    {
                        return " [Manual]";
                    }

                    return string.Empty;
                }

                var cb = new ConnectionBuilder(endPoint.Via);

                var processId = cb.GetProcessId();
                var connectionName = cb.GetConnectionName() + GetConnectionFlag() + GetCurrentFlag();

                var item = (Id: processId,
                            MenuItem: new XElement(_customUINS + "button",
                                            new XAttribute("id", $"Process{processId}{endPoint.ConnectionType}Button"),
                                            new XAttribute("label", connectionName),
                                            new XAttribute("tag", endPoint.Via.ToString()),
                                            new XAttribute("onAction", "OnConnect"),
                                            new XAttribute("imageMso", "ServerConnection"))
                           );

                return item;
            }
            )
            .OrderBy(x => x.Id)
            .Select(x => x.MenuItem);

            items.AddRange(connections);

            var menu = BuildPopupMenu(items);

            //Seperator
            menu.Add(new XElement(_customUINS + "menuSeparator",
                            new XAttribute("id", $"seperator")));

            menu.Add(new XElement(_customUINS + "button",
                        new XAttribute("id", $"ManualConnection"),
                        new XAttribute("label", "Enter Connection"),
                        new XAttribute("tag", $"ManualConnection"),
                        new XAttribute("onAction", "OnConnect"),
                        new XAttribute("imageMso", "ServerConnection")));

            return menu.ToString();
        }

        private IEnumerable<EndPointAddressVia> GetAliveEndPoints()
        {
            return _availableConnections.AvailableEndpoints;
        }
               
        private XElement BuildMessageButton(string message)
        {
            var loadingMessageMenu = BuildPopupMenu(
                    new[] {
                        new XElement(_customUINS + "button",
                            new XAttribute("id", $"Process0"),
                            new XAttribute("label", message),
                            new XAttribute("tag", "0"),
                            new XAttribute("onAction", "OnConnect"),
                            new XAttribute("imageMso", "ServerConnection"))
                        }
                );

            return loadingMessageMenu;
        }

        private XElement BuildPopupMenu(IEnumerable<XElement> items)
        {
            var menu =
                new XElement(_customUINS + "menu",
                    new XAttribute("itemSize", "large"),
                    items
                );

            return menu;
        }

        public void OnConnect(IRibbonControl control)
        {
            if (control.Tag == "0")
                return;

            if(control.Tag == "ManualConnection")
            {
                var form = new ConnectionStringEntry
                {
                    StartPosition = FormStartPosition.CenterParent
                };

                var result = form.ShowDialog(NativeWindow.FromHandle((IntPtr)_application.Hwnd));

                if (result == DialogResult.OK)
                {
                    if(Uri.TryCreate(form.ConnectionTextBox.Text, UriKind.Absolute, out Uri uri))
                    {
                        try
                        {
                            _connectionMonitor.SetManualConnection(uri);
                        }
                        catch
                        {
                            MessageBox.Show($"Unable to connect to {uri}. Please try a different connection.");
                        }
                    }
                }

                return;
            }

            try
            {
                _connectionMonitor.SetConnection(new Uri(control.Tag));
            }
            catch
            {
                MessageBox.Show($"Unable to connect to {control.Tag}. Please try a different connection.");
            }
        }

        public void OnRefresh(IRibbonControl control)
        {
            var task = RefreshAvailableConnections();

            _application.StatusBar = Resources.SearchingForServersMessage;

            Refresh();

            task.ContinueWith(x =>
            {
                if(_availableConnections.AvailableEndpoints.Count == 0)
                {
                    _application.SetStatusBarWithResetDelay(Resources.NoEndPointsAvailableMessage, 5);
                }
                else
                {
                    _application.StatusBar = false;
                }

                Refresh();

            });
        }

        public void OnCalculate(IRibbonControl control)
        {
            try
            {
                if (_connectionMonitor.IsConnected)
                {
                    _client.RequestCalculate();
                    _application.SetStatusBarWithResetDelay(Resources.ComputeRequestedMessage, 3);
                }
            }
            catch(Exception ex)
            {
                _application.SetStatusBarWithResetDelay(ex.Message, 5);
            }
        }

        public void OnLoadPositions(IRibbonControl control)
        {
            try
            {
                if (_connectionMonitor.IsConnected)
                {
                    _client.LoadPositions();
                    _application.SetStatusBarWithResetDelay(Resources.LoadPortfoliosRequestedMessage, 3);
                }
            }
            catch(Exception ex)
            {
                _application.SetStatusBarWithResetDelay(ex.Message, 5);
            }
        }

        private Task RefreshAvailableConnections()
        {
            return _availableConnections.FindAvailableServicesAsync();
        }
    }
}
