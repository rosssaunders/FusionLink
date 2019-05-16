//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace RxdSolutions.FusionLink
{
    internal class CellSubscriptions<T> where T : CellValueBase
    {
        private Dictionary<int, Dictionary<string, T>> _subscriptions;
        private readonly Func<int, string, T> _factory;

        public CellSubscriptions(Func<int, string, T> factory)
        {
            _subscriptions = new Dictionary<int, Dictionary<string, T>>();
            _factory = factory;
        }

        public IEnumerable<T> GetCells()
        {
            return _subscriptions.Values.SelectMany(x => x.Values);
        }

        public void Add(int id, string column)
        {
            if(!_subscriptions.ContainsKey(id))
            {
                var columns = new Dictionary<string, T> {
                    { column, _factory(id, column) }
                };

                _subscriptions.Add(id, columns);
            }

            if(!_subscriptions[id].ContainsKey(column))
            {
                _subscriptions[id].Add(column, _factory(id, column));
            }
        }

        public void Remove(int folioId, string column)
        {
            if (_subscriptions.ContainsKey(folioId))
            {
                _subscriptions[folioId].Clear();

                _subscriptions[folioId].Remove(column);
            }
        }

        public IEnumerable<T> Get(int folioId)
        {
            if(_subscriptions.ContainsKey(folioId))
            {
                return _subscriptions[folioId].Values;
            }

            return Enumerable.Empty<T>();
        }

        public T Get(int folioId, string column)
        {
            if (_subscriptions.ContainsKey(folioId))
            {
                return _subscriptions[folioId][column];
            }

            return null;
        }
    }
}