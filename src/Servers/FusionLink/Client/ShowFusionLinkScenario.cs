//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using RxdSolutions.FusionLink.Properties;
using sophis.scenario;
using sophis.utils;

namespace RxdSolutions.FusionLink.Client
{
    public class ShowFusionLinkScenario : CSMScenario
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
            return Resources.ShowFusionLinkScenarioName;
        }

        public override void Run()
        {
            Main.CaptionBar.Show();

            base.Run();
        }
    }
}
