//  Copyright (c) RXD Solutions. All rights reserved.
using System;

namespace RxdSolutions.FusionLink.Client
{
    [Serializable]
    public class PortfolioNotFoundException : Exception
    {
        public PortfolioNotFoundException() { }

        public PortfolioNotFoundException(string message) : base(message) { }

        public PortfolioNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected PortfolioNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}