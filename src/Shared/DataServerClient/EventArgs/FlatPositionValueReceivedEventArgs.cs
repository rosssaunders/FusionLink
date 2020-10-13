//  Copyright (c) RXD Solutions. All rights reserved.

namespace RxdSolutions.FusionLink.Client
{
    public class FlatPositionValueReceivedEventArgs : ValueSentEventArgs
    {
        public int PortfolioId { get; private set; }

        public int InstrumentId { get; private set; }

        public FlatPositionValueReceivedEventArgs(int portfolioId, int instrumentId, string column, object value)
             : base(column, value)

        {
            this.InstrumentId = instrumentId;
            this.PortfolioId = portfolioId;
        }
    }
}