//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Runtime.ExceptionServices;
using RxdSolutions.FusionLink.Properties;
using RxdSolutions.FusionLink.Services;
using sophis.commodity;
using sophis.instrument;
using sophis.static_data;
using sophis.value;

namespace RxdSolutions.FusionLink.Provider
{
    internal class InstrumentPropertyValue : IDisposable
    {
        private readonly InstrumentService _instrumentService;

        public CSMInstrument Instrument { get; }

        public int InstrumentId { get; }

        public object Reference { get; }

        public string Property { get; }

        public Exception Error { get; set; }

        public InstrumentPropertyValue(int instrumentId, object reference, string property, InstrumentService instrumentService)
        {
            InstrumentId = instrumentId;
            Reference = reference;
            Property = property;
            _instrumentService = instrumentService;

            Instrument = CSMInstrument.GetInstance(instrumentId);
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

                if (Instrument is null)
                {
                    if(Reference is object)
                        return string.Format(Resources.InstrumentNotFoundMessage, Reference);
                    else
                        return string.Format(Resources.InstrumentNotFoundMessage, InstrumentId);
                }

                object GetValueFromLookup()
                {
                    switch (Instrument.GetType_API())
                    {
                        case 'A':
                            CSMEquity a = Instrument;
                            return _instrumentService.GetValue(a, Property);

                        case 'B': //Caps and Floors
                            CSMCapFloor b = Instrument;
                            return _instrumentService.GetValue(b, Property);

                        case 'C': //Commissions
                            CSMCommission c = Instrument;
                            return _instrumentService.GetValue(c, Property);

                        case 'D': //CSMOption
                            CSMOption d = Instrument;
                            return _instrumentService.GetValue(d, Property);

                        case 'E': //Forex
                            CSMForexSpot e = Instrument;
                            return _instrumentService.GetValue(e, Property);

                        case 'K': // 'Non Deliverable Forward Forex'
                            CSMNonDeliverableForexForward k = Instrument;
                            return _instrumentService.GetValue(k, Property);

                        case 'X': // 'Forward Forex'
                            CSMForexFuture x = Instrument;
                            return _instrumentService.GetValue(x, Property);

                        case 'F': //CSMFuture
                            CSMFuture f = Instrument;
                            return _instrumentService.GetValue(f, Property);

                        case 'G': // 'Contracts for difference'
                            CSMContractForDifference g = Instrument;
                            return _instrumentService.GetValue(g, Property);

                        case 'H': // 'Issuers'
                            CSMIssuer h = Instrument;
                            return _instrumentService.GetValue(h, Property);

                        case 'I': // 'Indexes and Baskets'
                            CSMInstrument i = Instrument;
                            return _instrumentService.GetValue(i, Property);

                        case 'L': // 'Repos'
                            CSMLoanAndRepo l = Instrument;
                            return _instrumentService.GetValue(l, Property);

                        case 'N': // 'Bond Baskets' / Packages
                            CSMBondBasket n = Instrument;
                            return _instrumentService.GetValue(n, Property);

                        case 'M': // 'Listed Options'
                            CSMOption m = Instrument;
                            return _instrumentService.GetValue(m, Property);

                        case 'O': // 'Bonds'
                            CSMBond o = Instrument;
                            return _instrumentService.GetValue(o, Property);

                        case 'P': // 'Loans on Stock'
                            CSMLoanAndRepo p = Instrument;
                            return _instrumentService.GetValue(p, Property);

                        case 'Q': // 'Commodity'
                            CSMCommodity q = Instrument;
                            return _instrumentService.GetValue(q, Property);

                        case 'R': // 'Interest Rates'   
                            CSMInterestRate r = Instrument;
                            return _instrumentService.GetValue(r, Property);

                        case 'S': // 'Swaps'
                            CSMSwap s = Instrument;
                            return _instrumentService.GetValue(s, Property);

                        case 'T': // 'Debt Instruments'
                            CSMDebtInstrument t = Instrument;
                            return _instrumentService.GetValue(t, Property);

                        case 'U': // 'Commodity Indexes'
                            CSMCommodityBasket u = Instrument;
                            return _instrumentService.GetValue(u, Property);

                        case 'W': // 'Swapped Options'
                            CSMOption w = Instrument;
                            return _instrumentService.GetValue(w, Property);

                        case 'Z': // 'Fund'
                            CSMAmFund z = Instrument;
                            return _instrumentService.GetValue(z, Property);

                        default:
                            return "Unknown Instrument Type";
                    }
                }

                var value = GetValueFromLookup();

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

                return value.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
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
                    Instrument?.Dispose();
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