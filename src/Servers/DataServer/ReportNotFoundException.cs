//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink
{
    [Serializable]
    public class ReportNotFoundException : Exception
    {
        public ReportNotFoundException() { }

        public ReportNotFoundException(string message) : base(message) { }

        public ReportNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected ReportNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}