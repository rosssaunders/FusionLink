//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;

namespace RxdSolutions.FusionLink
{
    public class ObservableDataPoint<T> : ObservableBase
    {
        private object _value;

        public ObservableDataPoint(T key, object value)
        {
            CheckValidDataType(value);

            Key = key;
            _value = value;
        }

        public T Key { get; }

        public object Value 
        {
            get 
            {
                return _value;
            }
            set 
            {
                CheckValidDataType(value);

                if (_value is null && value is null)
                    return;

                var updateRequired = false;

                if(!_value.GetType().Equals(value?.GetType()))
                {
                    updateRequired = true;
                }
                else
                {
                    switch (value)
                    {
                        case int intValue:

                            if (!EqualityComparer<int>.Default.Equals((int)_value, intValue))
                            {
                                updateRequired = true;
                            }

                            break;

                        case long longValue:

                            if (!EqualityComparer<long>.Default.Equals((long)_value, longValue))
                            {
                                updateRequired = true;
                            }

                            break;

                        case decimal decimalValue:

                            if (!EqualityComparer<decimal>.Default.Equals((decimal)_value, decimalValue))
                            {
                                updateRequired = true;
                            }

                            break;

                        case double doubleValue:

                            if (!EqualityComparer<double>.Default.Equals((double)_value, doubleValue))
                            {
                                updateRequired = true;
                            }

                            break;

                        case string stringValue:

                            if (!EqualityComparer<string>.Default.Equals((string)_value, stringValue))
                            {
                                updateRequired = true;
                            }

                            break;

                        case DateTime dateTimeValue:

                            if (!EqualityComparer<DateTime>.Default.Equals((DateTime)_value, dateTimeValue))
                            {
                                updateRequired = true;
                            }

                            break;

                    }
                }
                
                if(updateRequired)
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void CheckValidDataType(object value)
        {
            var isValid = value is string || value is int || value is long || value is double || value is decimal || value is DateTime || value is null;
            if(!isValid)
            {
                throw new ApplicationException("Invalid data type passed");
            }
        }
    }
}