//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using RxdSolutions.FusionLink.Helpers;
using RxdSolutions.FusionLink.Interface;
using sophis.gui;
using sophis.instrument;
using sophis.static_data;

namespace RxdSolutions.FusionLink.Services
{
    public class PriceService
    {
        public List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            using var instrument = CSMInstrument.GetInstance(instrumentId);

            if (instrument is null)
            {
                throw new InstrumentNotFoundException();
            }

            int refCount = 0;
            var history = instrument.NEW_HistoryList(DataTypeExtensions.ConvertDateTime(startDate), DataTypeExtensions.ConvertDateTime(endDate), ref refCount, null);
            
            return TranslatePriceHistory(history, refCount);
        }

        public List<PriceHistory> GetCurrencyPriceHistory(int currencyId, DateTime startDate, DateTime endDate)
        {
            using var currency = CSMCurrency.GetCSRCurrency(currencyId);

            if (currency is null)
            {
                throw new CurrencyNotFoundException();
            }

            int refCount = 0;
            var history = currency.NEW_HistoryList(DataTypeExtensions.ConvertDateTime(startDate), DataTypeExtensions.ConvertDateTime(endDate), ref refCount, null);

            return TranslatePriceHistory(history, refCount);
        }

        private List<PriceHistory> TranslatePriceHistory(SSMHistory history, int refCount)
        {
            var results = new List<PriceHistory>();

            for (int i = 0; i < refCount; i++)
            {
                using SSMHistory price = history.GetNthElement(i);

                if (price.day != DataTypeExtensions.SophisNull)
                {
                    var ph = new PriceHistory()
                    {
                        Ask = (double?)DataTypeExtensions.ConvertDouble(price.ask, eMNullValueType.M_nvUndefined),
                        Bid = (double?)DataTypeExtensions.ConvertDouble(price.bid, eMNullValueType.M_nvUndefined),
                        First = (double?)DataTypeExtensions.ConvertDouble(price.first, eMNullValueType.M_nvUndefined),
                        High = (double?)DataTypeExtensions.ConvertDouble(price.high, eMNullValueType.M_nvUndefined),
                        Low = (double?)DataTypeExtensions.ConvertDouble(price.low, eMNullValueType.M_nvUndefined),
                        Last = (double?)DataTypeExtensions.ConvertDouble(price.last, eMNullValueType.M_nvUndefined),
                        Theoretical = (double?)DataTypeExtensions.ConvertDouble(price.theorical, eMNullValueType.M_nvUndefined),
                        Volume = (double?)DataTypeExtensions.ConvertDouble(price.volume, eMNullValueType.M_nvUndefined),
                        Date = (DateTime)price.day.ToDateTime()
                    };

                    results.Add(ph);
                }
            }

            return results;
        }
    }
}
