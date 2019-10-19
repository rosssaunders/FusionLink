//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Runtime.ExceptionServices;

namespace RxdSolutions.FusionLink.Provider
{
    internal class IsRealTimeEnabledValue : SystemValue
    {
        [HandleProcessCorruptedStateExceptions]
        public override object GetValue()
        {
            try
            {
                if(Error is object)
                {
                    return Error.Message;
                }

                return sophis.globals.CSMApi.gApi().IsRTconnected();
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}