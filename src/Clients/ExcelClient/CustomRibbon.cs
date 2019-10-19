//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private XNamespace customUINS = "http://schemas.microsoft.com/office/2006/01/customui";
        private static IRibbonUI _ribbonUi;

        private ConnectionMonitor _connectionMonitor;
        private DataServiceClient _client;
        private Microsoft.Office.Interop.Excel.Application _application;
        private AvailableConnections _availableConnections;

        private static HashSet<string> _processNames = new HashSet<string>() { "sophisvalue.exe", "sophisrisque.exe" };

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
                    if(_client.Connection.Uri.Equals(endPoint.Uri))
                    {
                        return " (Current)";
                    }

                    return string.Empty;
                }

                var item = (Id: endPoint.Process.Id, MenuItem: new XElement(customUINS + "button",
                        new XAttribute("id", $"Process{endPoint.Process.Id}Button"),
                        new XAttribute("label", ConnectionHelper.GetConnectionName(endPoint.Uri) + GetCurrentFlag()),
                        new XAttribute("tag", endPoint.Uri.ToString()),
                        new XAttribute("onAction", "OnConnect"),
                        new XAttribute("imageMso", "ServerConnection")));

                endPoint.Process.Dispose();

                return item;
            }
            )
            .OrderBy(x => x.Id)
            .Select(x => x.MenuItem);

            var menu = BuildPopupMenu(connections);

            return menu.ToString();
        }

        private IEnumerable<(Uri Uri, Process Process)> GetAliveEndPoints()
        {
            foreach (var endPoint in _availableConnections.AvailableEndpoints)
            {
                var id = ConnectionHelper.GetConnectionId(endPoint.Uri);

                Process process = null;
                try
                {
                    process = Process.GetProcessById(id);
                }
                catch
                {
                    process = null; //Process must have exited
                }

                if (process != null)
                {
                    if (_processNames.Contains(process.ProcessName.ToLower()))
                    {
                        yield return (endPoint.Uri, process);
                    }
                }
            };
        }
               
        private XElement BuildMessage(string message)
        {
            var loadingMessageMenu = BuildPopupMenu(
                    new[] {
                        new XElement(customUINS + "button",
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
                new XElement(customUINS + "menu",
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
                    ExcelStatusBarHelperAsync.SetStatusBarWithResetDelay(Resources.NoEndPointsAvailableMessage, 5);
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
                    ExcelStatusBarHelperAsync.SetStatusBarWithResetDelay(Resources.ComputeRequestedMessage, 3);
                }
            }
            catch(Exception ex)
            {
                ExcelStatusBarHelperAsync.SetStatusBarWithResetDelay(ex.Message, 5);
            }
        }

        public void OnLoadPositions(IRibbonControl control)
        {
            try
            {
                if (_connectionMonitor.IsConnected)
                {
                    _client.LoadPositions();
                    ExcelStatusBarHelperAsync.SetStatusBarWithResetDelay(Resources.LoadPortfoliosRequestedMessage, 3);
                }
            }
            catch(Exception ex)
            {
                ExcelStatusBarHelperAsync.SetStatusBarWithResetDelay(ex.Message, 5);
            }
        }

        private Task RefreshAvailableConnections()
        {
            return _availableConnections.FindAvailableServicesAsync();
        }
    }
}
