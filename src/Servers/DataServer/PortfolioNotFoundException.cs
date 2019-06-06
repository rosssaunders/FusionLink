//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink
{
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