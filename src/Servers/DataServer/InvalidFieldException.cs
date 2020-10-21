//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink
{
    [Serializable]
    public class InvalidFieldException : Exception
    {
        public InvalidFieldException() { }

        public InvalidFieldException(string message) : base(message) { }

        public InvalidFieldException(string message, Exception inner) : base(message, inner) { }

        protected InvalidFieldException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}