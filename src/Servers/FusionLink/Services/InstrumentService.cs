//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using RxdSolutions.FusionLink.Helpers;
using RxdSolutions.FusionLink.Interface;
using sophis.gui;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Services
{
    public class InstrumentService
    {
        public List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            var results = new List<PriceHistory>();

            using (var instrument = CSMInstrument.GetInstance(instrumentId))
            {
                if (instrument is null)
                {
                    throw new InstrumentNotFoundException();
                }

                int refCount = 0;
                var history = instrument.NEW_HistoryList(DataTypeExtensions.ConvertDateTime(startDate), DataTypeExtensions.ConvertDateTime(endDate), ref refCount, null);

                for (var i = 0; i < refCount; i++)
                {
                    using (SSMHistory price = history.GetNthElement(i))
                    {
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
                                Date = (DateTime)price.day.GetDateTime()
                            };

                            results.Add(ph);
                        }
                    }
                }

                return results;
            }
        }
    }
}
