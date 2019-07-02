//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink.Provider
{
    internal abstract class SystemValue
    {
        public Exception Error { get; set; }

        public abstract object GetValue();
    }
}