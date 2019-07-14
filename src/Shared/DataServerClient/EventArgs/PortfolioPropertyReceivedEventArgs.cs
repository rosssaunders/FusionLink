//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.Client
{
    public class PortfolioPropertyReceivedEventArgs : EventArgs
    {
        public int PortfolioId { get; private set; }

        public PortfolioProperty Property { get; private set; }

        public object Value { get; private set; }

        public PortfolioPropertyReceivedEventArgs(int portfolioId, PortfolioProperty property, object value)
        {
            this.PortfolioId = portfolioId;
            this.Property = property;
            this.Value = value;
        }
    }
}