//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using RxdSolutions.FusionLink.Properties;
using sophis.scenario;
using sophis.utils;

namespace RxdSolutions.FusionLink.Client
{
    public class StartStopDataServerScenario : CSMScenario
    {
        private DataServer dataServer { get { return Main.DataServer; } }

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
            return Resources.StartButtonText;
        }

        public override void Run()
        {
            if (dataServer.IsRunning)
                dataServer.Stop();
            else
                dataServer.Start();

            base.Run();
        }
    }
}
