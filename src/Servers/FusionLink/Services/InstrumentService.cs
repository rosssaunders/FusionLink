﻿//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RxdSolutions.FusionLink.Model;
using RxdSolutions.FusionLink.Provider;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Services
{
    public class InstrumentService
    {
        private readonly Dictionary<Type, Dictionary<string, Func<object, object>>> _dispatchMapping = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
        private readonly Dictionary<Type, Dictionary<string, bool>> _isMarketData = new Dictionary<Type, Dictionary<string, bool>>();

        public InstrumentService()
        {
            //Setup the Dispatch mapping
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(Instrument).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x);

            foreach(var x in types)
            {
                var allProperties = new Dictionary<string, Func<object, object>>();
                var marketDataProperties = new Dictionary<string, bool>();
                _dispatchMapping.Add(x, allProperties);
                _isMarketData.Add(x, marketDataProperties);

                foreach(var property in x.GetProperties())
                {
                    var md = property.GetCustomAttributes(typeof(MarketDataAttribute), true);
                    marketDataProperties.Add(property.Name.ToUpper(), md != null && md.Length > 0);

                    allProperties.Add(property.Name.ToUpper(), (obj) => property.GetValue(obj));
                }
            }
        }

        public object GetValue<T>(T instrument, string property) where T : Instrument
        {
            if(_dispatchMapping.ContainsKey(typeof(T)))
            {
                var propertyDict = _dispatchMapping[typeof(T)];

                if(propertyDict.ContainsKey(property.ToUpper()))
                {
                    return propertyDict[property.ToUpper()](instrument);
                }
                else
                {
                    return $"Unknown Instrument Property '{property}'";
                }
            }
            else
            {
                return $"Unknown Instrument Type '{nameof(instrument)}'";
            }
        }

        public bool IsMarketData<T>(T instrument, string property) where T : Instrument
        {
            if (_isMarketData.ContainsKey(typeof(T)))
            {
                var propertyDict = _isMarketData[typeof(T)];

                if (propertyDict.ContainsKey(property.ToUpper()))
                {
                    return propertyDict[property.ToUpper()];
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public DataTable GetInstrumentSet(int instrumentId, string property)
        {
            var item = new InstrumentPropertyValue(instrumentId, null, property, this);

            var result = item.GetValue();

            if(result is DataTable dt)
            {
                return dt;
            }

            throw new InvalidFieldException("Invalid Property value for set");
        }

        public DataTable GetInstrumentSet(string reference, string property)
        {
            var inst = CSMInstrument.GetCodeWithReference(reference);

            if (inst == 0)
                throw new InstrumentNotFoundException();

            return GetInstrumentSet(inst, property);
        }
    }
}
