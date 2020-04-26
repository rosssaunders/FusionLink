//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink.Client
{
    [Serializable]
    public class CalendarNotFoundException : Exception
    {
        public CalendarNotFoundException() { }

        public CalendarNotFoundException(string message) : base(message) { }

        public CalendarNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected CalendarNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}