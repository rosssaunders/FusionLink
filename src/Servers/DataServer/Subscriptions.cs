//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using RxdSolutions.FusionLink.Properties;

namespace RxdSolutions.FusionLink
{
    public class Subscriptions<T>
    {
        private readonly ConcurrentDictionary<T, (ObservableDataPoint<T> dataPoint, HashSet<string> subscribers)> _subscriptions;

        public event EventHandler<DataPointChangedEventArgs<T>> OnValueChanged;

        public event EventHandler<SubscriptionChangedEventArgs<T>> SubscriptionAdded;

        public event EventHandler<SubscriptionChangedEventArgs<T>> SubscriptionRemoved;

        public string DefaultMessage { get; set; } = Resources.DefaultGettingDataMessage;

        public Subscriptions()
        {
            _subscriptions = new ConcurrentDictionary<T, (ObservableDataPoint<T>, HashSet<string> subscribers)>();
        }

        public ObservableDataPoint<T> Add(string subscriber, T key)
        {
            if (!_subscriptions.ContainsKey(key))
            {
                var dp = new ObservableDataPoint<T>(key, DefaultMessage);
                if (_subscriptions.TryAdd(dp.Key, (dp, new HashSet<string>())))
                {
                    dp.PropertyChanged += DataPointPropertyChanged;

                    SubscriptionAdded?.Invoke(this, new SubscriptionChangedEventArgs<T>(key));
                }
            }

            _subscriptions[key].subscribers.Add(subscriber);

            return Get(key);
        }

        public void Remove(string subscriber, T key)
        {
            if (_subscriptions.ContainsKey(key))
            {
                if(_subscriptions[key].subscribers.Contains(subscriber))
                {
                    _subscriptions[key].subscribers.Remove(subscriber);
                }

                if (_subscriptions[key].subscribers.Count == 0)
                {
                    if (_subscriptions.TryRemove(key, out (ObservableDataPoint<T> dp, HashSet<string> subscribers) value))
                    {
                        value.dp.PropertyChanged -= DataPointPropertyChanged;

                        SubscriptionRemoved?.Invoke(this, new SubscriptionChangedEventArgs<T>(key));
                    }
                }
            }
        }

        public ObservableDataPoint<T> Get(T key)
        {
            if(_subscriptions.TryGetValue(key, out (ObservableDataPoint<T> dp, HashSet<string> _) value))
            {
                return value.dp;
            }

            return null;
        }

        public bool IsSubscribed(string subscriber, T key)
        {
            if (_subscriptions.TryGetValue(key, out (ObservableDataPoint<T> dp, HashSet<string> subscribers) value))
            {
                return value.subscribers.Contains(subscriber);
            }

            return false;
        }

        public IEnumerable<T> GetKeys()
        {
            return _subscriptions.Keys.ToList();
        }

        public int Count 
        {
            get => _subscriptions.Count;
        }

        private void DataPointPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnValueChanged?.Invoke(this, new DataPointChangedEventArgs<T>(sender as ObservableDataPoint<T>));
        }
    }
}
