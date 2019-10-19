//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink.Provider
{
    internal abstract class SystemValue
    {
        public Exception Error { get; set; }

        public abstract object GetValue();
    }
}