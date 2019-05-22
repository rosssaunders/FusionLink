//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.scenario;

namespace RxdSolutions.FusionLink
{
    public class SophisLoadedScenario : CSMScenario
    {
        public static event EventHandler OnAfterAllInitialisation;

        public override eMProcessingType GetProcessingType()
        {
            return eMProcessingType.M_pAfterAllInitialisation;
        }

        public override bool AvailableForScenarioList()
        {
            return false;
        }

        public override bool AvailableForReport()
        {
            return false;
        }

        public override void Run()
        {
            OnAfterAllInitialisation?.Invoke(this, new EventArgs());

            base.Run();
        }
    }
}
