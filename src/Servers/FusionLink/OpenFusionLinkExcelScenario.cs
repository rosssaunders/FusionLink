//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.IO;
using System.Windows.Forms;
using RxdSolutions.FusionLink.Properties;
using sophis.misc;
using sophis.scenario;
using sophis.utils;

namespace RxdSolutions.FusionLink
{
    public class OpenFusionLinkExcelScenario : CSMScenario
    {
        public override eMProcessingType GetProcessingType()
        {
            return eMProcessingType.M_pUserPreference;
        }

        public override bool AlwaysEnabled()
        {
            return true;
        }

        public override CMString GetName()
        {
            return Resources.OpenFusionLinkExcel;
        }

        public override void Run()
        {
            string defaultLocation = "";
            CSMConfigurationFile.getEntryValue("FusionLink", "XllLocation", ref defaultLocation, "");

            if(!string.IsNullOrWhiteSpace(defaultLocation))
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
            
            base.Run();
        }
    }
}
