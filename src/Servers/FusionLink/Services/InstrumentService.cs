//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using RxdSolutions.FusionLink.Model;
using sophis.inflation;
using sophis.instrument;
using sophis.utils;

namespace RxdSolutions.FusionLink.Services
{
    public class InstrumentService
    {
        private Dictionary<Type, Dictionary<string, Func<object, object>>> _dispatchMapping = new Dictionary<Type, Dictionary<string, Func<object, object>>>();

        public InstrumentService()
        {
            //Setup the Dispatch mapping
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(Instrument).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x);

            foreach(var x in types)
            {
                var properties = new Dictionary<string, Func<object, object>>();
                _dispatchMapping.Add(x, properties);

                foreach(var property in x.GetProperties())
                {
                    properties.Add(property.Name, (obj) => property.GetValue(obj));
                }
            }
        }

        public object GetValue<T>(T instrument, string property) where T : Instrument
        {
            return _dispatchMapping[typeof(T)][property].Invoke(instrument);
        }

        //public object GetValueX<T>(T instrument, string property) where T : CSMInstrument
        //{
        //    return InstrumentFields.GetValue(instrument, property);

        //    switch (instrument)
        //    {
        //        case CSMIssuer h: return IssuerFieldLookup.ContainsKey(propertyUpper) ? IssuerFieldLookup[propertyUpper](h) : FB();
        //        case CSMNonDeliverableForexForward k: return NDFFieldLookup.ContainsKey(propertyUpper) ? NDFFieldLookup[propertyUpper](k) : FB();
        //        case CSMCapFloor b: return CapFloorFields.ContainsKey(propertyUpper) ? CapFloorFields[propertyUpper](b) : FB();
        //        case CSMCommission c: return CommissionFields.ContainsKey(propertyUpper) ? CommissionFields[propertyUpper](c) : FB();
        //        case CSMOption d: return OptionFields.ContainsKey(propertyUpper) ? OptionFields[propertyUpper](d) : FB();
        //        case CSMForexSpot e: return ForexFieldLookup.ContainsKey(propertyUpper) ? ForexFieldLookup[propertyUpper](e) : FB();
        //        case CSMFuture f: return FutureFieldLookup.ContainsKey(propertyUpper) ? FutureFieldLookup[propertyUpper](f) : FB();
        //        case CSMContractForDifference g: return CFDFieldLookup.ContainsKey(propertyUpper) ? CFDFieldLookup[propertyUpper](g) : FB();
        //        case CSMLoanAndRepo l: return LoanAndRepoFieldLookup.ContainsKey(propertyUpper) ? LoanAndRepoFieldLookup[propertyUpper](l) : FB();
        //        case CSMBondBasket n: return BondBasketFieldLookup.ContainsKey(propertyUpper) ? BondBasketFieldLookup[propertyUpper](n) : FB();
        //        case CSMBond o: return BondFieldLookup.ContainsKey(propertyUpper) ? BondFieldLookup[propertyUpper](o) : FB();
        //        case CSMCommodityBasket u: return CommodityBasketFields.ContainsKey(propertyUpper) ? CommodityBasketFields[propertyUpper](u) : FB();
        //        case CSMCommodity q: return CommodityFieldLookup.ContainsKey(propertyUpper) ? CommodityFieldLookup[propertyUpper](q) : FB();
        //        case CSMInterestRate r: return InterestRateFieldLookup.ContainsKey(propertyUpper) ? InterestRateFieldLookup[propertyUpper](r) : FB();
        //        case CSMSwap s: return SwapFields.ContainsKey(propertyUpper) ? SwapFields[propertyUpper](s) : FB();
        //        case CSMDebtInstrument t: return DebtInstrumentFields.ContainsKey(propertyUpper) ? DebtInstrumentFields[propertyUpper](t) : FB();
        //        case CSMForexFuture x: return ForexFutureLookupFields.ContainsKey(propertyUpper) ? ForexFutureLookupFields[propertyUpper](x) : FB();
        //        case CSMAmFund z: return FundFields.ContainsKey(propertyUpper) ? FundFields[propertyUpper](z) : FB();
        //        case CSMEquity e: return EquityFields.ContainsKey(propertyUpper) ? EquityFields[propertyUpper](e) : FB();
        //        case CSMInstrument i: return InstrumentFieldLookup.ContainsKey(propertyUpper) ? InstrumentFieldLookup[propertyUpper](i) : FB();

        //        default: return FB();
        //    }
        //}
    }
}
