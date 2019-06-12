//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Interface;
using RxdSolutions.FusionLink.Properties;
using sophis.portfolio;
using sophis.utils;

namespace RxdSolutions.FusionLink
{
    internal class PortfolioPropertyValue : IDisposable
    {
        public CSMPortfolio Portfolio { get; }

        public int FolioId { get; }

        public PortfolioProperty Property { get; }

        public PortfolioPropertyValue(int folioId, PortfolioProperty property)
        {
            FolioId = folioId;
            Property = property;
            Portfolio = CSMPortfolio.GetCSRPortfolio(folioId);
        }

        public object GetValue()
        {
            if (Portfolio is null)
            {
                return string.Format(Resources.PortfolioNotFoundMessage, FolioId);
            }

            if (!Portfolio.IsLoaded())
            {
                return string.Format(Resources.PortfolioNotLoadedMessage, FolioId);
            }

            switch (Property)
            {
                case PortfolioProperty.Parent:
                    return Portfolio.GetParentCode();

                case PortfolioProperty.FullPath:
                    using(var fullName = new CMString())
                    {
                        Portfolio.GetFullName(fullName);
                        return fullName.StringValue;
                    }
                    
                default:
                    return $"Unknown Portfolio Property '{Property}'";
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Portfolio?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}