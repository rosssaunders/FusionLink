//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Properties;
using sophis.scenario;
using sophis.utils;
using Sophis.Windows.Integration;

namespace RxdSolutions.FusionLink.Client
{
    public class ShowDashboardScenario : CSMScenario
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
            return Resources.ShowDashboard;
        }

        public override void Run()
        {
            var wndKey = new WindowKey(3049, 0, 0);
            IntPtr activeWindow = WPFAdapter.Instance.GetActiveWindow(wndKey);
            if (IntPtr.Zero != activeWindow)
            {
                WPFAdapter.Instance.ActivateWindow(activeWindow);
            }
            else
            {
                var fwkElement = new DiagnosticsView(new DiagnosticsViewModel(Main.DataServer));
                WPFAdapter.Instance.OpenWindow(fwkElement, Resources.DashboardWindowCaption, wndKey, false);
            }

            base.Run();
        }
    }
}
