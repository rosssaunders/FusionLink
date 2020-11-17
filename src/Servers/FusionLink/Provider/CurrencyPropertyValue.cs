//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Data;
using System.Runtime.ExceptionServices;
using RxdSolutions.FusionLink.Model;
using RxdSolutions.FusionLink.Properties;
using RxdSolutions.FusionLink.Services;
using sophis.static_data;

namespace RxdSolutions.FusionLink.Provider
{
    internal class CurrencyPropertyValue : IDisposable
    {
        private readonly CurrencyService _currencyService;
        
        private bool? _isMarketData = null;

        public CSMCurrency Currency { get; }

        private Currency CurrencyObject { get; }

        public int CurrencyId { get; }

        public object Reference { get; }

        public string Property { get; }

        public Exception Error { get; set; }

        public CurrencyPropertyValue(int currencyId, object reference, string property, CurrencyService currencyService)
        {
            CurrencyId = currencyId;
            Reference = reference;
            Property = property;
            _currencyService = currencyService;

            Currency = CSMCurrency.GetCSRCurrency(currencyId);
            CurrencyObject = new Currency(CurrencyId);
        }

        [HandleProcessCorruptedStateExceptions]
        public object GetValue()
        {
            try
            {
                if (Error is object)
                {
                    return Error.Message;
                }

                if (Currency is null)
                {
                    if (Reference is object)
                        return string.Format(Resources.InstrumentNotFoundMessage, Reference);
                    else
                        return string.Format(Resources.InstrumentNotFoundMessage, CurrencyId);
                }

                var value = _currencyService.GetValue(CurrencyObject, Property);

                if (value is null)
                    return null;

                if (value is string)
                    return value;

                if (value is DateTime)
                    return value;

                if (value is int)
                    return value;

                if (value is double)
                    return value;

                if (value is bool)
                    return value;

                if (value is DataTable)
                    return value;

                return value.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public bool IsMarketData()
        {
            if (!_isMarketData.HasValue)
            {
                bool GetIsMarketData()
                {
                    var instrument = CurrencyObject;

                    if (instrument == null)
                    {
                        return false;
                    }

                    return _currencyService.IsMarketData(instrument, Property);
                }

                _isMarketData = GetIsMarketData();
            }

            return _isMarketData.Value;
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Currency?.Dispose();
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