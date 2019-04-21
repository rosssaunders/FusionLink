using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RxdSolutions.FusionLink
{

    public class Subscriptions<T>
    {
        private readonly ConcurrentDictionary<T, ObservableDataPoint<T>> _subscriptions;
        public event EventHandler<DataPointChangedEventArgs<T>> OnValueChanged;

        public string DefaultMessage { get; set; } = "Getting data... please wait";

        public Subscriptions()
        {
            _subscriptions = new ConcurrentDictionary<T, ObservableDataPoint<T>>();
        }

        public ObservableDataPoint<T> Add(T key)
        {
            if (!_subscriptions.ContainsKey(key))
            {
                var dp = new ObservableDataPoint<T>(key, DefaultMessage);
                if (_subscriptions.TryAdd(dp.Key, dp))
                {
                    dp.PropertyChanged += DataPointPropertyChanged;
                }
            }

            return Get(key);
        }

        public void Remove(T key)
        {
            if (!_subscriptions.ContainsKey(key))
                if(_subscriptions.TryRemove(key, out ObservableDataPoint<T> value))
                {
                    value.PropertyChanged -= DataPointPropertyChanged;
                }
        }

        public ObservableDataPoint<T> Get(T key)
        {
            if(_subscriptions.TryGetValue(key, out ObservableDataPoint<T> value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public IEnumerable<T> GetKeys()
        {
            return _subscriptions.Keys.ToList();
        }

        private void DataPointPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnValueChanged?.Invoke(this, new DataPointChangedEventArgs<T>(sender as ObservableDataPoint<T>));
        }
    }
}
