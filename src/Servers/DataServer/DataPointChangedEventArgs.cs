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
