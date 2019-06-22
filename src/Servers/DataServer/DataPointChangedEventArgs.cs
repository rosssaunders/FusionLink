//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink
{
    public class DataPointChangedEventArgs<T> : EventArgs
    {
        public DataPointChangedEventArgs(ObservableDataPoint<T> dataPoint)
        {
            DataPoint = dataPoint;
        }

        public ObservableDataPoint<T> DataPoint { get; }
    }
}
