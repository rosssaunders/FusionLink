using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using ExcelDna.Integration;
using ExcelDna.Integration.CustomUI;

namespace RxdSolutions.FusionLink.ExcelClient
{
    [ComVisible(true)]
    public class CustomRibbon : ExcelRibbon
    {
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
            XNamespace customUINS = "http://schemas.microsoft.com/office/2006/01/customui";

            var connections = AddIn.ConnectionMonitor.AvailableEndpoints.Select(x => 
                {
                    var id = ConnectionHelper.GetConnectionId(x.Uri);

                    var process = Process.GetProcessById(id);
                    var title = process.MainWindowTitle;
                    if (string.IsNullOrWhiteSpace(title))
                        title = x.Uri.ToString();

                    return (process.Id, new XElement(customUINS + "button",
                            new XAttribute("id", $"Process{id}Button"),
                            new XAttribute("label", $"{id} - {title}"),
                            new XAttribute("tag", x.Uri.ToString()),
                            new XAttribute("onAction", "OnConnect"),
                            new XAttribute("imageMso", "ServerConnection")));
                }
            )
            .OrderBy(x => x.Item1)
            .Select(x => x.Item2);

            var menu =
                new XElement(customUINS + "menu",
                    new XAttribute("itemSize", "large"),
                    connections
                );

            return menu.ToString();
        }

        public void OnConnect(IRibbonControl control)
        {
            AddIn.ConnectionMonitor.SetConnection(new Uri(control.Tag));
        }

        public void OnRefresh(IRibbonControl control)
        {
            var app = ExcelDnaUtil.Application as Microsoft.Office.Interop.Excel.Application;
            app.StatusBar = "Searching for available FusionLink servers. Please wait...";

            AddIn.ConnectionMonitor.FindAvailableServices();
        }
    }
}
