﻿//  Copyright (c) RXD Solutions. All rights reserved.
using System;

namespace RxdSolutions.FusionLink.Client
{

    [Serializable]
    public class PortfolioNotLoadedException : Exception
    {
        public PortfolioNotLoadedException() { }

        public PortfolioNotLoadedException(string message) : base(message) { }

        public PortfolioNotLoadedException(string message, Exception inner) : base(message, inner) { }

        protected PortfolioNotLoadedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}