//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using ExcelDna.Integration.CustomUI;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    [ComVisible(true)]
    public class CustomRibbon : ExcelRibbon
    {
        private XNamespace customUINS = "http://schemas.microsoft.com/office/2006/01/customui";

        public CustomRibbon()
        {
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
            if (AddIn.ConnectionMonitor.IsSearchingForEndPoints)
            {
                return BuildMessage(Resources.SearchingForServersMessage).ToString();
            }

            if(AddIn.ConnectionMonitor.AvailableEndpoints.Count == 0)
            {
                return BuildMessage(Resources.NoEndPointsAvailableMessage).ToString();
            }

            var connections = GetAliveEndPoints().Select(endPoint =>
            {
                var item = (Id: endPoint.Process.Id, MenuItem: new XElement(customUINS + "button",
                        new XAttribute("id", $"Process{endPoint.Process.Id}Button"),
                        new XAttribute("label", ConnectionHelper.GetConnectionName(endPoint.Uri)),
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
            foreach (var endPoint in AddIn.ConnectionMonitor.AvailableEndpoints)
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
                    yield return (endPoint.Uri, process);
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

            AddIn.ConnectionMonitor.SetConnection(new Uri(control.Tag));
        }

        public void OnRefresh(IRibbonControl control)
        {
            RefreshAvailableConnections();
        }

        public void OnCalculate(IRibbonControl control)
        {
            if(AddIn.Client.State == System.ServiceModel.CommunicationState.Opened)
                AddIn.Client.RequestCalculate();
        }

        public void OnLoadPositions(IRibbonControl control)
        {
            if (AddIn.Client.State == System.ServiceModel.CommunicationState.Opened)
                AddIn.Client.LoadPositions();
        }

        private static void RefreshAvailableConnections()
        {
            AddIn.ConnectionMonitor.FindAvailableServicesAsync();
        }
    }
}
