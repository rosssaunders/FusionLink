//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        private AvailableConnections _availableConnections;

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

            return _availableConnections.AvailableEndpoints.Count > 0;
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
                return BuildMessage(Resources.SearchingForServersMessage).ToString();
            }

            if(_availableConnections.AvailableEndpoints.Count == 0)
            {
                return BuildMessage(Resources.NoEndPointsAvailableMessage).ToString();
            }

            var connections = GetAliveEndPoints().Select(endPoint =>
            {
                string GetCurrentFlag()
                {
                    if(_client.Via.Equals(endPoint))
                    {
                        return " (Current)";
                    }

                    return string.Empty;
                }

                var cb = new ConnectionBuilder(endPoint);

                var processId = cb.GetProcessId();
                var connectionName = cb.GetConnectionName() + GetCurrentFlag();

                var item = (Id: processId, MenuItem: new XElement(_customUINS + "button",
                        new XAttribute("id", $"Process{processId}Button"),
                        new XAttribute("label", connectionName),
                        new XAttribute("tag", endPoint.ToString()),
                        new XAttribute("onAction", "OnConnect"),
                        new XAttribute("imageMso", "ServerConnection")));

                return item;
            }
            )
            .OrderBy(x => x.Id)
            .Select(x => x.MenuItem);

            var menu = BuildPopupMenu(connections);

            return menu.ToString();
        }

        private IEnumerable<Uri> GetAliveEndPoints()
        {
            return _availableConnections.AvailableEndpoints.Select(x => x.Via);
        }
               
        private XElement BuildMessage(string message)
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

            _connectionMonitor.SetConnection(new Uri(control.Tag));
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
