//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink
{
    [Serializable]
    public class CurveFamilyFoundException : Exception
    {
        public CurveFamilyFoundException() { }

        public CurveFamilyFoundException(string message) : base(message) { }

        public CurveFamilyFoundException(string message, Exception inner) : base(message, inner) { }

        protected CurveFamilyFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}