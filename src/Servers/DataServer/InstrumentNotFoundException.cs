//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink
{
    [Serializable]
    public class InstrumentNotFoundException : Exception
    {
        public InstrumentNotFoundException() { }

        public InstrumentNotFoundException(string message) : base(message) { }

        public InstrumentNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected InstrumentNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}