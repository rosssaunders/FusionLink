//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Data;
using System.Runtime.ExceptionServices;
using RxdSolutions.FusionLink.Model;
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
                            var a = new Equity(InstrumentId);
                            return _instrumentService.GetValue(a, Property);

                        case 'B': //Caps and Floors
                            var b = new CapFloor(InstrumentId);
                            return _instrumentService.GetValue(b, Property);

                        case 'C': //Commissions
                            var c = new Commission(InstrumentId);
                            return _instrumentService.GetValue(c, Property);

                        case 'D': //CSMOption
                            var d = new Option(InstrumentId);
                            return _instrumentService.GetValue(d, Property);

                        case 'E': //Forex
                            var e = new ForexSpot(InstrumentId);
                            return _instrumentService.GetValue(e, Property);

                        case 'K': // 'Non Deliverable Forward Forex'
                            var k = new NonDeliverableForexForward(InstrumentId);
                            return _instrumentService.GetValue(k, Property);

                        case 'X': // 'Forward Forex'
                            var x = new ForexFuture(InstrumentId);
                            return _instrumentService.GetValue(x, Property);

                        case 'F': //CSMFuture
                            var f = new Future(InstrumentId);
                            return _instrumentService.GetValue(f, Property);

                        case 'G': // 'Contracts for difference'
                            var g = new ContractForDifference(InstrumentId);
                            return _instrumentService.GetValue(g, Property);

                        case 'H': // 'Issuers'
                            var h = new Issuer(InstrumentId);
                            return _instrumentService.GetValue(h, Property);

                        case 'I': // 'Indexes and Baskets'
                            var i = new Index(InstrumentId);
                            return _instrumentService.GetValue(i, Property);

                        case 'L': // 'Repos'
                            var l = new LoanAndRepo(InstrumentId);
                            return _instrumentService.GetValue(l, Property);

                        case 'N': // 'Bond Baskets' / Packages
                            var n = new BondBasket(InstrumentId);
                            return _instrumentService.GetValue(n, Property);

                        case 'M': // 'Listed Options'
                            var m = new ListedOption(InstrumentId);
                            return _instrumentService.GetValue(m, Property);

                        case 'O': // 'Bonds'
                            var o = new Bond(InstrumentId);
                            return _instrumentService.GetValue(o, Property);

                        case 'P': // 'Loans on Stock'
                            var p = new LoanAndRepo(InstrumentId);
                            return _instrumentService.GetValue(p, Property);

                        case 'Q': // 'Commodity'
                            var q = new Commodity(InstrumentId);
                            return _instrumentService.GetValue(q, Property);

                        case 'R': // 'Interest Rates'   
                            var r = new InterestRate(InstrumentId);
                            return _instrumentService.GetValue(r, Property);

                        case 'S': // 'Swaps'
                            var s = new Swap(InstrumentId);
                            return _instrumentService.GetValue(s, Property);

                        case 'T': // 'Debt Instruments'
                            var t = new DebtInstrument(InstrumentId);
                            return _instrumentService.GetValue(t, Property);

                        case 'U': // 'Commodity Indexes'
                            var u = new CommodityBasket(InstrumentId);
                            return _instrumentService.GetValue(u, Property);

                        case 'W': // 'Swapped Options'
                            var w = new Option(InstrumentId);
                            return _instrumentService.GetValue(w, Property);

                        case 'Z': // 'Fund'
                            var z = new Fund(InstrumentId);
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

                if (value is DataTable)
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