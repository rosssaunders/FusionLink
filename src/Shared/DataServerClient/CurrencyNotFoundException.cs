//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink.Client
{
    [Serializable]
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException() { }

        public CurrencyNotFoundException(string message) : base(message) { }

        public CurrencyNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected CurrencyNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}