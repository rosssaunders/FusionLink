//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System.Collections.Generic;

namespace RxdSolutions.FusionLink
{
    public class ObservableValue<T> : ObservableBase
    {
        private T _value;

        public ObservableValue()
        {
        }

        public ObservableValue(T value)
        {
            _value = value;
        }

        public T Value 
        {
            get 
            {
                return _value;
            }
            set 
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}