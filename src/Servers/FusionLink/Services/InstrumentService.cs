//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using RxdSolutions.FusionLink.Model;

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
    }
}
