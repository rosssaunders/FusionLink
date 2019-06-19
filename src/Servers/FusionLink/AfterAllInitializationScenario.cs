//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.scenario;
using sophis.utils;

namespace RxdSolutions.FusionLink
{
    public class AfterAllInitializationScenario : CSMScenario
    {
        public event EventHandler AfterAllInitialization;

        public override eMProcessingType GetProcessingType()
        {
            return eMProcessingType.M_pAfterAllInitialisation;
        }

        public override bool AlwaysEnabled()
        {
            return true;
        }

        public override bool InTheAnalysisMenu()
        {
            return false;
        }

        public override CMString GetName()
        {
            return "AfterAllInitializationScenario";
        }

        public override void Run()
        {
            AfterAllInitialization?.Invoke(this, new EventArgs());
        }
    }
}
