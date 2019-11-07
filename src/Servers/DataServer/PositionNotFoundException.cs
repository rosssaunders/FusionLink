//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink
{
    [Serializable]
    public class PositionNotFoundException : Exception
    {
        public PositionNotFoundException() { }

        public PositionNotFoundException(string message) : base(message) { }

        public PositionNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected PositionNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}