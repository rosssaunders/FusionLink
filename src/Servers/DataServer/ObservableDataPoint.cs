//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;

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

                var updateRequired = false;

                switch (value)
                {
                    case int intValue:

                        if(_value is int)
                        {
                            if (!EqualityComparer<int>.Default.Equals((int)_value, intValue))
                            {
                                updateRequired = true;
                            }
                        }
                        else
                        {
                            updateRequired = true;
                        }
                        
                        break;

                    case decimal decimalValue:

                        if (_value is decimal)
                        {
                            if (!EqualityComparer<decimal>.Default.Equals((decimal)_value, decimalValue))
                            {
                                updateRequired = true;
                            }
                        }
                        else
                        {
                            updateRequired = true;
                        }

                        break;

                    case double doubleValue:

                        if (_value is double)
                        {
                            if (!EqualityComparer<double>.Default.Equals((double)_value, doubleValue))
                            {
                                updateRequired = true;
                            }
                        }
                        else
                        {
                            updateRequired = true;
                        }

                        break;

                    case string stringValue:

                        if (_value is string)
                        {
                            if (!EqualityComparer<string>.Default.Equals((string)_value, stringValue))
                            {
                                updateRequired = true;
                            }
                        }
                        else
                        {
                            updateRequired = true;
                        }

                        break;

                    case DateTime dateTimeValue:

                        if (_value is DateTime)
                        {
                            if (!EqualityComparer<DateTime>.Default.Equals((DateTime)_value, dateTimeValue))
                            {
                                updateRequired = true;
                            }
                        }
                        else
                        {
                            updateRequired = true;
                        }

                        break;

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
            var isValid = (value is string || value is int || value is double || value is decimal || value is DateTime);
            if(!isValid)
            {
                throw new ApplicationException("Invalid data type passed");
            }
        }
    }
}