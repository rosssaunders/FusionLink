//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Runtime.ExceptionServices;

namespace RxdSolutions.FusionLink
{
    internal class IsRealTimeEnabledValue : SystemValue
    {
        [HandleProcessCorruptedStateExceptions]
        public override object GetValue()
        {
            try
            {
                return sophis.globals.CSMApi.gApi().IsRTconnected();
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}