//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink
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