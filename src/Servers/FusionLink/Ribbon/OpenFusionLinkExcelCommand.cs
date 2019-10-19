//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using RxdSolutions.FusionLink.Properties;
using sophis.misc;
using Sophis.Windows;
using Sophis.Windows.Ribbon;

namespace RxdSolutions.FusionLink.Ribbon
{
    public class OpenFusionLinkExcelCommand : RibbonCommand
    {
        protected OpenFusionLinkExcelCommand() : base(Resources.CommandOpenFusionLinkExcelText, 
                                                      Resources.CommandOpenFusionLinkExcelName, 
                                                      typeof(OpenFusionLinkExcelCommand), 
                                                      "table_ok")
        {
        }

        public static OpenFusionLinkExcelCommand Register()
        {
            var cmd = new OpenFusionLinkExcelCommand();

            RibbonCommands.RegisterDynamic(cmd);
            SophisApplication.MainCommandTarget.CommandBindings.Add(new CommandBinding(cmd, new ExecutedRoutedEventHandler(Handler), new CanExecuteRoutedEventHandler(CanExecuteHandler)));

            return cmd;
        }

        private static void CanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private static void Handler(object sender, ExecutedRoutedEventArgs e)
        {
            string defaultLocation = "";
            CSMConfigurationFile.getEntryValue("FusionLink", "XllLocation", ref defaultLocation, "");

            if (!string.IsNullOrWhiteSpace(defaultLocation))
            {
                var location = Environment.ExpandEnvironmentVariables(Path.Combine(defaultLocation, "FusionLink-AddIn.xll"));

                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "excel.exe",
                    Arguments = $"\"\"\"{location}\"\"\""
                };

                System.Diagnostics.Process.Start(psi);
            }
            else
            {
                MessageBox.Show("No FusionLink location specified in the configuration file");
            }
        }
    }
}
