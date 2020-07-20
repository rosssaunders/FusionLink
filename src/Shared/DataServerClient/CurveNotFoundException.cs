//  Copyright (c) RXD Solutions. All rights reserved.
using System;

namespace RxdSolutions.FusionLink.Client
{
    [Serializable]
    public class CurveNotFoundException : Exception
    {
        public CurveNotFoundException() { }

        public CurveNotFoundException(string message) : base(message) { }

        public CurveNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected CurveNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}