//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using System.Linq;

namespace RxdSolutions.FusionLink.Provider
{
    internal class Subscriptions<T, U, V> where T : class, IDisposable 
    {
        private Dictionary<V, Dictionary<U, T>> _subscriptions;
        private readonly Func<V, U, T> _factory;

        public Subscriptions(Func<V, U, T> factory)
        {
            _subscriptions = new Dictionary<V, Dictionary<U, T>>();
            _factory = factory;
        }

        public int Count => _subscriptions.Count;

        public IEnumerable<T> GetCells()
        {
            return _subscriptions.Values.SelectMany(x => x.Values);
        }

        public void Add(V id, U property)
        {
            if (!_subscriptions.ContainsKey(id))
            {
                var columns = new Dictionary<U, T>
                {
                    { property, _factory(id, property) }
                };

                _subscriptions.Add(id, columns);
            }

            if (!_subscriptions[id].ContainsKey(property))
            {
                _subscriptions[id].Add(property, _factory(id, property));
            }
        }

        public void Remove(V id, U column)
        {
            if (_subscriptions.ContainsKey(id))
            {
                var cv = _subscriptions[id][column];
                cv.Dispose();

                _subscriptions[id].Remove(column);
            }
        }

        public IEnumerable<T> Get(V id)
        {
            if (_subscriptions.ContainsKey(id))
            {
                return _subscriptions[id].Values;
            }

            return Enumerable.Empty<T>();
        }

        public T Get(V id, U property)
        {
            if (_subscriptions.ContainsKey(id))
            {
                return _subscriptions[id][property];
            }

            return null;
        }
    }
}