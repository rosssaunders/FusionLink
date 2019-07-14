//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink.Client
{
    [Serializable]
    public class DataServiceException : Exception
    {
        public DataServiceException() { }

        public DataServiceException(string message) : base(message) { }

        public DataServiceException(string message, Exception inner) : base(message, inner) { }

        protected DataServiceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}