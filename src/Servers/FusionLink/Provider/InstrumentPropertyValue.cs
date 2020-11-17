//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Data;
using System.Runtime.ExceptionServices;
using RxdSolutions.FusionLink.Model;
using RxdSolutions.FusionLink.Properties;
using RxdSolutions.FusionLink.Services;
using sophis.commodity;
using sophis.instrument;
using sophis.value;

namespace RxdSolutions.FusionLink.Provider
{
    internal class InstrumentPropertyValue : IDisposable
    {
        private readonly InstrumentService _instrumentService;

        private bool? _isMarketData = null;

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

        private Instrument GetInstrumentFromCode()
        {
            switch (Instrument.GetType_API())
            {
                case 'A':
                    return new Equity(InstrumentId);
                    
                case 'B': //Caps and Floors
                    return new CapFloor(InstrumentId);
                    
                case 'C': //Commissions
                    return new Commission(InstrumentId);
                    
                case 'D': //CSMOption
                    return new Option(InstrumentId);
                    
                case 'E': //Forex
                    return new ForexSpot(InstrumentId);
                    
                case 'K': // 'Non Deliverable Forward Forex'
                    return new NonDeliverableForexForward(InstrumentId);
                    
                case 'X': // 'Forward Forex'
                    return new ForexFuture(InstrumentId);
                    
                case 'F': //CSMFuture
                    return new Future(InstrumentId);
                    
                case 'G': // 'Contracts for difference'
                    return new ContractForDifference(InstrumentId);
                    
                case 'H': // 'Issuers'
                    return new Issuer(InstrumentId);
                    
                case 'I': // 'Indexes and Baskets'
                    return new Index(InstrumentId);
                    
                case 'L': // 'Repos'
                    return new LoanAndRepo(InstrumentId);
                    
                case 'N': // 'Bond Baskets' / Packages
                    return new BondBasket(InstrumentId);
                    
                case 'M': // 'Listed Options'
                    return new ListedOption(InstrumentId);
                    
                case 'O': // 'Bonds'
                    return new Bond(InstrumentId);
                    
                case 'P': // 'Loans on Stock'
                    return new LoanAndRepo(InstrumentId);
                    
                case 'Q': // 'Commodity'
                    return new Commodity(InstrumentId);
                    
                case 'R': // 'Interest Rates'   
                    return new InterestRate(InstrumentId);
                    
                case 'S': // 'Swaps'
                    return new Swap(InstrumentId);
                    
                case 'T': // 'Debt Instruments'
                    return new DebtInstrument(InstrumentId);
                    
                case 'U': // 'Commodity Indexes'
                    return new CommodityBasket(InstrumentId);
                    
                case 'W': // 'Swapped Options'
                    return new Option(InstrumentId);
                    
                case 'Z': // 'Fund'
                    return new Fund(InstrumentId);
                    
                default:
                    return null;
            }
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
                    var instrument = GetInstrumentFromCode();

                    if(instrument == null)
                    {
                        return "Unknown Instrument Type";
                    }

                    return _instrumentService.GetValue(instrument, Property);
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

        [HandleProcessCorruptedStateExceptions]
        public bool IsMarketData()
        {
            if(!_isMarketData.HasValue)
            {
                bool GetIsMarketData()
                {
                    var instrument = GetInstrumentFromCode();

                    if(instrument == null)
                    {
                        return false;
                    }

                    return _instrumentService.IsMarketData(instrument, Property);
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