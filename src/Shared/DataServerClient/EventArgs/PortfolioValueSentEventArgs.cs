//  Copyright (c) RXD Solutions. All rights reserved.


namespace RxdSolutions.FusionLink.Client
{
    public class PortfolioValueReceivedEventArgs : ValueSentEventArgs
    {
        public int PortfolioId { get; private set; }

        public PortfolioValueReceivedEventArgs(int portfolioId, string column, object value)
            : base(column, value)
        {
            this.PortfolioId = portfolioId;
        }
    }
}