//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

namespace RxdSolutions.FusionLink
{
    internal class IsRealTimeEnabledValue : SystemValue
    {
        public override object GetValue()
        {
            return sophis.globals.CSMApi.gApi().IsRTconnected();
        }
    }
}