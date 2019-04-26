using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RxdSolutions.FusionLink
{
    public class Subscriptions<T>
    {
        private readonly ConcurrentDictionary<T, (ObservableDataPoint<T> dataPoint, HashSet<string> subscribers)> _subscriptions;
        public event EventHandler<DataPointChangedEventArgs<T>> OnValueChanged;

        public string DefaultMessage { get; set; } = "Getting data... please wait";

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
                }
            }

            _subscriptions[key].subscribers.Add(subscriber);

            return Get(key);
        }

        public void Remove(string subscriber, T key)
        {
            if (_subscriptions.ContainsKey(key))
            {
                _subscriptions[key].subscribers.Remove(subscriber);

                if(_subscriptions[key].subscribers.Count == 0)
                {
                    if (_subscriptions.TryRemove(key, out (ObservableDataPoint<T>, HashSet<string> subscribers) value))
                    {
                        value.Item1.PropertyChanged -= DataPointPropertyChanged;
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
